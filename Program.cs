using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Staff_Info.Data;
using Staff_Info.Services;

var builder = WebApplication.CreateBuilder(args);

// 加载配置（确保 appsettings.json 存在）
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Oracle 连接字符串（从配置读取）
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("连接字符串 'DefaultConnection' 未配置或为空。");
}

// 添加 DbContext（Oracle）
builder.Services.AddDbContext<StaffInfoDbContext>(options =>
    options.UseOracle(connectionString));

// 注册服务层接口
builder.Services.AddScoped<INursingSchedulingService, NursingSchedulingService>();
builder.Services.AddScoped<IEmergencySOSService, EmergencySOSService>();
builder.Services.AddScoped<IDisinfectionService, DisinfectionService>();



// 添加控制器支持
builder.Services.AddControllers();

// Swagger 配置
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "养老院员工信息管理系统 API",
        Version = "v1",
        Description = "包含员工管理、护理排班、SOS响应和消毒记录等核心功能"
    });
    

   
});

var app = builder.Build();

// 开发环境配置
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

    // 可选：自动迁移数据库（使用 Migrate 会尝试更改数据库结构）
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<StaffInfoDbContext>();
    db.Database.Migrate();
}
else
{
    // 生产环境异常处理
    app.UseExceptionHandler("/error");
}

// 启用 Swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Staff Info API v1");
    c.RoutePrefix = string.Empty; // 直接访问根路径显示Swagger UI
});

// 默认跳转到 Swagger
app.Use(async (context, next) =>
{
    if (context.Request.Path == "/")
    {
        context.Response.Redirect("/swagger");
        return;
    }
    await next();
});

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllers();
//app.Urls.Add("http://0.0.0.0:5000");
app.Run();
