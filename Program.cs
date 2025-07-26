using ElderlyCare.Data;
using ElderlyCare.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// 添加服务到容器
ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();

// 配置HTTP请求管道
ConfigurePipeline(app);

// 确保数据库创建和迁移
await EnsureDatabaseCreatedAsync(app);

app.Run();

void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    // 添加控制器
    services.AddControllers();
    
    // 添加Swagger
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen();
    
    // 配置数据库上下文，改成UseOracle
    services.AddDbContext<ElderlyCareDbContext>(options =>
        options.UseOracle(configuration.GetConnectionString("DefaultConnection")));
    
    // 注册服务
    services.AddScoped<IStaffService, StaffService>();
    services.AddScoped<INursingPlanService, NursingPlanService>();
    services.AddScoped<IActivityScheduleService, ActivityScheduleService>();
    services.AddScoped<IMedicalOrderService, MedicalOrderService>();
    services.AddScoped<IOperationLogService, OperationLogService>();
    services.AddScoped<IEmergencySOSService, EmergencySOSService>();
    services.AddScoped<ISystemAnnouncementService, SystemAnnouncementService>();
    services.AddScoped<IDisinfectionRecordService, DisinfectionRecordService>();
    services.AddScoped<INursingSchedulerService, NursingSchedulerService>();
    services.AddScoped<INotificationService, NotificationService>();
    
    // 添加跨域策略
    services.AddCors(options =>
    {
        options.AddPolicy("AllowAll", builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
    });
}

void ConfigurePipeline(WebApplication app)
{
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    
    app.UseHttpsRedirection();
    app.UseCors("AllowAll");
    app.UseAuthorization();
    app.MapControllers();
}

async Task EnsureDatabaseCreatedAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ElderlyCareDbContext>();
        await context.Database.EnsureCreatedAsync();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred creating the DB.");
    }
}
