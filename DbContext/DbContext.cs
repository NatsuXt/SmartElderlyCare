using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Staff_Info.Models;

namespace Staff_Info.Data
{
    public class StaffInfoDbContext : DbContext
    {
        public StaffInfoDbContext(DbContextOptions<StaffInfoDbContext> options) : base(options)
        {
        }

        // 定义所有DbSet
        public DbSet<ACTIVITYSCHEDULE> ActivitySchedules { get; set; }
        public DbSet<DISINFECTIONRECORD> DisinfectionRecords { get; set; }
        public DbSet<ELDERLYINFO> ElderlyInfos { get; set; }
        public DbSet<EMERGENCYSOS> EmergencySOS { get; set; }
        public DbSet<MEDICALORDER> MedicalOrders { get; set; }
        public DbSet<MEDICINEINVENTORY> MedicineInventories { get; set; }
        public DbSet<NURSINGPLAN> NursingPlans { get; set; }
        public DbSet<OPERATIONLOG> OperationLogs { get; set; }
        public DbSet<ROOMMANAGEMENT> RoomManagements { get; set; }
        public DbSet<SOSNOTIFICATION> SOSNotifications { get; set; }
        public DbSet<STAFFINFO> StaffInfos { get; set; }
        public DbSet<STAFFLOCATION> StaffLocations { get; set; }
        public DbSet<STAFFSCHEDULE> StaffSchedules { get; set; }
        public DbSet<SYSTEMANNOUNCEMENTS> SystemAnnouncements { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    // 配置表名
    modelBuilder.Entity<ACTIVITYSCHEDULE>().ToTable("ACTIVITYSCHEDULE");
    modelBuilder.Entity<DISINFECTIONRECORD>().ToTable("DISINFECTIONRECORD");
    modelBuilder.Entity<ELDERLYINFO>().ToTable("ELDERLYINFO");
    modelBuilder.Entity<EMERGENCYSOS>().ToTable("EMERGENCYSOS");
    modelBuilder.Entity<MEDICALORDER>().ToTable("MEDICALORDER");
    modelBuilder.Entity<MEDICINEINVENTORY>().ToTable("MEDICINEINVENTORY");
    modelBuilder.Entity<NURSINGPLAN>().ToTable("NURSINGPLAN");
    modelBuilder.Entity<OPERATIONLOG>().ToTable("OPERATIONLOG");
    modelBuilder.Entity<ROOMMANAGEMENT>().ToTable("ROOMMANAGEMENT");
    modelBuilder.Entity<SOSNOTIFICATION>().ToTable("SOSNOTIFICATION");
    modelBuilder.Entity<STAFFINFO>().ToTable("STAFFINFO");
    modelBuilder.Entity<STAFFLOCATION>().ToTable("STAFFLOCATION");
    modelBuilder.Entity<STAFFSCHEDULE>().ToTable("STAFFSCHEDULE");
    modelBuilder.Entity<SYSTEMANNOUNCEMENTS>().ToTable("SYSTEMANNOUNCEMENTS");
    modelBuilder.Entity<EMERGENCYSOS>(entity =>
    {
        // 配置主键
        entity.HasKey(e => e.CALL_ID);
    
        // 配置布尔值转换
        entity.Property(e => e.FOLLOW_UP_REQUIRED)
            .HasConversion(
                v => v ? 1 : 0,
                v => v == 1);
    
        // 配置关系
        entity.HasOne(e => e.ELDERLY)
            .WithMany(e => e.EMERGENCYSOS)
            .HasForeignKey(e => e.ELDERLY_ID);
    
        entity.HasOne(e => e.ROOM)
            .WithMany()
            .HasForeignKey(e => e.ROOM_ID);
    
        entity.HasOne(e => e.RESPONSE_STAFF)
            .WithMany()
            .HasForeignKey(e => e.RESPONSE_STAFF_ID);
    });
    // 配置Decimal类型的精度
    ConfigureDecimalPrecision(modelBuilder);
    modelBuilder.HasSequence<decimal>("DISINFECTIONRECORD_SEQ");

    modelBuilder.Entity<DISINFECTIONRECORD>()
        .Property(d => d.DISINFECTION_ID)
        .HasDefaultValueSql("DISINFECTIONRECORD_SEQ.NEXTVAL");
    // 配置关系
    ConfigureRelationships(modelBuilder);
}

private void ConfigureDecimalPrecision(ModelBuilder modelBuilder)
{
    // 为所有Decimal类型的主键和外键配置精度
    modelBuilder.Entity<ACTIVITYSCHEDULE>()
        .Property(a => a.ACTIVITY_ID)
        .HasPrecision(18, 2);
    
    modelBuilder.Entity<DISINFECTIONRECORD>()
        .Property(d => d.DISINFECTION_ID)
        .HasPrecision(18, 2);
    
    modelBuilder.Entity<ELDERLYINFO>()
        .Property(e => e.ELDERLY_ID)
        .HasColumnType("NUMBER(10)");
    
    modelBuilder.Entity<EMERGENCYSOS>()
        .Property(e => e.CALL_ID)
        .HasColumnType("NUMBER(10)"); // 明确指定为整数类型
    
    modelBuilder.Entity<MEDICALORDER>()
        .Property(m => m.ORDER_ID)
        .HasPrecision(18, 2);
    
    modelBuilder.Entity<MEDICALORDER>()
        .Property(m => m.ELDERLY_ID)
        .HasPrecision(18, 2);
    
    modelBuilder.Entity<MEDICINEINVENTORY>()
        .Property(m => m.MEDICINE_ID)
        .HasPrecision(18, 2);
    
    modelBuilder.Entity<MEDICINEINVENTORY>()
        .Property(m => m.UNIT_PRICE)
        .HasPrecision(18, 2);
    
    modelBuilder.Entity<NURSINGPLAN>()
        .Property(n => n.PLAN_ID)
        .HasPrecision(18, 2);
    
    modelBuilder.Entity<OPERATIONLOG>()
        .Property(o => o.LOG_ID)
        .HasPrecision(18, 2);
    
    modelBuilder.Entity<ROOMMANAGEMENT>()
        .Property(r => r.ROOM_ID)
        .HasPrecision(18, 2);
    
    modelBuilder.Entity<ROOMMANAGEMENT>()
        .Property(r => r.CAPACITY)
        .HasPrecision(18, 2);
    
    modelBuilder.Entity<ROOMMANAGEMENT>()
        .Property(r => r.FLOOR)
        .HasPrecision(18, 2);
    
    modelBuilder.Entity<ROOMMANAGEMENT>()
        .Property(r => r.RATE)
        .HasPrecision(18, 2);
    
    modelBuilder.Entity<SOSNOTIFICATION>()
        .Property(s => s.NOTIFICATION_ID)
        .HasPrecision(18, 2);
    
    modelBuilder.Entity<STAFFINFO>()
        .Property(s => s.STAFF_ID)
        .HasPrecision(18, 2);
    
    modelBuilder.Entity<STAFFINFO>()
        .Property(s => s.SALARY)
        .HasPrecision(18, 2);
    
    modelBuilder.Entity<STAFFLOCATION>()
        .Property(s => s.LOCATION_ID)
        .HasPrecision(18, 2);
    
    modelBuilder.Entity<STAFFSCHEDULE>()
        .Property(s => s.SCHEDULE_ID)
        .HasPrecision(18, 2);
    
    modelBuilder.Entity<SYSTEMANNOUNCEMENTS>()
        .Property(s => s.ANNOUNCEMENT_ID)
        .HasPrecision(18, 2);
    
    modelBuilder.Entity<SYSTEMANNOUNCEMENTS>()
        .Property(s => s.CREATED_BY)
        .HasPrecision(18, 2);
}

private void ConfigureRelationships(ModelBuilder modelBuilder)
{
    
    // 解决MEDICALORDER的ELDERLY_ID冲突
    modelBuilder.Entity<MEDICALORDER>()
        .HasOne(mo => mo.ELDERLY)
        .WithMany()
        .HasForeignKey(mo => mo.ELDERLY_ID)
        .IsRequired(false); // 如果允许为空
    
    // StaffInfo 关系
    modelBuilder.Entity<STAFFINFO>()
        .HasMany(s => s.ACTIVITYSCHEDULES)
        .WithOne(a => a.STAFF)
        .HasForeignKey(a => a.STAFF_ID)
        .IsRequired(false);

    modelBuilder.Entity<STAFFINFO>()
        .HasMany(s => s.DISINFECTIONRECORDS)
        .WithOne(d => d.STAFF)
        .HasForeignKey(d => d.STAFF_ID)
        .IsRequired(false);

    modelBuilder.Entity<STAFFINFO>()
        .HasMany(s => s.MEDICALORDERS)
        .WithOne(m => m.STAFF)
        .HasForeignKey(m => m.STAFF_ID)
        .IsRequired(false);

    modelBuilder.Entity<STAFFINFO>()
        .HasMany(s => s.NURSINGPLANS)
        .WithOne(n => n.STAFF)
        .HasForeignKey(n => n.STAFF_ID)
        .IsRequired(false);

    modelBuilder.Entity<STAFFINFO>()
        .HasMany(s => s.OPERATIONLOGS)
        .WithOne(o => o.STAFF)
        .HasForeignKey(o => o.STAFF_ID)
        .IsRequired(false);

    modelBuilder.Entity<STAFFINFO>()
        .HasMany(s => s.SOSNOTIFICATIONS)
        .WithOne(sn => sn.STAFF)
        .HasForeignKey(sn => sn.STAFF_ID)
        .IsRequired(false);

    modelBuilder.Entity<STAFFINFO>()
        .HasMany(s => s.STAFFSCHEDULES)
        .WithOne(ss => ss.STAFF)
        .HasForeignKey(ss => ss.STAFF_ID)
        .IsRequired(false);

    modelBuilder.Entity<STAFFINFO>()
        .HasMany(s => s.RESPONSIBLE_EMERGENCYSOS)
        .WithOne(e => e.RESPONSE_STAFF)
        .HasForeignKey(e => e.RESPONSE_STAFF_ID)
        .IsRequired(false);

    // 其他关系配置...
    modelBuilder.Entity<EMERGENCYSOS>()
        .HasOne(e => e.ELDERLY)
        .WithMany(e => e.EMERGENCYSOS) // Make sure ELDERLYINFO has this collection
        .HasForeignKey(e => e.ELDERLY_ID)
        .IsRequired(false);

    modelBuilder.Entity<EMERGENCYSOS>()
        .HasOne(e => e.ROOM)
        .WithMany()
        .HasForeignKey(e => e.ROOM_ID)
        .IsRequired(false);

    modelBuilder.Entity<SOSNOTIFICATION>()
        .HasOne(sn => sn.CALL)
        .WithMany()
        .HasForeignKey(sn => sn.CALL_ID)
        .IsRequired(false);

    modelBuilder.Entity<MEDICALORDER>()
        .HasOne(m => m.MEDICINE)
        .WithMany(mi => mi.MEDICALORDERS)
        .HasForeignKey(m => m.MEDICINE_ID)
        .IsRequired(false);

    modelBuilder.Entity<STAFFLOCATION>()
        .HasOne(sl => sl.ROOM)
        .WithMany()
        .HasForeignKey(sl => sl.FLOOR)
        .IsRequired(false);
}
        
    }
}
