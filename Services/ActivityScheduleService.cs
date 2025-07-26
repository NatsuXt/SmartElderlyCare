using ElderlyCare.Data;
using ElderlyCare.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElderlyCare.Services
{
    public class ActivityScheduleService : IActivityScheduleService
    {
        private readonly ElderlyCareDbContext _context;

        public ActivityScheduleService(ElderlyCareDbContext context)
        {
            _context = context;
        }

        public async Task<List<ActivitySchedule>> GetAllActivitiesAsync()
        {
            return await _context.ActivitySchedules
                .Include(a => a.Staff)
                .ToListAsync();
        }

        public async Task<ActivitySchedule> GetActivityByIdAsync(int activityId)
        {
            return await _context.ActivitySchedules
                .Include(a => a.Staff)
                .FirstOrDefaultAsync(a => a.ActivityId == activityId);
        }

        public async Task CreateActivityAsync(ActivitySchedule activity)
        {
            _context.ActivitySchedules.Add(activity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateActivityAsync(ActivitySchedule activity)
        {
            _context.ActivitySchedules.Update(activity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteActivityAsync(int activityId)
        {
            var activity = await _context.ActivitySchedules.FindAsync(activityId);
            if (activity != null)
            {
                _context.ActivitySchedules.Remove(activity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<ActivitySchedule>> GetActivitiesByStaffIdAsync(int staffId)
        {
            return await _context.ActivitySchedules
                .Where(a => a.StaffId == staffId)
                .ToListAsync();
        }

        public async Task<List<ActivitySchedule>> GetUpcomingActivitiesAsync(int days)
        {
            var today = DateTime.Today;
            var endDate = today.AddDays(days);
            return await _context.ActivitySchedules
                .Where(a => a.ActivityDate >= today && a.ActivityDate <= endDate)
                .OrderBy(a => a.ActivityDate)
                .ThenBy(a => a.ActivityTime)
                .ToListAsync();
        }
    }
}