using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using ElderCare.Api.Infrastructure;

namespace ElderCare.Api.Modules.Activities
{
    /// <summary>
    /// 活动报名/签到/查询 
    /// - 状态：已报名 / 已参加；“缺席”为查询派生，不落库
    /// </summary>
    public sealed class ParticipationRepository
    {
        private readonly IDbConnectionFactory _factory;
        public ParticipationRepository(IDbConnectionFactory factory) => _factory = factory;

        /// <summary>报名：写入一条“已报名”，自动记录报名时间</summary>
        public async Task<int> RegisterAsync(int activity_id, int elderly_id, IDbTransaction tx)
        {
            var conn = tx.Connection ?? throw new InvalidOperationException("Transaction has no connection.");

            // 1) 活动存在 & 是否仍在报名中
            int? isOpen = await conn.QuerySingleOrDefaultAsync<int?>(@$"
        SELECT CASE 
                 WHEN s.activity_date + s.activity_time > (SYSTIMESTAMP AT LOCAL) THEN 1 
                 ELSE 0 
               END AS IsOpen
          FROM {Tables.ACTIVITY_SCHEDULE} s
         WHERE s.activity_id = :id",
                new { id = activity_id }, tx);

            if (isOpen == null)
                throw new ArgumentException("活动不存在");
            if (isOpen.Value != 1)
                throw new ArgumentException("活动已结束，无法报名");

            // 2) 老人存在
            var elderCnt = await conn.ExecuteScalarAsync<int>(
                $@"SELECT COUNT(1) FROM {Tables.ELDERLY_INFO} WHERE ELDERLY_ID = :id",
                new { id = elderly_id }, tx);
            if (elderCnt == 0) throw new ArgumentException("老人不存在");

            // 3) 防重复报名
            var dup = await conn.ExecuteScalarAsync<int>(
                $@"SELECT COUNT(1) FROM {Tables.ACTIVITY_PARTICIPATION}
           WHERE activity_id=:aid AND elderly_id=:eid",
                new { aid = activity_id, eid = elderly_id }, tx);
            if (dup > 0) throw new ArgumentException("已报名，无需重复报名");

            // 4) 插入报名（status=已报名）
            var p = new DynamicParameters(new { activity_id, elderly_id, ts = DateTime.Now });
            p.Add("new_id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await conn.ExecuteAsync($@"
        INSERT INTO {Tables.ACTIVITY_PARTICIPATION}
          (activity_id, elderly_id, status, registration_time)
        VALUES
          (:activity_id, :elderly_id, '已报名', :ts)
        RETURNING participation_id INTO :new_id", p, tx);

            return p.Get<int>("new_id");
        }


        /// <summary>签到：将状态置为“已参加”，自动记录签到时间</summary>
        public async Task<int> CheckInAsync(int activity_id, int elderly_id, IDbTransaction tx)
        {
            var conn = tx.Connection ?? throw new InvalidOperationException("Transaction has no connection.");

            // 必须先有报名记录
            var exists = await conn.ExecuteScalarAsync<int>(
                $@"SELECT COUNT(1) FROM {Tables.ACTIVITY_PARTICIPATION}
                   WHERE activity_id=:aid AND elderly_id=:eid",
                new { aid = activity_id, eid = elderly_id }, tx);
            if (exists == 0) throw new ArgumentException("未报名，无法签到");

            // 更新为已参加，记录签到时间
            var rows = await conn.ExecuteAsync($@"
                UPDATE {Tables.ACTIVITY_PARTICIPATION}
                   SET status='已参加', check_in_time = :now
                 WHERE activity_id=:aid AND elderly_id=:eid",
                new { now = DateTime.Now, aid = activity_id, eid = elderly_id }, tx);

            return rows;
        }

        /// <summary>取消报名：仅允许活动尚未开始 且 当前为“已报名”的记录</summary>
        public async Task<int> CancelAsync(int activity_id, int elderly_id, IDbTransaction tx)
        {
            var conn = tx.Connection ?? throw new InvalidOperationException("Transaction has no connection.");

            var canCancel = await conn.ExecuteScalarAsync<int>($@"
                SELECT CASE 
                         WHEN s.activity_date + s.activity_time > (SYSTIMESTAMP AT LOCAL) 
                              AND p.status = '已报名'
                         THEN 1 ELSE 0 END
                FROM {Tables.ACTIVITY_SCHEDULE} s
                JOIN {Tables.ACTIVITY_PARTICIPATION} p
                  ON p.activity_id = s.activity_id
               WHERE p.activity_id=:aid AND p.elderly_id=:eid",
               new { aid = activity_id, eid = elderly_id }, tx);

            if (canCancel != 1) throw new ArgumentException("当前不可取消（可能活动已结束或已签到）");

            var rows = await conn.ExecuteAsync($@"
                DELETE FROM {Tables.ACTIVITY_PARTICIPATION}
                WHERE activity_id=:aid AND elderly_id=:eid",
                new { aid = activity_id, eid = elderly_id }, tx);

            return rows;
        }

        /// <summary>
        /// GET (by activity_id)：
        /// 活动“报名中”→ 返回“已报名”列表；活动“已结束”→ 返回“已参加”列表
        /// </summary>
        public async Task<IReadOnlyList<ActivityParticipationItemDto>> ListByActivityAsync(int activity_id, IDbTransaction? tx = null)
        {
            const string core = @"
                SELECT
                  p.participation_id,
                  p.activity_id,
                  p.elderly_id,
                  e.NAME AS elderly_name,
                  p.status,
                  p.registration_time,
                  p.check_in_time
                FROM {0} s
                JOIN {1} p ON p.activity_id = s.activity_id
                JOIN {2} e ON e.ELDERLY_ID   = p.elderly_id
               WHERE s.activity_id = :aid
                 AND p.status = CASE 
                                  WHEN s.activity_date + s.activity_time > (SYSTIMESTAMP AT LOCAL) 
                                    THEN '已报名' 
                                  ELSE '已参加' 
                                END
               ORDER BY e.NAME";
            var sql = string.Format(core, Tables.ACTIVITY_SCHEDULE, Tables.ACTIVITY_PARTICIPATION, Tables.ELDERLY_INFO);

            if (tx != null)
            {
                var c = tx.Connection ?? throw new InvalidOperationException("Transaction has no connection.");
                var rowsTx = await c.QueryAsync<ActivityParticipationItemDto>(sql, new { aid = activity_id }, tx);
                return rowsTx.ToList();
            }

            using var conn = _factory.Create();
            if (conn.State != ConnectionState.Open) conn.Open();  
            var rows = await conn.QueryAsync<ActivityParticipationItemDto>(sql, new { aid = activity_id });
            return rows.ToList();
        }

        public async Task<IReadOnlyList<ElderlyParticipationItemDto>> ListByElderlyAsync(int elderly_id, IDbTransaction? tx = null)
        {
            const string core = @"
                SELECT
                  p.participation_id,
                  p.activity_id,
                  s.activity_name,
                  s.activity_date,
                  TO_CHAR(s.activity_time, 'HH24:MI:SS.FF6') AS activity_time,
                  s.location,
                  p.status AS raw_status,
                  CASE 
                    WHEN s.activity_date + s.activity_time <= (SYSTIMESTAMP AT LOCAL)
                         AND p.status <> '已参加'
                    THEN '缺席'
                    ELSE p.status
                  END AS display_status,
                  p.registration_time,
                  p.check_in_time
                FROM {0} p
                JOIN {1} s ON s.activity_id = p.activity_id
               WHERE p.elderly_id = :eid
               ORDER BY s.activity_date DESC, s.activity_time DESC";
            var sql = string.Format(core, Tables.ACTIVITY_PARTICIPATION, Tables.ACTIVITY_SCHEDULE);

            if (tx != null)
            {
                var c = tx.Connection ?? throw new InvalidOperationException("Transaction has no connection.");
                var rowsTx = await c.QueryAsync<ElderlyParticipationItemDto>(sql, new { eid = elderly_id }, tx);
                return rowsTx.ToList();
            }

            using var conn = _factory.Create();
            if (conn.State != ConnectionState.Open) conn.Open();   
            var rows = await conn.QueryAsync<ElderlyParticipationItemDto>(sql, new { eid = elderly_id });
            return rows.ToList();
        }

        /// <summary>（可选）按活动清理全部报名记录</summary>
        public Task<int> DeleteByActivityAsync(int activity_id, IDbTransaction tx)
        {
            var conn = tx.Connection ?? throw new InvalidOperationException("Transaction has no connection.");
            return conn.ExecuteAsync(
                $"DELETE FROM {Tables.ACTIVITY_PARTICIPATION} WHERE activity_id = :aid",
                new { aid = activity_id }, tx);
        }
    }
}
