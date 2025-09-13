namespace ElderCare.Api.Modules.Activities
{
    public interface IActivitiesService
    {
        // Activity
        Task<int> CreateActivityAsync(CreateActivityDto dto);
        Task UpdateActivityAsync(int id, UpdateActivityDto dto);
        Task DeleteActivityAsync(int id); 
        Task<ActivitySchedule?> GetActivityAsync(int id);
        Task<IReadOnlyList<ActivitySchedule>> QueryActivitiesAsync(DateTime? from, DateTime? to, int page, int pageSize);

        // Participation
        Task<int> RegisterAsync(RegisterDto dto);
        Task CancelAsync(int activity_id, int elderly_id);              
        Task CheckInAsync(SignInDto dto);                               
        Task<IReadOnlyList<ActivityParticipation>> GetParticipationByActivityAsync(int activity_id);
        Task<IReadOnlyList<ActivityParticipation>> GetParticipationByElderlyAsync(int elderly_id);
    }
}
