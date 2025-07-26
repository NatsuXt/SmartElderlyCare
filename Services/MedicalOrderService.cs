using ElderlyCare.Data;
using ElderlyCare.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElderlyCare.Services
{
    public class MedicalOrderService : IMedicalOrderService
    {
        private readonly ElderlyCareDbContext _context;

        public MedicalOrderService(ElderlyCareDbContext context)
        {
            _context = context;
        }

        public async Task<List<MedicalOrder>> GetAllMedicalOrdersAsync()
        {
            return await _context.MedicalOrders
                .Include(mo => mo.Staff)
                //.Include(mo => mo.Elderly)
                //.Include(mo => mo.Medicine)
                .ToListAsync();
        }

        public async Task<MedicalOrder> GetMedicalOrderByIdAsync(int orderId)
        {
            return await _context.MedicalOrders
                .Include(mo => mo.Staff)
                //.Include(mo => mo.Elderly)
                //.Include(mo => mo.Medicine)
                .FirstOrDefaultAsync(mo => mo.OrderId == orderId);
        }

        public async Task CreateMedicalOrderAsync(MedicalOrder order)
        {
            _context.MedicalOrders.Add(order);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateMedicalOrderAsync(MedicalOrder order)
        {
            _context.MedicalOrders.Update(order);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteMedicalOrderAsync(int orderId)
        {
            var order = await _context.MedicalOrders.FindAsync(orderId);
            if (order != null)
            {
                _context.MedicalOrders.Remove(order);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<MedicalOrder>> GetMedicalOrdersByStaffIdAsync(int staffId)
        {
            return await _context.MedicalOrders
                .Where(mo => mo.StaffId == staffId)
                //.Include(mo => mo.Elderly)
                //.Include(mo => mo.Medicine)
                .ToListAsync();
        }

        public async Task<List<MedicalOrder>> GetMedicalOrdersByElderlyIdAsync(int elderlyId)
        {
            return await _context.MedicalOrders
                .Where(mo => mo.ElderlyId == elderlyId)
                .Include(mo => mo.Staff)
                //.Include(mo => mo.Medicine)
                .ToListAsync();
        }
    }
}