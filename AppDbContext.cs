using Microsoft.EntityFrameworkCore;
using api.Models;

namespace api;

// Database context
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {}

    public DbSet<ElderlyInfo> ElderlyInfos { get; set; }
    public DbSet<StaffInfo> StaffInfos { get; set; }
    public DbSet<NursingPlan> NursingPlans { get; set; }
    public DbSet<FeeSettlement> FeeSettlements { get; set; }
        public DbSet<FeeDetail> FeeDetails { get; set; }
    public DbSet<MedicalOrder> MedicalOrders { get; set; }
    public DbSet<RoomManagement> RoomManagements { get; set; }
    public DbSet<ActivitySchedule> ActivitySchedules { get; set; }
    public DbSet<ElderlyActivity> ElderlyActivities { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        // 配置多对多关系
        modelBuilder.Entity<NursingPlan>()
            .HasMany(n => n.FeeSettlements)
            .WithMany(f => f.NursingPlans)
            .UsingEntity(j => j.ToTable("NursingPlanFeeSettlement"));
    }
}
