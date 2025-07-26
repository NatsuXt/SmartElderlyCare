using ElderlyCare.Data;
using ElderlyCare.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElderlyCare.Services
{
    public class DisinfectionRecordService : IDisinfectionRecordService
    {
        private readonly ElderlyCareDbContext _context;

        public DisinfectionRecordService(ElderlyCareDbContext context)
        {
            _context = context;
        }

        public async Task CreateDisinfectionRecordAsync(DisinfectionRecord record)
        {
            record.DisinfectionTime = DateTime.Now;
            _context.DisinfectionRecords.Add(record);
            await _context.SaveChangesAsync();
        }

        public async Task<List<DisinfectionRecord>> GetDisinfectionRecordsAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.DisinfectionRecords
                .Include(dr => dr.Staff)
                .AsQueryable();

            if (startDate.HasValue)
            {
                query = query.Where(dr => dr.DisinfectionTime >= startDate);
            }

            if (endDate.HasValue)
            {
                query = query.Where(dr => dr.DisinfectionTime <= endDate);
            }

            return await query
                .OrderByDescending(dr => dr.DisinfectionTime)
                .ToListAsync();
        }

        public async Task<List<DisinfectionRecord>> GetDisinfectionRecordsByStaffIdAsync(int staffId)
        {
            return await _context.DisinfectionRecords
                .Where(dr => dr.StaffId == staffId)
                .Include(dr => dr.Staff)
                .OrderByDescending(dr => dr.DisinfectionTime)
                .ToListAsync();
        }

        public async Task<DisinfectionRecord> GetDisinfectionRecordByIdAsync(int recordId)
        {
            return await _context.DisinfectionRecords
                .Include(dr => dr.Staff)
                .FirstOrDefaultAsync(dr => dr.DisinfectionId == recordId);
        }

        public async Task DeleteDisinfectionRecordAsync(int recordId)
        {
            var record = await _context.DisinfectionRecords.FindAsync(recordId);
            if (record != null)
            {
                _context.DisinfectionRecords.Remove(record);
                await _context.SaveChangesAsync();
            }
        }

        public async Task GenerateMonthlyDisinfectionReportAsync(int year, int month)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            var records = await GetDisinfectionRecordsAsync(startDate, endDate);

            // 这里应该有生成报告的逻辑
            // 例如：按区域统计消毒次数、按员工统计消毒次数等
            // 实际项目中可能会返回一个报告对象或生成PDF/Excel文件
        }
    }
}