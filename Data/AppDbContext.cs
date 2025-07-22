using Microsoft.EntityFrameworkCore;
using ElderlyCareSystem.Models;

namespace ElderlyCareSystem.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // === DbSet 区域 ===
        public DbSet<ElderlyInfo> ElderlyInfos { get; set; }
        public DbSet<FamilyInfo> FamilyInfos { get; set; }
        public DbSet<HealthMonitoring> HealthMonitorings { get; set; }
        public DbSet<HealthAssessmentReport> HealthAssessmentReports { get; set; }
        public DbSet<MedicalOrder> MedicalOrders { get; set; }
        public DbSet<NursingPlan> NursingPlans { get; set; }
        public DbSet<FeeSettlement> FeeSettlements { get; set; }
        public DbSet<ActivityParticipation> ActivityParticipations { get; set; }
        public DbSet<DietRecommendation> DietRecommendations { get; set; }
        public DbSet<EmergencySOS> EmergencySOSs { get; set; }
        public DbSet<OperationLog> OperationLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // === FamilyInfo 外键关联 ElderlyInfo ===
            modelBuilder.Entity<FamilyInfo>()
                .HasOne(f => f.Elderly)
                .WithMany(e => e.Families)
                .HasForeignKey(f => f.ElderlyId)
                .OnDelete(DeleteBehavior.Cascade);

            // === HealthMonitoring 外键关联 ElderlyInfo ===
            modelBuilder.Entity<HealthMonitoring>()
                .HasOne(h => h.Elderly)
                .WithMany(e => e.HealthMonitorings)
                .HasForeignKey(h => h.ElderlyId)
                .OnDelete(DeleteBehavior.Cascade);

            // === HealthAssessmentReport 外键关联 ElderlyInfo ===
            modelBuilder.Entity<HealthAssessmentReport>()
                .HasOne(h => h.Elderly)
                .WithMany(e => e.HealthAssessmentReports)
                .HasForeignKey(h => h.ElderlyId)
                .OnDelete(DeleteBehavior.Cascade);

            // === DietRecommendation 外键关联 ElderlyInfo ===
            modelBuilder.Entity<DietRecommendation>()
                .HasOne(d => d.Elderly)
                .WithMany(e => e.DietRecommendations)
                .HasForeignKey(d => d.ElderlyId)
                .OnDelete(DeleteBehavior.Cascade);

            // === EmergencySOS 外键关联 ElderlyInfo ===
            modelBuilder.Entity<EmergencySOS>()
                .HasOne(e => e.Elderly)
                .WithMany(ei => ei.EmergencySOSCalls)
                .HasForeignKey(e => e.ElderlyId)
                .OnDelete(DeleteBehavior.Cascade);

            // === ActivityParticipation 外键 ElderlyInfo（注意：还有 activity_id，此处简化处理）===
            modelBuilder.Entity<ActivityParticipation>()
                .HasOne(a => a.Elderly)
                .WithMany(e => e.ActivityParticipations)
                .HasForeignKey(a => a.ElderlyId)
                .OnDelete(DeleteBehavior.Cascade);

            // 其它表的关系如 MedicalOrder、NursingPlan、FeeSettlement 可根据是否需要导航属性补充

            base.OnModelCreating(modelBuilder);
        }
    }
}
