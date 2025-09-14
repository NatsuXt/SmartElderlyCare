using Microsoft.EntityFrameworkCore;
using RoomDeviceManagement.Models;  // for HealthMonitoring
using api.Models;                   // for FeeSettlement  
using Staff_Info.Models;            // for StaffInfo
using ElderlyCareSystem.Models;     // for ElderlyInfo, FamilyInfo

public class AppDbContext : DbContext
{
    // 使用标准的 DbContextOptions 构造函数
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // 只保留职责范围内的实体（与数据库中实际存在的表对应）
    public DbSet<SystemAnnouncements> SystemAnnouncements { get; set; }
    public DbSet<OperationLog> OperationLogs { get; set; }
    public DbSet<VisitorRegistration> VisitorRegistrations { get; set; }
    public DbSet<VisitorLogin> VisitorLogins { get; set; }
    public DbSet<HealthMonitoring> HealthMonitorings { get; set; }
    public DbSet<FeeSettlement> FeeSettlements { get; set; }
    
    // StaffInfo 需要包含，因为其他表有外键引用
    public DbSet<STAFFINFO> StaffInfos { get; set; }

    // 添加ElderlyInfo用于验证，虽然主要由其他模块管理
    public DbSet<ElderlyInfo> ElderlyInfos { get; set; }

    // 注意：FamilyInfo 由其他模块管理
    // 我们通过外键ID来引用，但不在这个Context中管理这些实体

                protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
        // 明确指定表名映射，确保与数据库中的实际表名一致
                modelBuilder.Entity<SystemAnnouncements>()
            .ToTable("SYSTEMANNOUNCEMENTS");
                    
                modelBuilder.Entity<OperationLog>()
            .ToTable("OPERATIONLOG");
                    
                modelBuilder.Entity<VisitorRegistration>()
            .ToTable("VISITORREGISTRATION");
                    
                modelBuilder.Entity<VisitorLogin>()
            .ToTable("VISITORLOGIN");
                    
                modelBuilder.Entity<HealthMonitoring>()
            .ToTable("HEALTHMONITORING");
                    
                modelBuilder.Entity<FeeSettlement>()
            .ToTable("FEESETTLEMENT");
                    
                modelBuilder.Entity<ElderlyInfo>()
            .ToTable("ELDERLYINFO");

        // 配置decimal类型的精度，避免警告
        modelBuilder.Entity<FeeSettlement>()
            .Property(e => e.insurance_amount)
            .HasPrecision(18, 2);
            
        modelBuilder.Entity<FeeSettlement>()
            .Property(e => e.personal_payment)
            .HasPrecision(18, 2);
            
        modelBuilder.Entity<FeeSettlement>()
            .Property(e => e.total_amount)
            .HasPrecision(18, 2);
            
        modelBuilder.Entity<STAFFINFO>()
            .Property(e => e.SALARY)
            .HasPrecision(18, 2);

        // 暂时简化外键配置，避免复杂的外键关系问题
        // 在实际项目中，外键关系由数据库约束来保证
    }
}