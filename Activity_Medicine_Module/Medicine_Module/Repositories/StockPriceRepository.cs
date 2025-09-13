using System.Data;
using Dapper;
using ElderCare.Api.Infrastructure;

namespace ElderCare.Api.Modules.Medical
{
    public sealed class StockPriceRepository
    {
        private readonly IDbConnectionFactory _factory;
        public StockPriceRepository(IDbConnectionFactory f) => _factory = f;

        private const string TABLE = "MEDICINESTOCKPRICE";

        // ========== 聚合：列表（/api/medical/stock/aggregates 用到） ==========
        public async Task<IReadOnlyList<StockAggregate>> GetAggregatesAsync(int? medicine_id = null, int? activeOnly = 1, IDbTransaction? tx = null)
        {
            const string SQL = @"
SELECT
    MEDICINE_ID                               AS medicine_id,
    NVL(SUM(QUANTITY_IN_STOCK), 0)            AS total_quantity,
    NVL(SUM(RESERVED_QUANTITY), 0)            AS reserved_quantity,
    NVL(SUM(QUANTITY_IN_STOCK), 0) - NVL(SUM(RESERVED_QUANTITY), 0) AS available_quantity,
    COUNT(*)                                  AS active_batches
FROM MEDICINESTOCKPRICE
WHERE (:mid IS NULL OR MEDICINE_ID = :mid)
  AND (:active IS NULL OR IS_ACTIVE = :active)
GROUP BY MEDICINE_ID
ORDER BY MEDICINE_ID";

            if (tx != null)
            {
                var rows = await tx.Connection!.QueryAsync<StockAggregate>(SQL, new { mid = medicine_id, active = activeOnly }, tx);
                return rows.ToList();
            }
            using var conn = _factory.Create();
            var list = await conn.QueryAsync<StockAggregate>(SQL, new { mid = medicine_id, active = activeOnly });
            return list.ToList();
        }

        private static void EnsureOpen(IDbConnection conn)
        {
            if (conn.State != ConnectionState.Open) conn.Open();
        }

        // ------------------------- CRUD & Query -------------------------

        public async Task<int> CreateAsync(CreateStockBatchDto dto, IDbTransaction? tx = null)
        {
            // 1) 先在当前事务中生成主键（表不是自增，所以应用层生成）
            string GET_ID_SQL = $"SELECT NVL(MAX(STOCK_BATCH_ID), 0) + 1 FROM {TABLE}";
            int newId;

            if (tx != null)
            {
                newId = await tx.Connection!.ExecuteScalarAsync<int>(GET_ID_SQL, transaction: tx);
            }
            else
            {
                using var conn = _factory.Create();
                conn.Open();
                using var localTx = conn.BeginTransaction();
                try
                {
                    newId = await conn.ExecuteScalarAsync<int>(GET_ID_SQL, transaction: localTx);

                    // 2) 带上主键插入
                    const string SQL = @"
INSERT INTO {0}
  (STOCK_BATCH_ID, MEDICINE_ID, BATCH_NO, EXPIRATION_DATE, COST_PRICE, SALE_PRICE,
   QUANTITY_IN_STOCK, RESERVED_QUANTITY, MIN_STOCK_LEVEL, LOCATION, SUPPLIER,
   IS_ACTIVE, CREATED_AT, UPDATED_AT)
VALUES
  (:id, :medicine_id, :batch_no, :expiration_date, :cost_price, :sale_price,
   :quantity_in_stock, 0, :minimum_stock_level, :location, :supplier,
   :is_active, SYSDATE, SYSDATE)";

                    var sql = string.Format(SQL, TABLE);

                    var p = new DynamicParameters();
                    p.Add("id", newId);
                    p.Add("medicine_id", dto.medicine_id, DbType.Int32);
                    p.Add("batch_no", dto.batch_no);
                    p.Add("expiration_date", dto.expiration_date, DbType.Date);
                    p.Add("cost_price", dto.cost_price, DbType.Decimal);
                    p.Add("sale_price", dto.sale_price, DbType.Decimal);
                    p.Add("quantity_in_stock", dto.quantity_in_stock, DbType.Int32);
                    p.Add("minimum_stock_level", dto.minimum_stock_level, DbType.Int32);
                    p.Add("location", dto.location);
                    p.Add("supplier", dto.supplier);
                    p.Add("is_active", dto.is_active, DbType.Int32);

                    await conn.ExecuteAsync(sql, p, localTx);
                    localTx.Commit();
                    return newId;
                }
                catch
                {
                    try { localTx.Rollback(); } catch { }
                    throw;
                }
            }

            {
                const string SQL = @"
INSERT INTO {0}
  (STOCK_BATCH_ID, MEDICINE_ID, BATCH_NO, EXPIRATION_DATE, COST_PRICE, SALE_PRICE,
   QUANTITY_IN_STOCK, RESERVED_QUANTITY, MIN_STOCK_LEVEL, LOCATION, SUPPLIER,
   IS_ACTIVE, CREATED_AT, UPDATED_AT)
VALUES
  (:id, :medicine_id, :batch_no, :expiration_date, :cost_price, :sale_price,
   :quantity_in_stock, 0, :minimum_stock_level, :location, :supplier,
   :is_active, SYSDATE, SYSDATE)";
                var sql = string.Format(SQL, TABLE);

                var p = new DynamicParameters();
                p.Add("id", newId);
                p.Add("medicine_id", dto.medicine_id, DbType.Int32);
                p.Add("batch_no", dto.batch_no);
                p.Add("expiration_date", dto.expiration_date, DbType.Date);
                p.Add("cost_price", dto.cost_price, DbType.Decimal);
                p.Add("sale_price", dto.sale_price, DbType.Decimal);
                p.Add("quantity_in_stock", dto.quantity_in_stock, DbType.Int32);
                p.Add("minimum_stock_level", dto.minimum_stock_level, DbType.Int32);
                p.Add("location", dto.location);
                p.Add("supplier", dto.supplier);
                p.Add("is_active", dto.is_active, DbType.Int32);

                await tx.Connection!.ExecuteAsync(sql, p, tx);
                return newId;
            }
        }

        public async Task<StockBatch?> GetAsync(int stock_batch_id)
        {
            string sql = $@"
SELECT 
   STOCK_BATCH_ID        AS stock_batch_id,
   MEDICINE_ID           AS medicine_id,
   BATCH_NO              AS batch_no,
   EXPIRATION_DATE       AS expiration_date,
   COST_PRICE            AS cost_price,
   SALE_PRICE            AS sale_price,
   QUANTITY_IN_STOCK     AS quantity_in_stock,
   RESERVED_QUANTITY     AS reserved_quantity,
   MIN_STOCK_LEVEL       AS minimum_stock_level,
   LOCATION              AS location,
   SUPPLIER              AS supplier,
   IS_ACTIVE             AS is_active,
   CREATED_AT            AS created_at,
   UPDATED_AT            AS updated_at
FROM {TABLE}
WHERE STOCK_BATCH_ID = :id";

            using var conn = _factory.Create();
            EnsureOpen(conn);
            return await conn.QuerySingleOrDefaultAsync<StockBatch>(sql, new { id = stock_batch_id });
        }

        public async Task<IReadOnlyList<StockBatch>> QueryAsync(int? medicine_id, int? activeOnly, string? kw, int page, int pageSize)
        {
            var start = (page - 1) * pageSize + 1;
            var end = page * pageSize;

            string sql = $@"
SELECT * FROM (
  SELECT t.*,
         ROW_NUMBER() OVER (ORDER BY t.CREATED_AT DESC, t.STOCK_BATCH_ID DESC) rn
  FROM {TABLE} t
  WHERE (:mid IS NULL OR t.MEDICINE_ID = :mid)
    AND (:active IS NULL OR t.IS_ACTIVE = :active)
    AND (
         :kw IS NULL 
         OR UPPER(NVL(t.BATCH_NO, ''))   LIKE UPPER('%' || :kw || '%')
         OR UPPER(NVL(t.SUPPLIER, ''))   LIKE UPPER('%' || :kw || '%')
         OR UPPER(NVL(t.LOCATION, ''))   LIKE UPPER('%' || :kw || '%')
    )
)
WHERE rn BETWEEN :s AND :e";

            using var conn = _factory.Create();
            EnsureOpen(conn);
            var list = await conn.QueryAsync<StockBatch>(sql, new
            {
                mid = medicine_id,
                active = activeOnly,
                kw,
                s = start,
                e = end
            });
            return list.ToList();
        }

        public Task<int> UpdateAsync(int stock_batch_id, UpdateStockBatchDto dto, IDbTransaction? tx = null)
        {
            string sql = $@"
UPDATE {TABLE}
   SET BATCH_NO = :batch_no,
       EXPIRATION_DATE = :expiration_date,
       COST_PRICE = :cost_price,
       SALE_PRICE = :sale_price,
       QUANTITY_IN_STOCK = :quantity_in_stock,
       MIN_STOCK_LEVEL = :minimum_stock_level,
       LOCATION = :location,
       SUPPLIER = :supplier,
       IS_ACTIVE = :is_active,
       UPDATED_AT = SYSDATE
 WHERE STOCK_BATCH_ID = :id";

            var param = new
            {
                id = stock_batch_id,
                dto.batch_no,
                dto.expiration_date,
                dto.cost_price,
                dto.sale_price,
                dto.quantity_in_stock,
                minimum_stock_level = dto.minimum_stock_level,
                dto.location,
                dto.supplier,
                dto.is_active
            };

            if (tx != null)
                return tx.Connection!.ExecuteAsync(sql, param, tx);

            var conn = _factory.Create();
            EnsureOpen(conn);
            return conn.ExecuteAsync(sql, param);
        }

        public async Task<bool> AdjustQuantityAsync(int stock_batch_id, int delta, IDbTransaction? tx = null)
        {
            const string GET_SQL = $"SELECT QUANTITY_IN_STOCK FROM {TABLE} WHERE STOCK_BATCH_ID = :id FOR UPDATE";
            const string UPD_SQL = $"UPDATE {TABLE} SET QUANTITY_IN_STOCK = :q, UPDATED_AT = SYSDATE WHERE STOCK_BATCH_ID = :id";

            if (tx != null)
            {
                var current = await tx.Connection!.ExecuteScalarAsync<int?>(GET_SQL, new { id = stock_batch_id }, tx);
                if (current is null) return false;

                var q = current.Value + delta;
                if (q < 0) return false;

                await tx.Connection!.ExecuteAsync(UPD_SQL, new { id = stock_batch_id, q }, tx);
                return true;
            }

            using var conn = _factory.Create();
            EnsureOpen(conn);                           
            using var localTx = conn.BeginTransaction();
            try
            {
                var current = await conn.ExecuteScalarAsync<int?>(GET_SQL, new { id = stock_batch_id }, localTx);
                if (current is null) { localTx.Rollback(); return false; }

                var q = current.Value + delta;
                if (q < 0) { localTx.Rollback(); return false; }

                await conn.ExecuteAsync(UPD_SQL, new { id = stock_batch_id, q }, localTx);
                localTx.Commit();
                return true;
            }
            catch
            {
                try { localTx.Rollback(); } catch { }
                throw;
            }
        }

        public Task<int> DeleteAsync(int stock_batch_id, IDbTransaction? tx = null)
        {
            string sql = $"UPDATE {TABLE} SET IS_ACTIVE = 0, UPDATED_AT = SYSDATE WHERE STOCK_BATCH_ID = :id";
            if (tx != null)
                return tx.Connection!.ExecuteAsync(sql, new { id = stock_batch_id }, tx);

            var conn = _factory.Create();
            EnsureOpen(conn);
            return conn.ExecuteAsync(sql, new { id = stock_batch_id });
        }


        public async Task<StockAggregate?> GetAggregateSingleAsync(int medicine_id, IDbTransaction tx)
        {
            const string sql = $@"
SELECT
    MEDICINE_ID                         AS medicine_id,
    NVL(SUM(QUANTITY_IN_STOCK), 0)      AS total_quantity,
    NVL(SUM(RESERVED_QUANTITY), 0)      AS reserved_quantity,
    NVL(SUM(QUANTITY_IN_STOCK), 0) 
      - NVL(SUM(RESERVED_QUANTITY), 0)  AS available_quantity,
    COUNT(*)                            AS active_batches
FROM {TABLE}
WHERE MEDICINE_ID = :mid AND IS_ACTIVE = 1
GROUP BY MEDICINE_ID";
            var list = await tx.Connection!.QueryAsync<StockAggregate>(sql, new { mid = medicine_id }, tx);
            return list.FirstOrDefault();
        }

        public async Task<StockAggregate?> GetAggregateSingleAsync(int medicine_id)
        {
            const string sql = $@"
SELECT
    MEDICINE_ID                         AS medicine_id,
    NVL(SUM(QUANTITY_IN_STOCK), 0)      AS total_quantity,
    NVL(SUM(RESERVED_QUANTITY), 0)      AS reserved_quantity,
    NVL(SUM(QUANTITY_IN_STOCK), 0) 
      - NVL(SUM(RESERVED_QUANTITY), 0)  AS available_quantity,
    COUNT(*)                            AS active_batches
FROM {TABLE}
WHERE MEDICINE_ID = :mid AND IS_ACTIVE = 1
GROUP BY MEDICINE_ID";

            using var conn = _factory.Create();
            EnsureOpen(conn);
            var list = await conn.QueryAsync<StockAggregate>(sql, new { mid = medicine_id });
            return list.FirstOrDefault();
        }

        public Task<int> GetReorderThresholdAsync(int medicine_id, IDbTransaction tx)
        {
            string sql = $@"SELECT NVL(MAX(MIN_STOCK_LEVEL), 0)
                    FROM {TABLE}
                    WHERE MEDICINE_ID = :mid AND IS_ACTIVE = 1";
            return tx.Connection!.ExecuteScalarAsync<int>(sql, new { mid = medicine_id }, tx);
        }

        public async Task<int> GetReorderThresholdAsync(int medicine_id)
        {
            string sql = $@"SELECT NVL(MAX(MIN_STOCK_LEVEL), 0)
                    FROM {TABLE}
                    WHERE MEDICINE_ID = :mid AND IS_ACTIVE = 1";
            using var conn = _factory.Create();
            EnsureOpen(conn);
            return await conn.ExecuteScalarAsync<int>(sql, new { mid = medicine_id });
        }
        // 批量汇总：返回所有药品的聚合情况
        public async Task<IReadOnlyList<StockAggregate>> GetAggregatesAsync(int? medicine_id = null, int? activeOnly = 1)
        {
            string sql = $@"
SELECT
    MEDICINE_ID                         AS medicine_id,
    NVL(SUM(QUANTITY_IN_STOCK), 0)      AS total_quantity,
    NVL(SUM(RESERVED_QUANTITY), 0)      AS reserved_quantity,
    NVL(SUM(QUANTITY_IN_STOCK), 0) 
      - NVL(SUM(RESERVED_QUANTITY), 0)  AS available_quantity,
    COUNT(*)                            AS active_batches
FROM {TABLE}
WHERE (:mid IS NULL OR MEDICINE_ID = :mid)
  AND (:active IS NULL OR IS_ACTIVE = :active)
GROUP BY MEDICINE_ID
ORDER BY MEDICINE_ID";

            using var conn = _factory.Create();
            if (conn.State != ConnectionState.Open) conn.Open();

            var list = await conn.QueryAsync<StockAggregate>(sql, new
            {
                mid = medicine_id,
                active = activeOnly
            });
            return list.ToList();
        }

        // =========== FIFO 预占 ===========
        // 返回：第一个用到的批次id、加权平均售价、总金额
        public async Task<(int firstBatchId, decimal avgPrice, decimal totalAmount)> ReserveAcrossBatchesAsync(
            int medicine_id, int needQty, IDbTransaction tx)
        {
            const string Q_SQL = @"
SELECT STOCK_BATCH_ID AS Stock_Batch_Id,
       EXPIRATION_DATE,
       SALE_PRICE,
       QUANTITY_IN_STOCK,
       RESERVED_QUANTITY
FROM MEDICINESTOCKPRICE
WHERE MEDICINE_ID = :mid AND IS_ACTIVE = 1
ORDER BY EXPIRATION_DATE ASC, STOCK_BATCH_ID ASC";

            var rows = (await tx.Connection!.QueryAsync<dynamic>(Q_SQL, new { mid = medicine_id }, tx)).ToList();
            if (!rows.Any()) throw new InvalidOperationException("不存在可用批次");

            int remain = needQty;
            int? firstBatch = null;
            decimal amount = 0m;
            decimal amountForAvg = 0m;
            int qtyForAvg = 0;

            foreach (var r in rows)
            {
                int batchId = (int)r.STOCK_BATCH_ID;
                int stock = (int)r.QUANTITY_IN_STOCK;
                int reserved = (int)r.RESERVED_QUANTITY;
                int available = stock - reserved;
                if (available <= 0) continue;

                int take = Math.Min(available, remain);
                if (take <= 0) continue;

                const string U_SQL = @"UPDATE MEDICINESTOCKPRICE
                                       SET RESERVED_QUANTITY = RESERVED_QUANTITY + :take,
                                           UPDATED_AT = SYSDATE
                                       WHERE STOCK_BATCH_ID = :id";
                await tx.Connection.ExecuteAsync(U_SQL, new { take, id = batchId }, tx);

                if (firstBatch is null) firstBatch = batchId;

                decimal price = (decimal)r.SALE_PRICE;
                amount += price * take;

                amountForAvg += price * take;
                qtyForAvg += take;

                remain -= take;
                if (remain == 0) break;
            }

            if (remain > 0) throw new InvalidOperationException("库存可用量不足，无法完成预占");

            var avg = qtyForAvg == 0 ? 0m : Math.Round(amountForAvg / qtyForAvg, 2);
            return (firstBatch ?? 0, avg, amount);
        }

        // =========== FIFO 确认（扣实库存 & 回退预占） ===========
        public async Task ConfirmAcrossBatchesAsync(int medicine_id, int qty, IDbTransaction tx)
        {
            const string Q_SQL = @"
SELECT STOCK_BATCH_ID AS Id, QUANTITY_IN_STOCK AS STOCK, RESERVED_QUANTITY AS RESERVED
FROM MEDICINESTOCKPRICE
WHERE MEDICINE_ID = :mid AND IS_ACTIVE = 1
ORDER BY EXPIRATION_DATE ASC, STOCK_BATCH_ID ASC";

            var rows = (await tx.Connection!.QueryAsync<dynamic>(Q_SQL, new { mid = medicine_id }, tx)).ToList();
            if (!rows.Any()) throw new InvalidOperationException("不存在可用批次");

            int remain = qty;
            foreach (var r in rows)
            {
                int id = (int)r.ID;
                int stock = (int)r.STOCK;
                int reserved = (int)r.RESERVED;

                int maxTake = Math.Min(reserved, stock); // 安全：不能扣成负数
                if (maxTake <= 0) continue;

                int take = Math.Min(maxTake, remain);
                if (take <= 0) continue;

                const string U_SQL = @"UPDATE MEDICINESTOCKPRICE
                                       SET QUANTITY_IN_STOCK = QUANTITY_IN_STOCK - :take,
                                           RESERVED_QUANTITY = RESERVED_QUANTITY - :take,
                                           UPDATED_AT = SYSDATE
                                       WHERE STOCK_BATCH_ID = :id";
                await tx.Connection.ExecuteAsync(U_SQL, new { take, id }, tx);

                remain -= take;
                if (remain == 0) break;
            }

            if (remain > 0) throw new InvalidOperationException("预占量不足，无法确认发药");
        }

        // =========== FIFO 回退预占 ===========
        public async Task ReleaseReserveAcrossBatchesAsync(int medicine_id, int qty, IDbTransaction tx)
        {
            const string Q_SQL = @"
SELECT STOCK_BATCH_ID AS Id, RESERVED_QUANTITY AS RESERVED
FROM MEDICINESTOCKPRICE
WHERE MEDICINE_ID = :mid AND IS_ACTIVE = 1
ORDER BY EXPIRATION_DATE ASC, STOCK_BATCH_ID ASC";

            var rows = (await tx.Connection!.QueryAsync<dynamic>(Q_SQL, new { mid = medicine_id }, tx)).ToList();
            if (!rows.Any()) return;

            int remain = qty;
            foreach (var r in rows)
            {
                int id = (int)r.ID;
                int reserved = (int)r.RESERVED;
                if (reserved <= 0) continue;

                int take = Math.Min(reserved, remain);
                if (take <= 0) continue;

                const string U_SQL = @"UPDATE MEDICINESTOCKPRICE
                                       SET RESERVED_QUANTITY = RESERVED_QUANTITY - :take,
                                           UPDATED_AT = SYSDATE
                                       WHERE STOCK_BATCH_ID = :id";
                await tx.Connection.ExecuteAsync(U_SQL, new { take, id }, tx);

                remain -= take;
                if (remain == 0) break;
            }

            if (remain > 0) throw new InvalidOperationException("预占量不足，无法取消发药");
        }
    }
}

