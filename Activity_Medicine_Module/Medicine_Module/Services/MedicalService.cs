using System.Data;
using Dapper;
using ElderCare.Api.Infrastructure;

namespace ElderCare.Api.Modules.Medical
{
    public sealed class MedicalService : IMedicalService
    {
        private readonly IDbConnectionFactory _factory;
        private readonly StockPriceRepository _stockRepo;
        private readonly ProcurementRepository _procRepo;

        public MedicalService(
            IDbConnectionFactory factory,
            StockPriceRepository stockRepo,
            ProcurementRepository procRepo
        )
        {
            _factory = factory;
            _stockRepo = stockRepo;
            _procRepo = procRepo;
        }

        public async Task AutoProcureIfBelowThresholdAsync(int medicine_id, int staff_id)
        {
            using var conn = _factory.Create();
            if (conn.State != ConnectionState.Open) conn.Open();
            using var tx = conn.BeginTransaction();
            try
            {
                await AutoProcureIfBelowThresholdAsync(medicine_id, staff_id, tx);
                tx.Commit();
            }
            catch
            {
                try { tx.Rollback(); } catch { }
                throw;
            }
        }

        public async Task AutoProcureIfBelowThresholdAsync(int medicine_id, int staff_id, IDbTransaction tx)
        {
            // 1) 取单个聚合
            var ag = await _stockRepo.GetAggregateSingleAsync(medicine_id, tx);
            var currentAvailable = ag?.available_quantity ?? 0;

            // 2) 取“阈值”（补货线）
            var threshold = await _stockRepo.GetReorderThresholdAsync(medicine_id, tx);

            if (threshold <= 0) return;               // 未设置阈值则直接跳过
            if (currentAvailable >= threshold) return; // 库存未低于阈值

            // 3) 低于阈值 -> 生成采购：目标补到 2 * threshold，至少买一个 threshold
            var target = threshold * 2;
            var qtyToBuy = Math.Max(target - currentAvailable, threshold);

            await _procRepo.CreateAsync(medicine_id, qtyToBuy, staff_id, tx);
        }

        public Task<int> AddMedicineAsync(CreateMedicineDto dto)
            => throw new NotImplementedException();

        public Task UpdateMedicineAsync(int id, UpdateMedicineDto dto)
            => throw new NotImplementedException();

        public Task<MedicineInventory?> GetMedicineAsync(int id)
            => throw new NotImplementedException();

        public Task<IReadOnlyList<MedicineInventory>> SearchMedicinesAsync(string? keyword, int page, int pageSize)
            => throw new NotImplementedException();

        public Task<int> CreateOrderAsync(CreateMedicalOrderDto dto)
            => throw new NotImplementedException();

        public Task UpdateOrderAsync(int id, UpdateMedicalOrderDto dto)
            => throw new NotImplementedException();

        public Task<MedicalOrder?> GetOrderAsync(int id)
            => throw new NotImplementedException();

        public Task<IReadOnlyList<MedicalOrder>> QueryOrdersAsync(int? elderly_id, DateTime? from, DateTime? to, int page, int pageSize)
            => throw new NotImplementedException();

        public Task<int> AddReminderAsync(CreateReminderDto dto)
            => throw new NotImplementedException();

        public Task<IReadOnlyList<VoiceAssistantReminder>> GetRemindersByOrderAsync(int order_id)
            => throw new NotImplementedException();

        public Task<int> UpsertThresholdAsync(ThresholdUpsertDto dto)
            => throw new NotImplementedException();

        public Task<IReadOnlyList<HealthThreshold>> GetThresholdsAsync(int elderly_id)
            => throw new NotImplementedException();

        public Task<int?> CheckAndAlertAsync(HealthDataSampleDto dto)
            => throw new NotImplementedException();

        public Task<IReadOnlyList<HealthAlert>> QueryAlertsAsync(int? elderly_id, string? type, DateTime? from, DateTime? to, int page, int pageSize)
            => throw new NotImplementedException();

        public Task ExecuteMedicationAsync(int order_id, ExecuteMedicationDto dto)
            => throw new NotImplementedException();
    }
}
