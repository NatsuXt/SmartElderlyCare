using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    // 使用标准的 DbContextOptions 构造函数
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // 只保留职责范围内的实体
    public DbSet<SystemAnnouncements> SystemAnnouncements { get; set; }
    public DbSet<OperationLog> OperationLogs { get; set; }
    public DbSet<VisitorRegistration> VisitorRegistrations { get; set; }
    public DbSet<HealthMonitoring> HealthMonitorings { get; set; }
    public DbSet<FeeSettlement> FeeSettlements { get; set; }

    // 注意：StaffInfo、ElderlyInfo、FamilyInfo 由其他模块管理
    // 我们通过外键ID来引用，但不在这个Context中管理这些实体

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // 暂时简化配置，避免复杂的外键关系问题
        // 在实际项目中，外键关系由数据库约束来保证
    }
}