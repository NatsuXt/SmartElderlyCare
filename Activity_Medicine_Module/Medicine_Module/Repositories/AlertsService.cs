using System.Data;
using Dapper;
using ElderCare.Api.Infrastructure;

namespace ElderCare.Api.Modules.Medical
{
    /// <summary>
    /// 健康告警服务（直接访问数据库，无仓储）
    /// 依赖外部 DTO/实体：HealthDataSampleDto、HealthAlert
    /// </summary>
    public sealed class AlertsService
    {
        private readonly IDbConnectionFactory _factory;
        private readonly string _alertsTable;     
        private readonly string _thresholdTable;  

        public AlertsService(IDbConnectionFactory factory, IConfiguration cfg)
        {
            _factory = factory;

            var schema = cfg.GetSection("Oracle")["Schema"];
            _alertsTable = string.IsNullOrWhiteSpace(schema) ? "HEALTHALERT" : $"{schema}.HEALTHALERT";
            _thresholdTable = string.IsNullOrWhiteSpace(schema) ? "HEALTHTHRESHOLD" : $"{schema}.HEALTHTHRESHOLD";
        }

        private static void EnsureOpen(IDbConnection conn)
        {
            if (conn.State != ConnectionState.Open) conn.Open();
        }

        /// <summary>
        /// 用于“人工上报”或外部监测数据推送时的一键检查入口：
        /// 1) 读取老人对应阈值；2) 超范围则落表 HEALTHALERT；3) 返回新 ALERT_ID（未触发返回 0）
        /// </summary>
        public async Task<int> CheckAsync(HealthDataSampleDto dto)
        {
            var mapped = new AlertCheckDto
            {
                elderly_id = dto.elderly_id,
                alert_type = dto.alert_type ?? string.Empty,
                value = Convert.ToDecimal(dto.value),     
                record_time = dto.record_time ?? DateTime.Now,
                notified_staff_id = dto.notified_staff_id
            };

            return await CheckAndInsertAsync(mapped).ConfigureAwait(false);
        }

        // 内部映射 DTO
        private sealed class AlertCheckDto
        {
            public int elderly_id { get; set; }
            public string alert_type { get; set; } = string.Empty;
            public decimal value { get; set; }
            public DateTime record_time { get; set; }
            public int? notified_staff_id { get; set; }
        }

        /// <summary>
        /// 私有：检查阈值并在达标时写入 HEALTHALERT 表，返回新 ALERT_ID（若不报警，返回 0）
        /// </summary>
        private async Task<int> CheckAndInsertAsync(AlertCheckDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.alert_type))
                return 0;

            using var conn = _factory.Create();
            EnsureOpen(conn);
            using var tx = conn.BeginTransaction();

            try
            {
                // 1) 读取阈值（个体 + 指标）
                var getThdSql = $@"
SELECT MIN_VALUE, MAX_VALUE
  FROM {_thresholdTable}
 WHERE ELDERLY_ID = :elderly_id
   AND DATA_TYPE   = :data_type";

                var th = await conn.QuerySingleOrDefaultAsync<(decimal MIN_VALUE, decimal MAX_VALUE)>(
                    getThdSql, new { elderly_id = dto.elderly_id, data_type = dto.alert_type }, tx);

                // 未配置阈值：不触发（按业务可改为默认触发或默认不过滤）
                if (th == default)
                {
                    tx.Commit();
                    return 0;
                }

                bool isAlert = dto.value < th.MIN_VALUE || dto.value > th.MAX_VALUE;
                if (!isAlert)
                {
                    tx.Commit();
                    return 0;
                }

                // 2) 插入 HEALTHALERT（ALERT_ID 有默认序列/触发器，RETURNING 取新 ID）
                var insertSql = $@"
INSERT INTO {_alertsTable}
  (ELDERLY_ID, ALERT_TYPE, ALERT_TIME, ALERT_VALUE, NOTIFIED_STAFF_ID, STATUS)
VALUES
  (:elderly_id, :alert_type, :alert_time, :alert_value, :staff_id, 'OPEN')
RETURNING ALERT_ID INTO :new_id";

                var p = new DynamicParameters();
                p.Add("elderly_id", dto.elderly_id);
                p.Add("alert_type", dto.alert_type);
                p.Add("alert_time", dto.record_time);
                p.Add("alert_value", dto.value);
                p.Add("staff_id", dto.notified_staff_id);
                p.Add("new_id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await conn.ExecuteAsync(insertSql, p, tx);
                int newId = p.Get<int>("new_id");

                tx.Commit();
                return newId;
            }
            catch
            {
                try { tx.Rollback(); } catch { }
                throw;
            }
        }

        /// <summary> PUT：关闭告警（OPEN → CLOSED） </summary>
        public async Task<int> CloseAsync(int alert_id)
        {
            using var conn = _factory.Create();
            EnsureOpen(conn);

            var sql = $@"
UPDATE {_alertsTable}
   SET STATUS = 'CLOSED'
 WHERE ALERT_ID = :id
   AND STATUS = 'OPEN'";

            return await conn.ExecuteAsync(sql, new { id = alert_id });
        }

        /// <summary>
        /// GET：分页查询（elderly_id / alert_type / status / 时间区间）
        /// </summary>
        public async Task<IReadOnlyList<HealthAlert>> ListAsync(
            int page, int pageSize,
            int? elderly_id = null,
            string? alert_type = null,
            string? status = null,
            DateTime? from = null,
            DateTime? to = null)
        {
            using var conn = _factory.Create();
            EnsureOpen(conn);

            var sql = $@"
SELECT
    ALERT_ID,
    ELDERLY_ID,
    ALERT_TYPE,
    ALERT_TIME,
    ALERT_VALUE,
    NOTIFIED_STAFF_ID,
    STATUS
FROM {_alertsTable}
WHERE 1=1";

            var p = new DynamicParameters();

            if (elderly_id.HasValue)
            {
                sql += " AND ELDERLY_ID = :elderly_id";
                p.Add("elderly_id", elderly_id.Value);
            }
            if (!string.IsNullOrWhiteSpace(alert_type))
            {
                sql += " AND ALERT_TYPE = :alert_type";
                p.Add("alert_type", alert_type);
            }
            if (!string.IsNullOrWhiteSpace(status))
            {
                sql += " AND STATUS = :status";
                p.Add("status", status);
            }
            if (from.HasValue)
            {
                sql += " AND ALERT_TIME >= :from";
                p.Add("from", from.Value);
            }
            if (to.HasValue)
            {
                sql += " AND ALERT_TIME < :to";
                p.Add("to", to.Value);
            }

            sql += " ORDER BY ALERT_TIME DESC, ALERT_ID DESC";
            sql += " OFFSET :skip ROWS FETCH NEXT :take ROWS ONLY";
            p.Add("skip", Math.Max(0, (page - 1) * pageSize));
            p.Add("take", pageSize);

            var rows = await conn.QueryAsync<HealthAlert>(sql, p);
            return rows.ToList();
        }
    }
}
