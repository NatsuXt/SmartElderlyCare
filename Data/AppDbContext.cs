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
        public DbSet<FeeDetail> FeeDetails { get; set; }
        public DbSet<ActivityParticipation> ActivityParticipations { get; set; }
        public DbSet<DietRecommendation> DietRecommendations { get; set; }
        public DbSet<EmergencySOS> EmergencySOSs { get; set; }
        public DbSet<OperationLog> OperationLogs { get; set; }

        public DbSet<HealthAlert> HealthAlerts { get; set; }
        public DbSet<HealthThreshold> HealthThresholds { get; set; }
        public DbSet<VoiceAssistantReminder> VoiceAssistantReminders { get; set; }
        
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

            // === ActivityParticipation 外键关联 ElderlyInfo ===
            modelBuilder.Entity<ActivityParticipation>()
                .HasOne(a => a.Elderly)
                .WithMany(e => e.ActivityParticipations)
                .HasForeignKey(a => a.ElderlyId)
                .OnDelete(DeleteBehavior.Cascade);

            // === FeeSettlement 与 ElderlyInfo ===
            modelBuilder.Entity<FeeSettlement>()
                .HasOne(f => f.Elderly)
                .WithMany(e => e.FeeSettlements)
                .HasForeignKey(f => f.ElderlyId)
                .OnDelete(DeleteBehavior.Cascade);

            // === FeeDetail 与 FeeSettlement ===
            modelBuilder.Entity<FeeDetail>()
                .HasOne(fd => fd.FeeSettlement)
                .WithMany(fs => fs.FeeDetails)
                .HasForeignKey(fd => fd.FeeSettlementId)
                .OnDelete(DeleteBehavior.Cascade);

            // === HealthAlert 外键关联 ElderlyInfo ===
            modelBuilder.Entity<HealthAlert>()
                .HasOne(h => h.Elderly)
                .WithMany(e => e.HealthAlerts)
                .HasForeignKey(h => h.ElderlyId)
                .OnDelete(DeleteBehavior.Cascade);

            // === HealthThreshold 外键关联 ElderlyInfo ===
            modelBuilder.Entity<HealthThreshold>()
                .HasOne(ht => ht.Elderly)
                .WithMany(e => e.HealthThresholds)
                .HasForeignKey(ht => ht.ElderlyId)
                .OnDelete(DeleteBehavior.Cascade);

            // === VoiceAssistantReminder 外键关联 ElderlyInfo ===
            modelBuilder.Entity<VoiceAssistantReminder>()
                .HasOne(r => r.Elderly)
                .WithMany(e => e.VoiceAssistantReminders)
                .HasForeignKey(r => r.ElderlyId)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}
