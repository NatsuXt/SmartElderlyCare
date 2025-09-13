using Staff_Info.Data;
using Staff_Info.DTOs;
using Staff_Info.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Staff_Info.Services
{
    public interface IDisinfectionService
    {
        Task RecordDisinfectionAsync(DisinfectionRecordDto dto);
        Task<DisinfectionReport> GenerateMonthlyReportAsync(DisinfectionReportRequestDto dto);
        
    }

    public class DisinfectionService : IDisinfectionService
    {
        private readonly StaffInfoDbContext _context;

        public DisinfectionService(StaffInfoDbContext context)
        {
            _context = context;
        }

        public async Task RecordDisinfectionAsync(DisinfectionRecordDto dto)
        {
            // Oracle-compatible staff existence check
            var staffExists = await _context.StaffInfos
                .Where(s => s.STAFF_ID == dto.StaffId)
                .CountAsync() > 0;
            
            if (!staffExists)
            {
                throw new ArgumentException("Staff with provided ID does not exist.");
            }

            // Validate required fields
            if (string.IsNullOrWhiteSpace(dto.Area))
            {
                throw new ArgumentException("Area is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.Methods))
            {
                throw new ArgumentException("Methods is required.");
            }

            // Ensure disinfection time is not in the future
            if (dto.DisinfectionTime > DateTime.Now)
            {
                throw new ArgumentException("Disinfection time cannot be in the future.");
            }
            

            var record = new DISINFECTIONRECORD
            {
                AREA = dto.Area.Trim(),
                DISINFECTION_TIME = dto.DisinfectionTime,
                STAFF_ID = dto.StaffId, // Only set STAFF_ID, not STAFF_ID1
                METHODS = dto.Methods.Trim()
            };

            _context.DisinfectionRecords.Add(record);
            await _context.SaveChangesAsync();
        }

        public async Task<DisinfectionReport> GenerateMonthlyReportAsync(DisinfectionReportRequestDto dto)
        {
            // Validate month and year
            if (dto.Month < 1 || dto.Month > 12)
            {
                throw new ArgumentException("Month must be between 1 and 12.");
            }

            if (dto.Year < 2000 || dto.Year > DateTime.Now.Year + 1)
            {
                throw new ArgumentException($"Year must be between 2000 and {DateTime.Now.Year + 1}.");
            }

            var startDate = new DateTime(dto.Year, dto.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            // Get disinfection records with staff information
            var records = await _context.DisinfectionRecords
                .Include(d => d.STAFF)
                .Where(d => d.DISINFECTION_TIME >= startDate && 
                           d.DISINFECTION_TIME <= endDate)
                .AsNoTracking()
                .ToListAsync();

            // Generate report data with null checks
            var report = new DisinfectionReport
            {
                Month = $"{dto.Year}-{dto.Month:00}",
                TotalDisinfections = records.Count,
                ByArea = records
                    .GroupBy(r => r.AREA)
                    .ToDictionary(g => g.Key ?? "Unknown", g => g.Count()),
                ByStaff = records
                    .GroupBy(r => r.STAFF != null ? r.STAFF.NAME : "Unknown Staff")
                    .ToDictionary(g => g.Key, g => g.Count()),
                ByMethod = records
                    .GroupBy(r => r.METHODS)
                    .ToDictionary(g => g.Key ?? "Unknown", g => g.Count())
            };

            return report;
        }
    }

    public class DisinfectionReport
    {
        public string Month { get; set; }
        public int TotalDisinfections { get; set; }
        public Dictionary<string, int> ByArea { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> ByStaff { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> ByMethod { get; set; } = new Dictionary<string, int>();
    }
}