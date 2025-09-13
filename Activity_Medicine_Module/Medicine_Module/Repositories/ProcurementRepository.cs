using System.Data;
using Dapper;
using ElderCare.Api.Infrastructure;

namespace ElderCare.Api.Modules.Medical
{
    /// <summary>
    /// MEDICINEPROCUREMENT 采购单
    /// </summary>
    public sealed class ProcurementRepository
    {
        private readonly IDbConnectionFactory _factory;
        public ProcurementRepository(IDbConnectionFactory factory) => _factory = factory;

        private static string T(string name)
            => string.IsNullOrWhiteSpace(Tables.Schema) ? name : $"{Tables.Schema}.{name}";

        private string TABLE => T("MEDICINEPROCUREMENT");

        /// <summary>
        /// 新建采购单：status 强制为“待采购”，忽略外部传入
        /// </summary>
        public async Task<int> CreateAsync(int medicine_id, int qty, int staff_id, IDbTransaction? tx = null)
        {
            var p = new DynamicParameters();
            p.Add("mid", medicine_id);
            p.Add("qty", qty);
            p.Add("sid", staff_id);
            p.Add("status", "待采购");
            p.Add("new_id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            string sql = $@"
INSERT INTO {TABLE}
  (MEDICINE_ID, PURCHASE_QUANTITY, PURCHASE_TIME, STAFF_ID, STATUS)
VALUES
  (:mid, :qty, SYSDATE, :sid, :status)
RETURNING PROCUREMENT_ID INTO :new_id";

            if (tx != null)
            {
                await tx.Connection!.ExecuteAsync(sql, p, tx);
            }
            else
            {
                using var conn = _factory.Create();
                conn.Open();
                await conn.ExecuteAsync(sql, p);
            }

            return p.Get<int>("new_id");
        }

        /// <summary>
        /// 将采购单标记为“已入库”
        /// </summary>
        public async Task<int> ReceiveAsync(int procurement_id, IDbTransaction? tx = null)
        {
            string sql = $@"UPDATE {TABLE} SET STATUS='已入库' WHERE PROCUREMENT_ID=:id";
            if (tx != null)
                return await tx.Connection!.ExecuteAsync(sql, new { id = procurement_id }, tx);

            using var conn = _factory.Create();
            conn.Open();
            return await conn.ExecuteAsync(sql, new { id = procurement_id });
        }

        /// <summary>分页查询</summary>
        public async Task<IReadOnlyList<MedicineProcurement>> ListAsync(string? status, int page, int pageSize)
        {
            var start = (page - 1) * pageSize + 1;
            var end = page * pageSize;
            using var conn = _factory.Create();
            conn.Open();

            string sql = $@"
SELECT * FROM (
  SELECT t.*, ROW_NUMBER() OVER (ORDER BY t.PURCHASE_TIME DESC, t.PROCUREMENT_ID DESC) rn
    FROM {TABLE} t
   WHERE (:st IS NULL OR t.STATUS=:st)
)
WHERE rn BETWEEN :s AND :e";

            var list = await conn.QueryAsync<MedicineProcurement>(sql, new
            {
                st = status,
                s = start,
                e = end
            });

            return list.ToList();
        }
    }
}
