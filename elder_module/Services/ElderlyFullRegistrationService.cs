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
            // 开启事务
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
                    // 如果有入住状态字段，比如 Status = "入住"
                };
                await _context.ElderlyInfos.AddAsync(elderly);
                await _context.SaveChangesAsync(); // 保存，获取 ElderlyId

                // 2. 添加健康评估报告
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

                // 3. 添加健康监测数据
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

                // 4. 添加家属信息
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
                    }
                }

                await _context.SaveChangesAsync(); // 保存所有新增数据

                // 提交事务
                await transaction.CommitAsync();

                return elderly.ElderlyId;
            }
            catch
            {
                // 发生异常，回滚事务
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
