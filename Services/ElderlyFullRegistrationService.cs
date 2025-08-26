using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ElderlyCareSystem.Data;
using ElderlyCareSystem.Dtos;
using ElderlyCareSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace ElderlyCareSystem.Services
{
    public class ElderlyFullRegistrationService
    {
        private readonly AppDbContext _context;

        public ElderlyFullRegistrationService(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 老人入住登记与初次健康评估（包括家属信息），支持事务回滚
        /// </summary>
        public async Task<int> RegisterElderlyAsync(
    ElderlyInfoCreateDto elderlyDto,
    HealthAssessmentReportCreateDto assessmentDto,
    HealthMonitoringCreateDto monitoringDto,
    List<FamilyInfoCreateDto> familyDtos)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 1. 添加老人基本信息
                var elderly = new ElderlyInfo
                {
                    Name = elderlyDto.Name,
                    Gender = elderlyDto.Gender,
                    BirthDate = elderlyDto.BirthDate,
                    IdCardNumber = elderlyDto.IdCardNumber,
                    ContactPhone = elderlyDto.ContactPhone,
                    Address = elderlyDto.Address,
                    EmergencyContact = elderlyDto.EmergencyContact
                };
                await _context.ElderlyInfos.AddAsync(elderly);
                await _context.SaveChangesAsync(); // 保存，获取 ElderlyId

                // 2. 自动为老人生成账户（密码初始值 0000 的哈希）
                var elderlyAccount = new ElderlyAccount
                {
                    ElderlyId = elderly.ElderlyId
                    // PasswordHash 会在构造函数里自动生成 "0000" 的哈希
                };
                await _context.ElderlyAccounts.AddAsync(elderlyAccount);

                // 3. 添加健康评估报告
                var assessment = new HealthAssessmentReport
                {
                    ElderlyId = elderly.ElderlyId,
                    AssessmentDate = assessmentDto.AssessmentDate,
                    PhysicalHealthFunction = assessmentDto.PhysicalHealthFunction,
                    PsychologicalFunction = assessmentDto.PsychologicalFunction,
                    CognitiveFunction = assessmentDto.CognitiveFunction,
                    HealthGrade = assessmentDto.HealthGrade
                };
                await _context.HealthAssessmentReports.AddAsync(assessment);

                // 4. 添加健康监测数据
                var monitoring = new HealthMonitoring
                {
                    ElderlyId = elderly.ElderlyId,
                    MonitoringDate = monitoringDto.MonitoringDate,
                    HeartRate = monitoringDto.HeartRate,
                    BloodPressure = monitoringDto.BloodPressure,
                    OxygenLevel = monitoringDto.OxygenLevel,
                    Temperature = monitoringDto.Temperature,
                    Status = monitoringDto.Status
                };
                await _context.HealthMonitorings.AddAsync(monitoring);

                // 5. 添加家属信息 + 生成账户
                if (familyDtos != null)
                {
                    foreach (var f in familyDtos)
                    {
                        var family = new FamilyInfo
                        {
                            ElderlyId = elderly.ElderlyId,
                            Name = f.Name,
                            Relationship = f.Relationship,
                            ContactPhone = f.ContactPhone,
                            ContactEmail = f.ContactEmail,
                            Address = f.Address,
                            IsPrimaryContact = f.IsPrimaryContact
                        };
                        await _context.FamilyInfos.AddAsync(family);
                        await _context.SaveChangesAsync(); // 保存，获取 FamilyId

                        // 自动生成家属账户
                        var familyAccount = new FamilyAccount
                        {
                            FamilyId = family.FamilyId
                            // PasswordHash 会在构造函数里自动生成 "0000" 的哈希
                        };
                        await _context.FamilyAccounts.AddAsync(familyAccount);
                    }
                }

                // 一次性保存所有新建的账户和健康数据
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return elderly.ElderlyId;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
