using Microsoft.EntityFrameworkCore;
using ElderlyCare.Models;

namespace ElderlyCare.Data
{
    public class ElderlyCareDbContext : DbContext
    {
        public ElderlyCareDbContext(DbContextOptions<ElderlyCareDbContext> options) : base(options)
        {
        }

        public DbSet<StaffInfo> StaffInfos { get; set; }
        public DbSet<NursingPlan> NursingPlans { get; set; }
        public DbSet<ActivitySchedule> ActivitySchedules { get; set; }
        public DbSet<MedicalOrder> MedicalOrders { get; set; }
        public DbSet<OperationLog> OperationLogs { get; set; }
        public DbSet<EmergencySOS> EmergencySOS { get; set; }
        public DbSet<SystemAnnouncement> SystemAnnouncements { get; set; }
        public DbSet<DisinfectionRecord> DisinfectionRecords { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // StaffInfo 关系配置
            modelBuilder.Entity<StaffInfo>()
                .HasMany(s => s.NursingPlans)
                .WithOne(np => np.Staff)
                .HasForeignKey(np => np.StaffId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<StaffInfo>()
                .HasMany(s => s.ActivitySchedules)
                .WithOne(a => a.Staff)
                .HasForeignKey(a => a.StaffId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<StaffInfo>()
                .HasMany(s => s.MedicalOrders)
                .WithOne(mo => mo.Staff)
                .HasForeignKey(mo => mo.StaffId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<StaffInfo>()
                .HasMany(s => s.OperationLogs)
                .WithOne(ol => ol.Staff)
                .HasForeignKey(ol => ol.StaffId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<StaffInfo>()
                .HasMany(s => s.EmergencySOSResponses)
                .WithOne(es => es.ResponseStaff)
                .HasForeignKey(es => es.ResponseStaffId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<StaffInfo>()
                .HasMany(s => s.SystemAnnouncements)
                .WithOne(sa => sa.CreatedBy)
                .HasForeignKey(sa => sa.CreatedById)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<StaffInfo>()
                .HasMany(s => s.DisinfectionRecords)
                .WithOne(dr => dr.Staff)
                .HasForeignKey(dr => dr.StaffId)
                .OnDelete(DeleteBehavior.SetNull);

            // NursingPlan 关系配置
            //modelBuilder.Entity<NursingPlan>()
                //.HasOne(np => np.Elderly)
                //  .WithMany()
                //.HasForeignKey(np => np.ElderlyId)
                //.OnDelete(DeleteBehavior.Restrict);

            // ActivitySchedule 关系配置
                //modelBuilder.Entity<ActivitySchedule>()
                //.HasMany(a => a.ActivityParticipations)
                //.WithOne()
                //.HasForeignKey(ap => ap.ActivityId)
                //.OnDelete(DeleteBehavior.Cascade);

            // MedicalOrder 关系配置
                //modelBuilder.Entity<MedicalOrder>()
                //.HasOne(mo => mo.Elderly)
                //.WithMany()
                //.HasForeignKey(mo => mo.ElderlyId)
                //.OnDelete(DeleteBehavior.Restrict);

                //modelBuilder.Entity<MedicalOrder>()
                //.HasOne(mo => mo.Medicine)
                //.WithMany()
                //.HasForeignKey(mo => mo.MedicineId)
                //.OnDelete(DeleteBehavior.Restrict);

                //modelBuilder.Entity<MedicalOrder>()
                //.HasMany(mo => mo.VoiceAssistantReminders)
                //.WithOne()
                //.HasForeignKey(var => var.OrderId)
                //.OnDelete(DeleteBehavior.Cascade);

            // EmergencySOS 关系配置
                //modelBuilder.Entity<EmergencySOS>()
                //.HasOne(es => es.Elderly)
                //.WithMany()
                //.HasForeignKey(es => es.ElderlyId)
                //.OnDelete(DeleteBehavior.Restrict);

                //modelBuilder.Entity<EmergencySOS>()
                //.HasOne(es => es.Room)
                //.WithMany()
                //.HasForeignKey(es => es.RoomId)
                //.OnDelete(DeleteBehavior.SetNull);

            // 添加索引以提高查询性能
            modelBuilder.Entity<StaffInfo>()
                .HasIndex(s => s.Name);

            modelBuilder.Entity<NursingPlan>()
                .HasIndex(np => np.ElderlyId);

            modelBuilder.Entity<NursingPlan>()
                .HasIndex(np => np.StaffId);

            modelBuilder.Entity<ActivitySchedule>()
                .HasIndex(a => a.ActivityDate);

            modelBuilder.Entity<MedicalOrder>()
                .HasIndex(mo => mo.ElderlyId);

            modelBuilder.Entity<EmergencySOS>()
                .HasIndex(es => es.ElderlyId);

            modelBuilder.Entity<EmergencySOS>()
                .HasIndex(es => es.CallTime);

            modelBuilder.Entity<DisinfectionRecord>()
                .HasIndex(dr => dr.DisinfectionTime);

            // 配置表名和列名约定
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                // 设置表名为复数形式
                entity.SetTableName(entity.DisplayName() + "s");

                // 设置列名为小写加下划线格式
                foreach (var property in entity.GetProperties())
                {
                    property.SetColumnName(ToSnakeCase(property.Name));
                }
            }
        }

        private string ToSnakeCase(string input)
        {
            if (string.IsNullOrEmpty(input)) { return input; }

            var startUnderscores = System.Text.RegularExpressions.Regex.Match(input, @"^_+");
            return startUnderscores + System.Text.RegularExpressions.Regex.Replace(input, @"([a-z0-9])([A-Z])", "$1_$2").ToLower();
        }
    }
}
