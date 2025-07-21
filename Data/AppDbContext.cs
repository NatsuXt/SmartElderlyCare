using Microsoft.EntityFrameworkCore;
using ElderlyCareSystem.Models;
using System.Collections.Generic;

namespace ElderlyCareSystem.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<ElderlyInfo> ElderlyInfo { get; set; }
        public DbSet<HealthAssessmentReport> HealthAssessmentReport { get; set; }
        public DbSet<HealthMonitoring> HealthMonitoring { get; set; }
        public DbSet<FamilyInfo> FamilyInfo { get; set; }


    }
}