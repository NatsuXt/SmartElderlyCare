using System.Data;

namespace ElderCare.Api.Modules.Medical
{
    public interface IProcurementRepository
    {
        Task<int> CreateAsync(int medicine_id, int qty, int staff_id, IDbTransaction tx);
        Task ReceiveAsync(int procurement_id, IDbTransaction tx);
        Task<IReadOnlyList<MedicineProcurement>> ListAsync(string? status, int page, int pageSize);
    }

    public interface IReminderRepository
    {
        Task<int> CreateAsync(VoiceAssistantReminder r, IDbTransaction tx);
        Task UpdateStatusAsync(int reminder_id, string status, IDbTransaction tx);
        Task<IReadOnlyList<VoiceAssistantReminder>> ListByOrderAsync(int order_id);
    }

    public interface IThresholdsRepository
    {
        Task<IReadOnlyList<HealthThreshold>> GetByElderlyAsync(int elderly_id);
        Task<int> UpsertAsync(ThresholdUpsertDto dto, IDbTransaction tx);
    }

    public interface IAlertsRepository
    {
        Task<int> CreateAsync(HealthAlert a, IDbTransaction tx);
        Task UpdateStatusAsync(int alert_id, string status, string? notes, IDbTransaction tx);
        Task<IReadOnlyList<HealthAlert>> QueryAsync(int? elderly_id, string? status, DateTime? from, DateTime? to, int page, int pageSize);
    }
}
