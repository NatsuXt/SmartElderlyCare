using System.Data;
using Dapper;
using ElderCare.Api.Infrastructure;

namespace ElderCare.Api.Modules.Medical
{
    /// <summary>
    /// 仅负责 MEDICINEINVENTORY 的查询（分页 + 关键字）
    /// </summary>
    public sealed class MedicinesRepository
    {
        private readonly IDbConnectionFactory _factory;
        public MedicinesRepository(IDbConnectionFactory factory) => _factory = factory;

        public async Task<IReadOnlyList<MedicineInventory>> SearchAsync(string? kw, int page, int pageSize)
        {
            using var conn = _factory.Create();

            // 分页边界
            var start = (page <= 0 ? 1 : page - 1) * (pageSize <= 0 ? 20 : pageSize) + 1;
            var end = (page <= 0 ? 1 : page) * (pageSize <= 0 ? 20 : pageSize);

            var sql = $@"
SELECT * FROM (
    SELECT t.*, ROW_NUMBER() OVER (ORDER BY t.MEDICINE_ID DESC) AS rn
    FROM {Tables.MEDICINE_INVENTORY} t
    WHERE (:p_kw1 IS NULL
           OR INSTR(UPPER(t.MEDICINE_NAME), UPPER(:p_kw1)) > 0
           OR INSTR(UPPER(t.SPECIFICATION), UPPER(:p_kw2)) > 0)
)
WHERE rn BETWEEN :p_start AND :p_end";

            var rows = await conn.QueryAsync<MedicineInventory>(sql, new
            {
                p_kw1 = kw,
                p_kw2 = kw,    
                p_start = start,
                p_end = end
            });

            return rows.ToList();
        }

        public async Task<MedicineInventory?> GetByIdAsync(int id)
        {
            using var conn = _factory.Create();
            var sql = $"SELECT * FROM {Tables.MEDICINE_INVENTORY} WHERE MEDICINE_ID = :p_id";
            return await conn.QuerySingleOrDefaultAsync<MedicineInventory>(sql, new { p_id = id });
        }
    }
}
