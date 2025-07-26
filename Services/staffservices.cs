using ElderlyCare.Data;
using ElderlyCare.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElderlyCare.Services
{
    public class StaffService : IStaffService
    {
        private readonly ElderlyCareDbContext _context;

        public StaffService(ElderlyCareDbContext context)
        {
            _context = context;
        }

        public async Task<StaffInfo> GetStaffByIdAsync(int staffId)
        {
            return await _context.StaffInfos.FindAsync(staffId);
        }

        public async Task<List<StaffInfo>> GetAllStaffAsync()
        {
            return await _context.StaffInfos.ToListAsync();
        }

        public async Task AddStaffAsync(StaffInfo staff)
        {
            _context.StaffInfos.Add(staff);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateStaffAsync(StaffInfo staff)
        {
            _context.StaffInfos.Update(staff);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteStaffAsync(int staffId)
        {
            var staff = await _context.StaffInfos.FindAsync(staffId);
            if (staff != null)
            {
                _context.StaffInfos.Remove(staff);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<StaffInfo>> GetStaffByPositionAsync(string position)
        {
            return await _context.StaffInfos
                .Where(s => s.Position == position)
                .ToListAsync();
        }
    }
}