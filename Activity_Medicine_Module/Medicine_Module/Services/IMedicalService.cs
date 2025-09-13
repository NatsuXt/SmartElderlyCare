namespace ElderCare.Api.Modules.Medical
{
    public interface IMedicalService
    {
        Task<int> CreateOrderAsync(CreateMedicalOrderDto dto);
        Task UpdateOrderAsync(int order_id, UpdateMedicalOrderDto dto);
        Task<MedicalOrder?> GetOrderAsync(int order_id);
        Task<IReadOnlyList<MedicalOrder>> QueryOrdersAsync(int? elderly_id, DateTime? from, DateTime? to, int page, int pageSize);
        Task ExecuteMedicationAsync(int order_id, ExecuteMedicationDto exec);

        Task<int> AddMedicineAsync(CreateMedicineDto dto);
        Task UpdateMedicineAsync(int id, UpdateMedicineDto dto);
        Task<MedicineInventory?> GetMedicineAsync(int id);
        Task<IReadOnlyList<MedicineInventory>> SearchMedicinesAsync(string? kw, int page, int pageSize);

        Task<int> AddReminderAsync(CreateReminderDto dto);
        Task<IReadOnlyList<VoiceAssistantReminder>> GetRemindersByOrderAsync(int order_id);

        Task<int> UpsertThresholdAsync(ThresholdUpsertDto dto);
        Task<IReadOnlyList<HealthThreshold>> GetThresholdsAsync(int elderly_id);
        Task<int?> CheckAndAlertAsync(HealthDataSampleDto dto);

        Task<IReadOnlyList<HealthAlert>> QueryAlertsAsync(int? elderly_id, string? status, DateTime? from, DateTime? to, int page, int pageSize);
    }
}
