using System.Data;
using Dapper;
using ElderCare.Api.Infrastructure;

namespace ElderCare.Api.Modules.Medical
{
    /// <summary>
    /// 直接用类，不用接口；表名：HEALTHTHRESHOLD
    /// </summary>
    public sealed class HealthThresholdsService
    {
        private const string TABLE = "HEALTHTHRESHOLD";
        private readonly IDbConnectionFactory _factory;

        public HealthThresholdsService(IDbConnectionFactory factory) => _factory = factory;

        private static void EnsureOpen(IDbConnection conn)
        {
            if (conn.State != ConnectionState.Open) conn.Open();
        }

        /// <summary>
        /// 若 (elderly_id, data_type) 存在则更新；否则插入。主键是 IDENTITY，不手动填。
        /// </summary>
        public async Task<int> UpsertAsync(HealthThresholdUpsertDto dto)
        {
            using var conn = _factory.Create();
            EnsureOpen(conn);

            using var tx = conn.BeginTransaction();
            try
            {
                // 1) 先找是否已存在同一个 (ELDERLY_ID, DATA_TYPE)
                var existedId = await conn.ExecuteScalarAsync<int?>(
                    $@"SELECT THRESHOLD_ID 
                         FROM {TABLE}
                        WHERE ELDERLY_ID = :elderly_id 
                          AND DATA_TYPE  = :data_type
                        FETCH FIRST 1 ROWS ONLY",
                    new { dto.elderly_id, dto.data_type }, tx);

                if (existedId is not null)
                {
                    // 2a) 已存在：更新
                    var rows = await conn.ExecuteAsync(
                        $@"UPDATE {TABLE}
                              SET MIN_VALUE   = :min_value,
                                  MAX_VALUE   = :max_value,
                                  DESCRIPTION = :description
                            WHERE THRESHOLD_ID = :id",
                        new
                        {
                            id = existedId.Value,
                            dto.min_value,
                            dto.max_value,
                            dto.description
                        }, tx);

                    tx.Commit();
                    return existedId.Value;
                }
                else
                {
                    // 2b) 不存在：插入 —— 不写 THRESHOLD_ID，让 IDENTITY 默认值生效
                    var p = new DynamicParameters();
                    p.Add("elderly_id", dto.elderly_id);
                    p.Add("data_type", dto.data_type);
                    p.Add("min_value", dto.min_value);
                    p.Add("max_value", dto.max_value);
                    p.Add("description", dto.description);
                    p.Add("new_id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    await conn.ExecuteAsync(
                        $@"INSERT INTO {TABLE}
                               (ELDERLY_ID, DATA_TYPE, MIN_VALUE, MAX_VALUE, DESCRIPTION)
                         VALUES (:elderly_id, :data_type, :min_value, :max_value, :description)
                         RETURNING THRESHOLD_ID INTO :new_id", p, tx);

                    var newId = p.Get<int>("new_id");
                    tx.Commit();
                    return newId;
                }
            }
            catch
            {
                try { tx.Rollback(); } catch { /* ignore */ }
                throw;
            }
        }

        /// <summary>
        /// 分页查询，可选按 elderly_id / data_type 过滤。
        /// </summary>
        public async Task<IReadOnlyList<HealthThreshold>> ListAsync(
            int page, int pageSize, int? elderly_id = null, string? data_type = null)
        {
            using var conn = _factory.Create();
            EnsureOpen(conn);

            var where = "WHERE 1=1";
            var p = new DynamicParameters();

            if (elderly_id.HasValue)
            {
                where += " AND ELDERLY_ID = :elderly_id";
                p.Add("elderly_id", elderly_id.Value);
            }

            if (!string.IsNullOrWhiteSpace(data_type))
            {
                where += " AND DATA_TYPE = :data_type";
                p.Add("data_type", data_type);
            }

            var s = Math.Max(0, (page - 1) * pageSize) + 1;
            var e = page * pageSize;

            var sql = $@"
SELECT * FROM (
  SELECT t.*,
         ROW_NUMBER() OVER (ORDER BY THRESHOLD_ID DESC) rn
    FROM {TABLE} t
   {where}
)
WHERE rn BETWEEN :s AND :e";

            p.Add("s", s);
            p.Add("e", e);

            var rows = await conn.QueryAsync<HealthThreshold>(sql, p);
            return rows.ToList();
        }

        /// <summary>按主键取单条</summary>
        public async Task<HealthThreshold?> GetAsync(int threshold_id)
        {
            using var conn = _factory.Create();
            EnsureOpen(conn);

            var sql = $@"SELECT THRESHOLD_ID, ELDERLY_ID, DATA_TYPE, MIN_VALUE, MAX_VALUE, DESCRIPTION
                           FROM {TABLE}
                          WHERE THRESHOLD_ID = :id";

            return await conn.QuerySingleOrDefaultAsync<HealthThreshold>(sql, new { id = threshold_id });
        }

        /// <summary>删除</summary>
        public async Task<int> DeleteAsync(int threshold_id)
        {
            using var conn = _factory.Create();
            EnsureOpen(conn);
            return await conn.ExecuteAsync(
                $@"DELETE FROM {TABLE} WHERE THRESHOLD_ID = :id",
                new { id = threshold_id });
        }

        // ======= DTO / Model =======
        public sealed record HealthThresholdUpsertDto(
            int elderly_id,
            string data_type,
            double min_value,
            double max_value,
            string? description
        );

        public sealed record HealthThreshold
        {
            public int THRESHOLD_ID { get; init; }
            public int ELDERLY_ID { get; init; }
            public string DATA_TYPE { get; init; } = "";
            public double MIN_VALUE { get; init; }
            public double MAX_VALUE { get; init; }
            public string? DESCRIPTION { get; init; }
        }
    }
}
