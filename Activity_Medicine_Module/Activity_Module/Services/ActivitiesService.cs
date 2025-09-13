using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using ElderCare.Api.Infrastructure;

namespace ElderCare.Api.Modules.Activities
{
    public sealed class ActivitiesService : IActivitiesService
    {
        private readonly IDbConnectionFactory _factory;
        private readonly ActivitiesRepository _activitiesRepo;
        private readonly ParticipationRepository _participationRepo;

        public ActivitiesService(
            IDbConnectionFactory factory,
            ActivitiesRepository activitiesRepository,
            ParticipationRepository participationRepository)
        {
            _factory = factory;
            _activitiesRepo = activitiesRepository;
            _participationRepo = participationRepository;
        }

        public async Task<int> CreateActivityAsync(CreateActivityDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var entity = new ActivitySchedule
            {
                activity_name = dto.activity_name ?? string.Empty,
                activity_date = dto.activity_date,
                activity_time = dto.activity_time ?? "00:00:00", 
                location = dto.location ?? string.Empty,
                staff_id = dto.staff_id,
                activity_description = dto.activity_description,
                elderly_participants = null
            };

            using var conn = _factory.Create();
            if (conn.State != ConnectionState.Open) conn.Open();  
            using var tx = conn.BeginTransaction();

            try
            {
                var id = await _activitiesRepo.CreateAsync(entity, tx);
                tx.Commit();
                return id;
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        public async Task UpdateActivityAsync(int id, UpdateActivityDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            using var conn = _factory.Create();
            if (conn.State != ConnectionState.Open) conn.Open();   
            using var tx = conn.BeginTransaction();

            try
            {
                await _activitiesRepo.UpdateAsync(id, dto, tx);
                tx.Commit();
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        public async Task DeleteActivityAsync(int id)
        {
            using var conn = _factory.Create();
            if (conn.State != ConnectionState.Open) conn.Open();   
            using var tx = conn.BeginTransaction();

            try
            {
                await _activitiesRepo.DeleteAsync(id, tx);
                tx.Commit();
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        public Task<ActivitySchedule?> GetActivityAsync(int id)
            => _activitiesRepo.GetAsync(id);

        public Task<IReadOnlyList<ActivitySchedule>> QueryActivitiesAsync(DateTime? from, DateTime? to, int page, int pageSize)
            => _activitiesRepo.QueryAsync(from, to, page, pageSize);

        public async Task<int> RegisterAsync(RegisterDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            using var conn = _factory.Create();
            if (conn.State != ConnectionState.Open) conn.Open();   
            using var tx = conn.BeginTransaction();

            try
            {
                var newId = await _participationRepo.RegisterAsync(dto.activity_id, dto.elderly_id, tx);
                tx.Commit();
                return newId;
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        public async Task CancelAsync(int activity_id, int elderly_id)
        {
            using var conn = _factory.Create();
            if (conn.State != ConnectionState.Open) conn.Open();   
            using var tx = conn.BeginTransaction();

            try
            {
                await _participationRepo.CancelAsync(activity_id, elderly_id, tx);
                tx.Commit();
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        public async Task CheckInAsync(SignInDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            using var conn = _factory.Create();
            if (conn.State != ConnectionState.Open) conn.Open();   
            using var tx = conn.BeginTransaction();

            try
            {
                await _participationRepo.CheckInAsync(dto.activity_id, dto.elderly_id, tx);
                tx.Commit();
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        public async Task<IReadOnlyList<ActivityParticipation>> GetParticipationByActivityAsync(int activity_id)
        {
            var rows = await _participationRepo.ListByActivityAsync(activity_id);
            return rows.Select(r => new ActivityParticipation
            {
                participation_id = r.participation_id,
                activity_id = r.activity_id,
                elderly_id = r.elderly_id,
                status = r.status,
                registration_time = r.registration_time,
                check_in_time = r.check_in_time
            }).ToList();
        }

        public async Task<IReadOnlyList<ActivityParticipation>> GetParticipationByElderlyAsync(int elderly_id)
        {
            var rows = await _participationRepo.ListByElderlyAsync(elderly_id);
            return rows.Select(r => new ActivityParticipation
            {
                participation_id = r.participation_id,
                activity_id = r.activity_id,
                elderly_id = elderly_id,
                status = r.raw_status,
                registration_time = r.registration_time,
                check_in_time = r.check_in_time
            }).ToList();
        }
    }
}
