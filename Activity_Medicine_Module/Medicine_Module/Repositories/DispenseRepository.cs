using System.Data;
using Dapper;
using ElderCare.Api.Infrastructure;

namespace ElderCare.Api.Modules.Medical
{
    public sealed class DispenseRepository
    {
        private readonly IDbConnectionFactory _factory;
        private readonly StockPriceRepository _stockRepo;
        private readonly ProcurementRepository _procRepo;

        private const string TABLE = "MEDICINE_DISPENSE";

        private static void EnsureOpen(IDbConnection conn)
        {
            if (conn.State != ConnectionState.Open) conn.Open();
        }

        public DispenseRepository(
            IDbConnectionFactory factory,
            StockPriceRepository stockRepo,
            ProcurementRepository procRepo)
        {
            _factory = factory;
            _stockRepo = stockRepo;
            _procRepo = procRepo;
        }

        // ① 预占（创建发药单）
        public async Task<int> CreateReserveAsync(CreateDispenseDto dto)
        {
            using var conn = _factory.Create();
            EnsureOpen(conn);
            using var tx = conn.BeginTransaction();

            try
            {
                // 先按到期日 FIFO 预占，将多个批次的预占拆散并返回：
                // firstBatchId 仅用于满足表的 NOT NULL STOCK_BATCH_ID（当表头需要一个批次Id）
                var (firstBatchId, avgPrice, totalAmount) =
                    await _stockRepo.ReserveAcrossBatchesAsync(dto.Medicine_Id, dto.Quantity, tx);

                // 预占完成后，可用量下降，尝试触发自动补货
                await TryAutoProcureAsync(dto.Medicine_Id, dto.Staff_Id ?? 0, tx);

                // 生成账单号（若未传）
                string billId = string.IsNullOrWhiteSpace(dto.Bill_Id)
                    ? await conn.ExecuteScalarAsync<string>(
                        "SELECT SUBSTR(SYS_GUID(),1,20) FROM DUAL", transaction: tx
                      ) ?? Guid.NewGuid().ToString("N")[..20]
                    : dto.Bill_Id!;

                const string INS = @"
INSERT INTO MEDICINE_DISPENSE
  (BILL_ID,
   ELDERLY_ID,
   ORDER_ID,
   STAFF_ID,
   MEDICINE_ID,
   STOCK_BATCH_ID,
   QUANTITY,
   UNIT_SALE_PRICE,
   TOTAL_AMOUNT,
   PAYMENT_STATUS,
   PAYMENT_METHOD,
   SETTLEMENT_ID,
   DISPENSE_TIME,
   STATUS,
   REMARKS)
VALUES
  (:bill,
   :elder,
   :oid,
   :sid,
   :mid,
   :batch,
   :qty,
   :price,
   :amount,
   'UNPAID',           -- 新建默认未支付
   :payMethod,
   :settlementId,
   NULL,               -- 未确认发药，时间为空
   'RESERVED',         -- 新建默认状态：预占
   :remarks)
RETURNING DISPENSE_ID INTO :new_id";

                var p = new DynamicParameters();
                p.Add("bill", billId);
                p.Add("elder", dto.Elderly_Id);
                p.Add("oid", dto.Order_Id);
                p.Add("sid", dto.Staff_Id);
                p.Add("mid", dto.Medicine_Id);
                p.Add("batch", firstBatchId);      
                p.Add("qty", dto.Quantity);
                p.Add("price", avgPrice);           
                p.Add("amount", totalAmount);
                p.Add("payMethod", dto.Payment_Method);
                p.Add("settlementId", dto.Settlement_Id);  
                p.Add("remarks", dto.Remarks);        
                p.Add("new_id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await conn.ExecuteAsync(INS, p, tx);
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

        // ② 支付状态（只改 PAYMENT_STATUS）
        public async Task<int> UpdatePayStatusAsync(int dispense_id, string pay_status, IDbTransaction? tx = null)
        {
            const string sql = @"
UPDATE MEDICINE_DISPENSE
   SET PAYMENT_STATUS = :pay_status
 WHERE DISPENSE_ID    = :id";

            var param = new { id = dispense_id, pay_status };

            if (tx != null)
            {
                return await tx.Connection!.ExecuteAsync(sql, param, tx);
            }

            using var conn = _factory.Create();
            EnsureOpen(conn);
            return await conn.ExecuteAsync(sql, param);
        }

        // ③ 确认发药（扣实库存 + 释放预占）
        public async Task ConfirmAsync(int dispenseId)
        {
            using var conn = _factory.Create();
            EnsureOpen(conn);
            using var tx = conn.BeginTransaction();

            try
            {
                var head = await conn.QuerySingleOrDefaultAsync<dynamic>(@"
SELECT DISPENSE_ID, MEDICINE_ID, QUANTITY, PAYMENT_STATUS, DISPENSE_TIME
FROM MEDICINE_DISPENSE
WHERE DISPENSE_ID = :id
FOR UPDATE", new { id = dispenseId }, tx);

                if (head is null) throw new InvalidOperationException("发药单不存在");
                if (head.PAYMENT_STATUS != "PAID") throw new InvalidOperationException("未支付，不能确认发药");
                if (head.DISPENSE_TIME != null) throw new InvalidOperationException("该发药单已确认");

                int medicineId = (int)head.MEDICINE_ID;
                int qty = (int)head.QUANTITY;

                await _stockRepo.ConfirmAcrossBatchesAsync(medicineId, qty, tx);

                // 发药后总量下降，再次尝试自动补货
                await TryAutoProcureAsync(medicineId, 0, tx);

                await conn.ExecuteAsync(
                    "UPDATE MEDICINE_DISPENSE SET DISPENSE_TIME = SYSDATE, STATUS = 'DISPENSED' WHERE DISPENSE_ID = :id",
                    new { id = dispenseId }, tx);

                tx.Commit();
            }
            catch
            {
                try { tx.Rollback(); } catch { }
                throw;
            }
        }

        // ④ 取消发药（释放预占）
        public async Task CancelAsync(int dispenseId)
        {
            using var conn = _factory.Create();
            EnsureOpen(conn);
            using var tx = conn.BeginTransaction();

            try
            {
                var head = await conn.QuerySingleOrDefaultAsync<dynamic>(@"
SELECT DISPENSE_ID, MEDICINE_ID, QUANTITY, PAYMENT_STATUS, DISPENSE_TIME
FROM MEDICINE_DISPENSE
WHERE DISPENSE_ID = :id
FOR UPDATE", new { id = dispenseId }, tx);

                if (head is null) throw new InvalidOperationException("发药单不存在");
                if (head.DISPENSE_TIME != null) throw new InvalidOperationException("已发药，不能取消");

                int medicineId = (int)head.MEDICINE_ID;
                int qty = (int)head.QUANTITY;

                await _stockRepo.ReleaseReserveAcrossBatchesAsync(medicineId, qty, tx);

                await TryAutoProcureAsync(medicineId, 0, tx);

                await conn.ExecuteAsync(
                    "UPDATE MEDICINE_DISPENSE SET STATUS = 'CANCELLED' WHERE DISPENSE_ID = :id",
                    new { id = dispenseId }, tx);

                tx.Commit();
            }
            catch
            {
                try { tx.Rollback(); } catch { }
                throw;
            }
        }

        // ⑤ 列表
        public async Task<IReadOnlyList<DispenseRecord>> QueryAsync(
            int page, int pageSize, int? elderly_id, int? medicine_id,
            string? status, string? pay_status)
        {
            string statusFilter = status?.ToUpperInvariant() switch
            {
                "DISPENSED" => " AND DISPENSE_TIME IS NOT NULL ",
                "RESERVED" => " AND (STATUS = 'RESERVED' OR (STATUS IS NULL AND DISPENSE_TIME IS NULL)) ",
                "CANCELLED" => " AND STATUS = 'CANCELLED' ",
                _ => string.Empty
            };

            string payFilter = string.IsNullOrWhiteSpace(pay_status) ? "" : " AND PAYMENT_STATUS = :ps ";
            string elderFilter = elderly_id is null ? "" : " AND ELDERLY_ID = :elder ";
            string medFilter = medicine_id is null ? "" : " AND MEDICINE_ID = :mid ";

            int s = (page - 1) * pageSize + 1;
            int e = page * pageSize;

            string sql = $@"
SELECT * FROM (
  SELECT t.*,
         ROW_NUMBER() OVER (ORDER BY NVL(DISPENSE_TIME, DATE '1970-01-01') DESC, DISPENSE_ID DESC) rn
  FROM {TABLE} t
  WHERE 1=1 {statusFilter} {payFilter} {elderFilter} {medFilter}
) WHERE rn BETWEEN :s AND :e";

            using var conn = _factory.Create();
            EnsureOpen(conn);

            var list = await conn.QueryAsync<DispenseRecord>(sql, new
            {
                s,
                e,
                ps = pay_status,
                elder = elderly_id,
                mid = medicine_id
            });
            return list.ToList();
        }

        /// <summary>
        /// 分页查询（支持 elderly_id / medicine_id / status / pay_status / remarks）
        /// </summary>
        public async Task<IReadOnlyList<DispenseRecord>> ListAsync(
            int page, int pageSize,
            int? elderly_id = null,
            int? medicine_id = null,
            string? status = null,
            string? pay_status = null,
            string? remarksLike = null)
        {
            using var conn = _factory.Create();
            EnsureOpen(conn);

            var sql = $@"
SELECT
    DISPENSE_ID,
    BILL_ID,
    ELDERLY_ID,
    ORDER_ID,
    STAFF_ID,
    MEDICINE_ID,
    STOCK_BATCH_ID,
    QUANTITY,
    UNIT_SALE_PRICE,
    TOTAL_AMOUNT,
    PAYMENT_STATUS,
    PAYMENT_METHOD,
    SETTLEMENT_ID,
    DISPENSE_TIME,
    STATUS,
    REMARKS
FROM {TABLE}
WHERE 1=1";

            var p = new DynamicParameters();

            if (elderly_id.HasValue)
            {
                sql += " AND ELDERLY_ID = :elderly_id";
                p.Add("elderly_id", elderly_id);
            }
            if (medicine_id.HasValue)
            {
                sql += " AND MEDICINE_ID = :medicine_id";
                p.Add("medicine_id", medicine_id);
            }
            if (!string.IsNullOrWhiteSpace(status))
            {
                sql += " AND STATUS = :status";
                p.Add("status", status);
            }
            if (!string.IsNullOrWhiteSpace(pay_status))
            {
                sql += " AND PAYMENT_STATUS = :pay_status";
                p.Add("pay_status", pay_status);
            }
            if (!string.IsNullOrWhiteSpace(remarksLike))
            {
                sql += " AND REMARKS LIKE :rk";
                p.Add("rk", $"%{remarksLike}%");
            }

            sql += " ORDER BY DISPENSE_ID DESC OFFSET :skip ROWS FETCH NEXT :take ROWS ONLY";
            p.Add("skip", Math.Max(0, (page - 1) * pageSize));
            p.Add("take", pageSize);

            var rows = await conn.QueryAsync<DispenseRecord>(sql, p);
            return rows.ToList();
        }

        /// <summary>
        /// 单条查询
        /// </summary>
        public async Task<DispenseRecord?> GetAsync(int dispense_id)
        {
            using var conn = _factory.Create();
            EnsureOpen(conn);

            var sql = $@"
SELECT
    DISPENSE_ID,
    BILL_ID,
    ELDERLY_ID,
    ORDER_ID,
    STAFF_ID,
    MEDICINE_ID,
    STOCK_BATCH_ID,
    QUANTITY,
    UNIT_SALE_PRICE,
    TOTAL_AMOUNT,
    PAYMENT_STATUS,
    PAYMENT_METHOD,
    SETTLEMENT_ID,
    DISPENSE_TIME,
    STATUS,
    REMARKS
FROM {TABLE}
WHERE DISPENSE_ID = :id";

            return await conn.QuerySingleOrDefaultAsync<DispenseRecord>(sql, new { id = dispense_id });
        }

        // --- 自动补货：available < threshold 时插入“待采购” ---
        private async Task TryAutoProcureAsync(int medicine_id, int staff_id, IDbTransaction tx)
        {
            // 单药聚合（含 available_quantity）
            var agg = await _stockRepo.GetAggregateSingleAsync(medicine_id, tx);
            // 该药品在所有批次中的补货阈值（取每批 MIN_STOCK_LEVEL 的最大值）
            var threshold = await _stockRepo.GetReorderThresholdAsync(medicine_id, tx);

            if (agg is null || threshold <= 0) return;

            var available = agg.available_quantity;  
            if (available < threshold)
            {
                var target = threshold * 2;
                var need = Math.Max(target - available, threshold);
                await _procRepo.CreateAsync(medicine_id, need, staff_id, tx);
            }
        }
    }
}
