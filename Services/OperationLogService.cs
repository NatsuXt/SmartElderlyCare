using ElderlyCare.Data;
using ElderlyCare.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElderlyCare.Services
{
    public class OperationLogService : IOperationLogService
    {
        private readonly ElderlyCareDbContext _context;

        public OperationLogService(ElderlyCareDbContext context)
        {
            _context = context;
        }

        public async Task LogOperationAsync(OperationLog log)
        {
            _context.OperationLogs.Add(log);
            await _context.SaveChangesAsync();
        }

        public async Task<List<OperationLog>> GetOperationLogsAsync(int? staffId = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.OperationLogs.AsQueryable();

            if (staffId.HasValue)
            {
                query = query.Where(ol => ol.StaffId == staffId);
            }

            if (startDate.HasValue)
            {
                query = query.Where(ol => ol.OperationTime >= startDate);
            }

            if (endDate.HasValue)
            {
                query = query.Where(ol => ol.OperationTime <= endDate);
            }

            return await query
                .OrderByDescending(ol => ol.OperationTime)
                .ToListAsync();
        }

        public async Task<List<OperationLog>> GetRecentOperationLogsAsync(int count)
        {
            return await _context.OperationLogs
                .OrderByDescending(ol => ol.OperationTime)
                .Take(count)
                .ToListAsync();
        }
    }
}