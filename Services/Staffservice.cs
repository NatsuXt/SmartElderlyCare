// Services/StaffService.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using ElderlyCareManagement.Data;
using ElderlyCareManagement.DTOs;
using ElderlyCareManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace ElderlyCareManagement.Services
{
    public interface IStaffService
    {
        Task<List<StaffInfoDTO>> GetAllStaffAsync();
        Task<StaffDetailDTO> GetStaffByIdAsync(int id);
        Task<StaffInfoDTO> AddStaffAsync(StaffCreateDTO staffDto);
        Task UpdateStaffAsync(StaffUpdateDTO staffDto);
        Task DeleteStaffAsync(int id);
    }

    public class StaffService : IStaffService
    {
        private readonly ElderlyCareContext _context;

        public StaffService(ElderlyCareContext context)
        {
            _context = context;
        }

        public async Task<List<StaffInfoDTO>> GetAllStaffAsync()
        {
            return await _context.StaffInfos
                .Select(s => new StaffInfoDTO
                {
                    StaffId = s.StaffId,
                    Name = s.Name,
                    Gender = s.Gender,
                    Position = s.Position,
                    ContactPhone = s.ContactPhone,
                    Email = s.Email,
                    HireDate = s.HireDate,
                    SkillLevel = s.SkillLevel
                })
                .ToListAsync();
        }

        public async Task<StaffDetailDTO> GetStaffByIdAsync(int id)
        {
            var staff = await _context.StaffInfos.FindAsync(id);
            if (staff == null) return null;

            return new StaffDetailDTO
            {
                StaffId = staff.StaffId,
                Name = staff.Name,
                Gender = staff.Gender,
                Position = staff.Position,
                ContactPhone = staff.ContactPhone,
                Email = staff.Email,
                HireDate = staff.HireDate,
                SkillLevel = staff.SkillLevel,
                Salary = staff.Salary,
                WorkSchedule = staff.WorkSchedule
            };
        }

        public async Task<StaffInfoDTO> AddStaffAsync(StaffCreateDTO staffDto)
        {
            var staff = new StaffInfo
            {
                Name = staffDto.Name,
                Gender = staffDto.Gender,
                Position = staffDto.Position,
                ContactPhone = staffDto.ContactPhone,
                Email = staffDto.Email,
                HireDate = staffDto.HireDate,
                Salary = staffDto.Salary,
                SkillLevel = staffDto.SkillLevel,
                WorkSchedule = staffDto.WorkSchedule
            };

            _context.StaffInfos.Add(staff);
            await _context.SaveChangesAsync();

            return new StaffInfoDTO
            {
                StaffId = staff.StaffId,
                Name = staff.Name,
                Gender = staff.Gender,
                Position = staff.Position,
                ContactPhone = staff.ContactPhone,
                Email = staff.Email,
                HireDate = staff.HireDate,
                SkillLevel = staff.SkillLevel
            };
        }

        public async Task UpdateStaffAsync(StaffUpdateDTO staffDto)
        {
            var staff = await _context.StaffInfos.FindAsync(staffDto.StaffId);
            if (staff == null) return;

            staff.Name = staffDto.Name;
            staff.Gender = staffDto.Gender;
            staff.Position = staffDto.Position;
            staff.ContactPhone = staffDto.ContactPhone;
            staff.Email = staffDto.Email;
            staff.HireDate = staffDto.HireDate;
            staff.Salary = staffDto.Salary;
            staff.SkillLevel = staffDto.SkillLevel;
            staff.WorkSchedule = staffDto.WorkSchedule;

            _context.Entry(staff).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteStaffAsync(int id)
        {
            var staff = await _context.StaffInfos.FindAsync(id);
            if (staff != null)
            {
                _context.StaffInfos.Remove(staff);
                await _context.SaveChangesAsync();
            }
        }
    }
}