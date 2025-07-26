using ElderlyCare.Data;
using ElderlyCare.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElderlyCare.Services
{
    public class NursingPlanService : INursingPlanService
    {
        private readonly ElderlyCareDbContext _context;

        public NursingPlanService(ElderlyCareDbContext context)
        {
            _context = context;
        }

        public async Task<List<NursingPlan>> GetAllNursingPlansAsync()
        {
            return await _context.NursingPlans
                //.Include(np => np.Staff)
                //.Include(np => np.Elderly)
                .ToListAsync();
        }

        public async Task<NursingPlan> GetNursingPlanByIdAsync(int planId)
        {
            return await _context.NursingPlans
                //.Include(np => np.Staff)
                // .Include(np => np.Elderly)
                .FirstOrDefaultAsync(np => np.PlanId == planId);
        }

        public async Task AssignNursingPlanAsync(int planId, int staffId)
        {
            var plan = await _context.NursingPlans.FindAsync(planId);
            if (plan != null)
            {
                plan.StaffId = staffId;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<NursingPlan>> GetUnassignedNursingPlansAsync()
        {
            return await _context.NursingPlans
                .Where(np => np.StaffId == null)
                // .Include(np => np.Elderly)
                .ToListAsync();
        }

        public async Task<List<NursingPlan>> GetNursingPlansByStaffIdAsync(int staffId)
        {
            return await _context.NursingPlans
                .Where(np => np.StaffId == staffId)
                //.Include(np => np.Elderly)
                .ToListAsync();
        }

        public async Task CreateNursingPlanAsync(NursingPlan plan)
        {
            _context.NursingPlans.Add(plan);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateNursingPlanAsync(NursingPlan plan)
        {
            _context.NursingPlans.Update(plan);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteNursingPlanAsync(int planId)
        {
            var plan = await _context.NursingPlans.FindAsync(planId);
            if (plan != null)
            {
                _context.NursingPlans.Remove(plan);
                await _context.SaveChangesAsync();
            }
        }
    }
}