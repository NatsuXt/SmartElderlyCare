using ElderlyCareSystem.Data;
using ElderlyCareSystem.Dtos;
using ElderlyCareSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ElderlyCareSystem.Services
{
    public class ElderlyInfoService
    {
        private readonly AppDbContext _context;

        public ElderlyInfoService(AppDbContext context)
        {
            _context = context;
        }

        // 查询单个老人
        public async Task<ElderlyInfoDto?> GetElderlyByIdAsync(int elderlyId)
        {
            return await _context.ElderlyInfos
                .Where(e => e.ElderlyId == elderlyId)
                .Select(e => new ElderlyInfoDto
                {
                    ElderlyId = e.ElderlyId,
                    Name = e.Name,
                    Gender = e.Gender,
                    BirthDate = e.BirthDate,
                    IdCardNumber = e.IdCardNumber,
                    ContactPhone = e.ContactPhone,
                    Address = e.Address,
                    EmergencyContact = e.EmergencyContact
                })
                .FirstOrDefaultAsync();
        }

        // 查询所有老人
        public async Task<List<ElderlyInfoDto>> GetAllElderliesAsync()
        {
            return await _context.ElderlyInfos
                .Select(e => new ElderlyInfoDto
                {
                    ElderlyId = e.ElderlyId,
                    Name = e.Name,
                    Gender = e.Gender,
                    BirthDate = e.BirthDate,
                    IdCardNumber = e.IdCardNumber,
                    ContactPhone = e.ContactPhone,
                    Address = e.Address,
                    EmergencyContact = e.EmergencyContact
                })
                .ToListAsync();
        }
        /// <summary>
        /// 按属性名修改 ElderlyInfo 表中的指定属性
        /// </summary>
        public async Task<bool> UpdatePropertyAsync(int elderlyId, string propertyName, object value)
        {
            var entity = await _context.ElderlyInfos.FindAsync(elderlyId);
            if (entity == null) return false;

            // 通过反射获取属性
            var prop = typeof(ElderlyInfo).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (prop == null) return false; // 属性不存在

            // 尝试类型转换
            try
            {
                var convertedValue = Convert.ChangeType(value, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                prop.SetValue(entity, convertedValue);
            }
            catch
            {
                return false; // 类型转换失败
            }

            await _context.SaveChangesAsync();
            return true;
        }


        // 删除老人信息
        public async Task<bool> DeleteElderlyAsync(int elderlyId)
        {
            var entity = await _context.ElderlyInfos.FindAsync(elderlyId);
            if (entity == null) return false;

            _context.ElderlyInfos.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
