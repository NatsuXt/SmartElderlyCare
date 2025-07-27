// Data/ElderlyCareContext.cs
using ElderlyCareManagement.Models;
using Microsoft.EntityFrameworkCore;


namespace ElderlyCareManagement.Data
{
    public class ElderlyCareContext : DbContext
    {
        public ElderlyCareContext(DbContextOptions<ElderlyCareContext> options) : base(options)
        {
        }

        // 员工信息模块核心表
        public DbSet<StaffInfo> StaffInfos { get; set; }
        public DbSet<NursingPlan> NursingPlans { get; set; }
        public DbSet<ActivitySchedule> ActivitySchedules { get; set; }
        public DbSet<MedicalOrder> MedicalOrders { get; set; }
        public DbSet<OperationLog> OperationLogs { get; set; }
        public DbSet<EmergencySOS> EmergencySos { get; set; }
        public DbSet<SystemAnnouncement> SystemAnnouncements { get; set; }
        public DbSet<DisinfectionRecord> DisinfectionRecords { get; set; }
        public DbSet<SosNotification> SosNotifications { get; set; }
        public DbSet<StaffSchedule> StaffSchedules { get; set; }
        public DbSet<StaffLocation> StaffLocations { get; set; }
        public DbSet<Room> Rooms { get; set; }
        // 相关辅助表
        //public DbSet<ElderlyInfo> ElderlyInfos { get; set; }
        //public DbSet<RoomManagement> RoomManagements { get; set; }
        //public DbSet<MedicineInventory> MedicineInventories { get; set; }
        //public DbSet<ActivityParticipation> ActivityParticipations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // 配置StaffInfo表
            modelBuilder.Entity<StaffInfo>(entity =>
            {
                entity.ToTable("StaffInfo");
                entity.HasKey(e => e.StaffId);
                
                entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Gender).HasMaxLength(10);
                entity.Property(e => e.Position).HasMaxLength(50);
                entity.Property(e => e.ContactPhone).HasMaxLength(20);
                entity.Property(e => e.Email).HasMaxLength(50);
                entity.Property(e => e.SkillLevel).HasMaxLength(100); // 存储多种技能，如"基础护理,急救护理"
                entity.Property(e => e.WorkSchedule).HasMaxLength(200); // 存储工作时间安排
            });

            // 护理计划与员工关系
            modelBuilder.Entity<NursingPlan>()
                .HasOne(np => np.Staff)
                .WithMany(s => s.NursingPlans)
                .HasForeignKey(np => np.StaffId)
                .OnDelete(DeleteBehavior.Restrict);

            // 活动安排与员工关系
            modelBuilder.Entity<ActivitySchedule>()
                .HasOne(a => a.Staff)
                .WithMany(s => s.ActivitySchedules)
                .HasForeignKey(a => a.StaffId)
                .OnDelete(DeleteBehavior.Restrict);

            // 医嘱与员工关系
            modelBuilder.Entity<MedicalOrder>()
                .HasOne(m => m.Staff)
                .WithMany(s => s.MedicalOrders)
                .HasForeignKey(m => m.StaffId)
                .OnDelete(DeleteBehavior.Restrict);

            // 操作日志与员工关系
            modelBuilder.Entity<OperationLog>()
                .HasOne(o => o.Staff)
                .WithMany(s => s.OperationLogs)
                .HasForeignKey(o => o.StaffId)
                .OnDelete(DeleteBehavior.Restrict);

            // 紧急SOS与响应员工关系
            modelBuilder.Entity<EmergencySOS>()
                .HasOne(e => e.ResponseStaff)
                .WithMany(s => s.EmergencySosResponses)
                .HasForeignKey(e => e.ResponseStaffId)
                .OnDelete(DeleteBehavior.Restrict);

            // 系统公告与创建员工关系
            modelBuilder.Entity<SystemAnnouncement>()
                .HasOne(s => s.Staff)
                .WithMany(s => s.SystemAnnouncements)
                .HasForeignKey(s => s.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            // 消毒记录与员工关系
            modelBuilder.Entity<DisinfectionRecord>()
                .HasOne(d => d.Staff)
                .WithMany(s => s.DisinfectionRecords)
                .HasForeignKey(d => d.StaffId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<StaffSchedule>()
                .HasOne(ss => ss.Staff)
                .WithMany(s => s.Schedules)
                .HasForeignKey(ss => ss.StaffId);
            
            modelBuilder.Entity<StaffLocation>()
                .HasOne(sl => sl.Staff)
                .WithMany(s => s.Locations)
                .HasForeignKey(sl => sl.StaffId);
            
            modelBuilder.Entity<StaffLocation>()
                .HasOne(sl => sl.Room)
                .WithMany()
                .HasForeignKey(sl => sl.RoomId);

            // 配置索引以提高查询性能
            modelBuilder.Entity<StaffInfo>().HasIndex(s => s.Position);
            modelBuilder.Entity<StaffInfo>().HasIndex(s => s.SkillLevel);
            modelBuilder.Entity<NursingPlan>().HasIndex(np => np.StaffId);
            modelBuilder.Entity<NursingPlan>().HasIndex(np => np.ElderlyId);
            modelBuilder.Entity<EmergencySOS>().HasIndex(e => e.CallStatus);
            modelBuilder.Entity<EmergencySOS>().HasIndex(e => e.ResponseStaffId);
            modelBuilder.Entity<DisinfectionRecord>().HasIndex(d => d.StaffId);
            modelBuilder.Entity<DisinfectionRecord>().HasIndex(d => d.DisinfectionTime);
        }
    }
}