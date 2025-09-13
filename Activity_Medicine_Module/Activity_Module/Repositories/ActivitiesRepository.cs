using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using Dapper;
using ElderCare.Api.Infrastructure;

namespace ElderCare.Api.Modules.Activities
{
    /// <summary>
    /// 活动仓储（仅返回 elderly_participants，按状态动态生成）：
    /// - 报名中：elderly_participants = 已报名名单
    /// - 已结束：elderly_participants = 已参加名单
    /// </summary>
    public sealed class ActivitiesRepository : IActivitiesRepository
    {
        private readonly IDbConnectionFactory _factory;
        public ActivitiesRepository(IDbConnectionFactory factory) => _factory = factory;

        #region Create / Update

        /// <summary>
        /// 新建活动。前端传入的 activity_time 允许 "HH:mm" / "HH:mm:ss" / "HH:mm:ss.ffff"。
        /// 存库时写入 INTERVAL DAY(2) TO SECOND(4)（使用 TO_DSINTERVAL）。
        /// </summary>
        public async Task<int> CreateAsync(ActivitySchedule a, IDbTransaction tx)
        {
            if (a is null) throw new ArgumentNullException(nameof(a));

            // 规范化时间字符串
            var raw = (a.activity_time ?? string.Empty).Replace(" ", string.Empty);
            if (!TimeSpan.TryParseExact(
                    raw,
                    new[] { @"hh\:mm", @"h\:mm", @"hh\:mm\:ss", @"h\:mm\:ss", @"hh\:mm\:ss\.ffff", @"h\:mm\:ss\.ffff" },
                    CultureInfo.InvariantCulture,
                    out var ts))
            {
                throw new ArgumentException("activity_time 必须是 HH:mm、HH:mm:ss 或 HH:mm:ss.ffff 格式");
            }

            // 生成 FF4 的 DS INTERVAL 字符串：'DD HH:MI:SS.FF4'（此处 DD 固定 0）
            var fractionalTicks = ts.Ticks % TimeSpan.TicksPerSecond; // 100ns
            var ff4 = (int)(fractionalTicks / 1000);
            var timeStr = $"0 {ts:hh\\:mm\\:ss}.{ff4:D4}";

            var sql = $@"
                INSERT INTO {Tables.ACTIVITY_SCHEDULE}
                    (activity_name, activity_date, activity_time, location, staff_id, elderly_participants, activity_description)
                VALUES
                    (:activity_name, :activity_date, TO_DSINTERVAL(:timeStr), :location, :staff_id, :elderly_participants, :activity_description)
                RETURNING activity_id INTO :new_id";

            var conn = tx.Connection ?? throw new InvalidOperationException("Transaction has no connection.");
            var p = new DynamicParameters(new
            {
                a.activity_name,
                a.activity_date,
                timeStr,
                a.location,
                a.staff_id,
                a.elderly_participants,   // 存在也不会用于对外显示，GET 时会被动态名单覆盖
                a.activity_description
            });
            p.Add("new_id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await conn.ExecuteAsync(sql, p, tx);
            return p.Get<int>("new_id");
        }

        /// <summary>
        /// 更新活动（传入了 activity_time 才更新该列）。
        /// </summary>
        public Task UpdateAsync(int id, UpdateActivityDto dto, IDbTransaction tx)
        {
            if (dto is null) throw new ArgumentNullException(nameof(dto));

            string? timeStr = null;
            if (!string.IsNullOrWhiteSpace(dto.activity_time))
            {
                var cleaned = dto.activity_time!.Replace(" ", string.Empty);
                if (!TimeSpan.TryParseExact(
                        cleaned,
                        new[] { @"hh\:mm", @"h\:mm", @"hh\:mm\:ss", @"h\:mm\:ss", @"hh\:mm\:ss\.ffff", @"h\:mm\:ss\.ffff" },
                        CultureInfo.InvariantCulture,
                        out var ts))
                {
                    throw new ArgumentException("activity_time 必须是 HH:mm、HH:mm:ss 或 HH:mm:ss.ffff 格式");
                }

                var fractionalTicks = ts.Ticks % TimeSpan.TicksPerSecond;
                var ff4 = (int)(fractionalTicks / 1000);
                timeStr = $"0 {ts:hh\\:mm\\:ss}.{ff4:D4}";
            }

            var sql = $@"
        UPDATE {Tables.ACTIVITY_SCHEDULE} SET
          activity_name        = COALESCE(:activity_name, activity_name),
          activity_date        = COALESCE(:activity_date, activity_date),
          activity_time        = CASE WHEN :timeStr IS NULL THEN activity_time ELSE TO_DSINTERVAL(:timeStr) END,
          location             = COALESCE(:location, location),
          activity_description = COALESCE(:activity_description, activity_description)
        WHERE activity_id = :id";

            var conn = tx.Connection ?? throw new InvalidOperationException("Transaction has no connection.");
            return conn.ExecuteAsync(sql, new
            {
                id,
                dto.activity_name,
                dto.activity_date,
                timeStr,
                dto.location,
                dto.activity_description
            }, tx);
        }

        #endregion

        #region Read (包含：elderly_participants 动态生成 + 状态：报名中/已结束)

        /// <summary>
        /// 获取单个活动：elderly_participants 为动态名单
        /// - 报名中：已报名名单
        /// - 已结束：已参加名单
        /// </summary>
        public async Task<ActivitySchedule?> GetAsync(int id, IDbTransaction? tx = null)
        {
            var sql = $@"
                SELECT
                  v.activity_id,
                  v.activity_name,
                  v.activity_date,
                  TO_CHAR(v.activity_time, 'HH24:MI:SS.FF6') AS activity_time,
                  v.location,
                  v.staff_id,
                  NVL((
                    SELECT LISTAGG(e.NAME, ',') WITHIN GROUP (ORDER BY e.NAME)
                    FROM {Tables.ACTIVITY_PARTICIPATION} p
                    JOIN {Tables.ELDERLY_INFO} e
                      ON e.ELDERLY_ID = p.elderly_id
                   WHERE p.activity_id = v.activity_id
                     AND TRIM(p.status) = CASE WHEN v.status = '报名中' THEN '已报名' ELSE '已参加' END
                  ), '') AS elderly_participants,
                  v.activity_description,
                  v.status
                FROM (
                  SELECT
                    t.*,
                    CASE
                      WHEN t.activity_date + t.activity_time > (SYSTIMESTAMP AT LOCAL)
                        THEN '报名中'
                      ELSE '已结束'
                    END AS status
                  FROM {Tables.ACTIVITY_SCHEDULE} t
                ) v
                WHERE v.activity_id = :id";

            if (tx != null)
            {
                var c = tx.Connection ?? throw new InvalidOperationException("Transaction has no connection.");
                return await c.QuerySingleOrDefaultAsync<ActivitySchedule>(sql, new { id }, tx);
            }

            using var conn = _factory.Create();
            return await conn.QuerySingleOrDefaultAsync<ActivitySchedule>(sql, new { id });
        }

        /// <summary>
        /// 分页查询活动：elderly_participants 同样为动态名单；并返回 status（报名中/已结束）
        /// </summary>
        public async Task<IReadOnlyList<ActivitySchedule>> QueryAsync(DateTime? from, DateTime? to, int page, int pageSize)
        {
            using var conn = _factory.Create();
            var s = (page - 1) * pageSize + 1;
            var e = page * pageSize;

            var sql = $@"
                SELECT * FROM (
                  SELECT
                    v.activity_id,
                    v.activity_name,
                    v.activity_date,
                    TO_CHAR(v.activity_time, 'HH24:MI:SS.FF6') AS activity_time,
                    v.location,
                    v.staff_id,
                    NVL((
                      SELECT LISTAGG(e.NAME, ',') WITHIN GROUP (ORDER BY e.NAME)
                      FROM {Tables.ACTIVITY_PARTICIPATION} p
                      JOIN {Tables.ELDERLY_INFO} e
                        ON e.ELDERLY_ID = p.elderly_id
                     WHERE p.activity_id = v.activity_id
                       AND TRIM(p.status) = CASE WHEN v.status = '报名中' THEN '已报名' ELSE '已参加' END
                    ), '') AS elderly_participants,
                    v.activity_description,
                    v.status,
                    ROW_NUMBER() OVER (ORDER BY v.activity_date DESC, v.activity_time DESC) rn
                  FROM (
                    SELECT
                      t.*,
                      CASE
                        WHEN t.activity_date + t.activity_time > (SYSTIMESTAMP AT LOCAL)
                          THEN '报名中'
                        ELSE '已结束'
                      END AS status
                    FROM {Tables.ACTIVITY_SCHEDULE} t
                    WHERE (:fromAt IS NULL OR t.activity_date >= :fromAt)
                      AND (:toAt   IS NULL OR t.activity_date <= :toAt)
                  ) v
                )
                WHERE rn BETWEEN :s AND :e";

            var list = await conn.QueryAsync<ActivitySchedule>(sql, new
            {
                fromAt = from,
                toAt = to,
                s,
                e
            });

            return list.ToList();
        }

        #endregion

        #region Delete

        public Task DeleteAsync(int id, IDbTransaction tx)
        {
            var conn = tx.Connection ?? throw new InvalidOperationException("Transaction has no connection.");
            var sql = $"DELETE FROM {Tables.ACTIVITY_SCHEDULE} WHERE activity_id = :id";
            return conn.ExecuteAsync(sql, new { id }, tx);
        }

        #endregion
    }
}
