// Services/DisinfectionService.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElderlyCareManagement.Data;
using ElderlyCareManagement.DTOs;
using ElderlyCareManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace ElderlyCareManagement.Services
{
    public interface IDisinfectionService
    {
        Task<DisinfectionRecordDTO> RecordDisinfectionAsync(DisinfectionCreateDTO recordDto);
        Task<List<DisinfectionRecordDTO>> GetDisinfectionRecordsAsync(DateTime? startDate, DateTime? endDate);
        Task<DisinfectionReportDTO> GenerateMonthlyReportAsync(int year, int month);
        Task<DisinfectionRecordDetailDTO> GetDisinfectionRecordDetailAsync(int recordId);
    }

    public class DisinfectionService : IDisinfectionService
    {
        private readonly ElderlyCareContext _context;

        public DisinfectionService(ElderlyCareContext context)
        {
            _context = context;
        }

        public async Task<DisinfectionRecordDTO> RecordDisinfectionAsync(DisinfectionCreateDTO recordDto)
        {
            var record = new DisinfectionRecord
            {
                Area = recordDto.Area,
                StaffId = recordDto.StaffId,
                Method = recordDto.Method,
                DisinfectionTime = DateTime.Now
            };

            _context.DisinfectionRecords.Add(record);
            await _context.SaveChangesAsync();

            return new DisinfectionRecordDTO
            {
                DisinfectionId = record.DisinfectionId,
                Area = record.Area,
                DisinfectionTime = record.DisinfectionTime,
                StaffName = record.Staff.Name,
                Method = record.Method
            };
        }

        public async Task<List<DisinfectionRecordDTO>> GetDisinfectionRecordsAsync(DateTime? startDate, DateTime? endDate)
        {
            var query = _context.DisinfectionRecords
                .Include(d => d.Staff)
                .AsQueryable();

            if (startDate.HasValue)
                query = query.Where(d => d.DisinfectionTime >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(d => d.DisinfectionTime <= endDate.Value);

            return await query
                .OrderByDescending(d => d.DisinfectionTime)
                .Select(d => new DisinfectionRecordDTO
                {
                    DisinfectionId = d.DisinfectionId,
                    Area = d.Area,
                    DisinfectionTime = d.DisinfectionTime,
                    StaffName = d.Staff.Name,
                    Method = d.Method
                })
                .ToListAsync();
        }

        public async Task<DisinfectionReportDTO> GenerateMonthlyReportAsync(int year, int month)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            var records = await GetDisinfectionRecordsAsync(startDate, endDate);

            var report = new DisinfectionReportDTO
            {
                Year = year,
                Month = month,
                TotalDisinfections = records.Count,
                AreasDisinfected = records.GroupBy(r => r.Area)
                    .Select(g => new AreaDisinfectionSummaryDTO
                    {
                        AreaName = g.Key,
                        DisinfectionCount = g.Count(),
                        LastDisinfection = g.Max(r => r.DisinfectionTime)
                    }).ToList(),
                StaffParticipation = records.GroupBy(r => r.StaffName)
                    .Select(g => new StaffDisinfectionSummaryDTO
                    {
                        StaffName = g.Key,
                        DisinfectionCount = g.Count()
                    }).ToList()
            };

            return report;
        }

        public async Task<DisinfectionRecordDetailDTO> GetDisinfectionRecordDetailAsync(int recordId)
        {
            return await _context.DisinfectionRecords
                .Where(d => d.DisinfectionId == recordId)
                .Select(d => new DisinfectionRecordDetailDTO
                {
                    DisinfectionId = d.DisinfectionId,
                    Area = d.Area,
                    DisinfectionTime = d.DisinfectionTime,
                    StaffId = d.StaffId,
                    StaffName = d.Staff.Name,
                    Method = d.Method
                })
                .FirstOrDefaultAsync();
        }
    }
}