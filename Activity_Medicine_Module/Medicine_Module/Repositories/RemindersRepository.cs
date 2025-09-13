using System.Data;
using Dapper;
using ElderCare.Api.Infrastructure;

namespace ElderCare.Api.Modules.Medical
{
    public sealed class ReminderRepository
    {
        private readonly IDbConnectionFactory _factory;
        public ReminderRepository(IDbConnectionFactory factory) => _factory = factory;

        private static readonly string TABLE = Tables.VOICE_REMINDER;
        private const string STATUS_PENDING = "待提醒";
        private const string STATUS_TAKEN = "已服药";

        /// <summary>创建一条“待提醒”记录</summary>
        public async Task<int> CreateAsync(CreateReminderDto dto)
        {
            using var conn = _factory.Create(); // 工厂已打开连接
            var sql = $@"
                INSERT INTO {TABLE}
                    (ORDER_ID, ELDERLY_ID, REMINDER_TIME, REMINDER_COUNT, REMINDER_STATUS)
                VALUES
                    (:order_id, :elderly_id, :reminder_time, :reminder_count, :status)
                RETURNING REMINDER_ID INTO :new_id";

            var p = new DynamicParameters(new
            {
                order_id = dto.order_id,
                elderly_id = dto.elderly_id,
                reminder_time = dto.reminder_time,
                reminder_count = dto.reminder_count,
                status = STATUS_PENDING
            });
            p.Add("new_id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await conn.ExecuteAsync(sql, p);
            return p.Get<int>("new_id");
        }

        /// <summary>将某条提醒更新为“已服药”</summary>
        public Task<int> MarkTakenAsync(int reminder_id)
        {
            using var conn = _factory.Create();
            var sql = $@"
                UPDATE {TABLE}
                   SET REMINDER_STATUS = :status
                 WHERE REMINDER_ID = :id
                   AND (REMINDER_STATUS <> :status OR REMINDER_STATUS IS NULL)";
            return conn.ExecuteAsync(sql, new { status = STATUS_TAKEN, id = reminder_id });
        }

        /// <summary>按医嘱ID查询提醒</summary>
        public async Task<IReadOnlyList<ReminderItemDto>> ListByOrderAsync(int order_id)
        {
            using var conn = _factory.Create();
            var sql = $@"
                SELECT
                    REMINDER_ID      AS reminder_id,
                    ORDER_ID         AS order_id,
                    ELDERLY_ID       AS elderly_id,
                    REMINDER_TIME    AS reminder_time,
                    REMINDER_COUNT   AS reminder_count,
                    REMINDER_STATUS  AS reminder_status
                  FROM {TABLE}
                 WHERE ORDER_ID = :order_id
                 ORDER BY REMINDER_TIME DESC";
            var rows = await conn.QueryAsync<ReminderItemDto>(sql, new { order_id });
            return rows.ToList();
        }

        /// <summary>删除一条提醒</summary>
        public Task<int> DeleteAsync(int reminder_id)
        {
            using var conn = _factory.Create();
            var sql = $"DELETE FROM {TABLE} WHERE REMINDER_ID = :id";
            return conn.ExecuteAsync(sql, new { id = reminder_id });
        }
    }
}
