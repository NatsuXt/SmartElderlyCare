using System.Data;
using Dapper;
using ElderCare.Api.Infrastructure;

namespace ElderCare.Api.Modules.Medical
{
    /// <summary>
    /// 医嘱仓储实现：
    /// </summary>
    public sealed class MedicalOrdersRepository : IMedicalOrdersRepository
    {
        private readonly IDbConnectionFactory _factory;
        public MedicalOrdersRepository(IDbConnectionFactory f) => _factory = f;

        public async Task<int> CreateAsync(MedicalOrder o, IDbTransaction tx)
        {
            if (tx is null) throw new ArgumentNullException(nameof(tx));
            var conn = tx.Connection ?? throw new InvalidOperationException("Transaction has no connection.");

            var sql = $@"
                INSERT INTO {Tables.MEDICAL_ORDER}
                  (elderly_id, staff_id, medicine_id, order_date, dosage, frequency, duration)
                VALUES
                  (:elderly_id, :staff_id, :medicine_id, :order_date, :dosage, :frequency, :duration)
                RETURNING order_id INTO :new_id";

            var p = new DynamicParameters(new
            {
                o.elderly_id,
                o.staff_id,
                o.medicine_id,
                o.order_date,
                o.dosage,
                o.frequency,
                o.duration
            });
            p.Add("new_id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await conn.ExecuteAsync(sql, p, tx);
            return p.Get<int>("new_id");
        }

        public Task UpdateAsync(int order_id, UpdateMedicalOrderDto dto, IDbTransaction tx)
        {
            if (tx is null) throw new ArgumentNullException(nameof(tx));
            var conn = tx.Connection ?? throw new InvalidOperationException("Transaction has no connection.");

            var sql = $@"
                UPDATE {Tables.MEDICAL_ORDER}
                   SET dosage    = COALESCE(:dosage,    dosage),
                       frequency = COALESCE(:frequency, frequency),
                       duration  = COALESCE(:duration,  duration)
                 WHERE order_id = :order_id";

            return conn.ExecuteAsync(sql, new
            {
                order_id,
                dto.dosage,
                dto.frequency,
                dto.duration
            }, tx);
        }

        public async Task<int> DeleteAsync(int order_id, IDbTransaction tx)
        {
            if (tx is null) throw new ArgumentNullException(nameof(tx));
            var conn = tx.Connection ?? throw new InvalidOperationException("Transaction has no connection.");

            var sql = $@"DELETE FROM {Tables.MEDICAL_ORDER} WHERE order_id = :id";
            return await conn.ExecuteAsync(sql, new { id = order_id }, tx);
        }

        public async Task<MedicalOrder?> GetAsync(int order_id, IDbTransaction? tx = null)
        {
            var sql = $@"SELECT * FROM {Tables.MEDICAL_ORDER} WHERE order_id = :order_id";

            if (tx != null)
            {
                var c = tx.Connection ?? throw new InvalidOperationException("Transaction has no connection.");
                return await c.QuerySingleOrDefaultAsync<MedicalOrder>(sql, new { order_id }, tx);
            }

            using var conn = _factory.Create();
            if (conn.State != ConnectionState.Open) conn.Open();
            return await conn.QuerySingleOrDefaultAsync<MedicalOrder>(sql, new { order_id });
        }

        public async Task<IReadOnlyList<MedicalOrder>> QueryAsync(int? elderly_id, DateTime? from, DateTime? to, int page, int pageSize)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            var start = (page - 1) * pageSize + 1;
            var end = page * pageSize;

            var sql = $@"
                SELECT * FROM (
                    SELECT t.*,
                           ROW_NUMBER() OVER (ORDER BY t.order_date DESC, t.order_id DESC) rn
                      FROM {Tables.MEDICAL_ORDER} t
                     WHERE (:elderly_id IS NULL OR t.elderly_id = :elderly_id)
                       AND (:fromAt    IS NULL OR t.order_date >= :fromAt)
                       AND (:toAt      IS NULL OR t.order_date <= :toAt)
                )
                WHERE rn BETWEEN :s AND :e";

            using var conn = _factory.Create();
            if (conn.State != ConnectionState.Open) conn.Open();
            var list = await conn.QueryAsync<MedicalOrder>(sql, new
            {
                elderly_id,
                fromAt = from,
                toAt = to,
                s = start,
                e = end
            });

            return list.ToList();
        }
    }

    public static class MedicalOrdersRepositoryExtensions
    {
        public static Task<int> CreateAsync(
            this IMedicalOrdersRepository repo,
            CreateMedicalOrderDto dto,
            IDbTransaction tx)
        {
            if (repo is null) throw new ArgumentNullException(nameof(repo));
            if (dto is null) throw new ArgumentNullException(nameof(dto));

            var entity = new MedicalOrder
            {
                elderly_id = dto.elderly_id,
                staff_id = dto.staff_id,
                medicine_id = dto.medicine_id,
                order_date = dto.order_date,
                dosage = dto.dosage,
                frequency = dto.frequency,
                duration = dto.duration
            };

            return repo.CreateAsync(entity, tx);
        }
    }
}
