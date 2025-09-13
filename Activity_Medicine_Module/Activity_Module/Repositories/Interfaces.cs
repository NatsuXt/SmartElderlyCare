using System.Data;

namespace ElderCare.Api.Modules.Activities
{
    public interface IActivitiesRepository
    {
        Task<int> CreateAsync(ActivitySchedule a, IDbTransaction tx);
        Task UpdateAsync(int id, UpdateActivityDto dto, IDbTransaction tx);
        Task<ActivitySchedule?> GetAsync(int id, IDbTransaction? tx = null);
        Task<IReadOnlyList<ActivitySchedule>> QueryAsync(DateTime? from, DateTime? to, int page, int pageSize);
        Task DeleteAsync(int id, IDbTransaction tx);                    
    }

    public interface IParticipationRepository
    {
        Task<int> RegisterAsync(int activity_id, int elderly_id, IDbTransaction tx);
        Task CancelAsync(int activity_id, int elderly_id, IDbTransaction tx); 
        Task CheckInAsync(int activity_id, int elderly_id, bool attended, string? feedback, IDbTransaction tx);
        Task<IReadOnlyList<ActivityParticipation>> ListByActivityAsync(int activity_id);
        Task<IReadOnlyList<ActivityParticipation>> ListByElderlyAsync(int elderly_id);
        Task DeleteByActivityAsync(int activity_id, IDbTransaction tx); 
    }
}
