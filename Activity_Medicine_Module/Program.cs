using Dapper;
using ElderCare.Api.Infrastructure;
using ElderCare.Api.Modules.Activities;
using ElderCare.Api.Modules.Medical;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// 固定监听 0.0.0.0:6006
builder.WebHost.ConfigureKestrel(o => o.ListenAnyIP(6006));

// 读取配置
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// 基础服务
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ElderCare API — Medical & Activities",
        Version = "v1",
        Description = "智慧养老院系统：医嘱与药品管理、活动安排模块 API"
    });
});

// Dapper：开启下划线映射（MEDICINE_ID -> medicine_id）
DefaultTypeMap.MatchNamesWithUnderscores = true;

// 注册 Oracle 连接工厂（只创建不打开）
builder.Services.AddSingleton<IDbConnectionFactory>(sp =>
{
    var cs = builder.Configuration.GetConnectionString("DefaultConnection");
    if (string.IsNullOrWhiteSpace(cs))
        throw new InvalidOperationException("连接字符串 'DefaultConnection' 未配置或为空。");
    return new OracleConnectionFactory(cs!);
});

// 可选：配置 Schema 前缀（仓储里用 Tables.Schema 拼接完全限定名）
var schema = builder.Configuration.GetSection("Oracle:Schema").Value;
if (!string.IsNullOrWhiteSpace(schema))
    Tables.Schema = schema.Trim().ToUpperInvariant();

// 仓储 / 服务注册（控制器依赖实现类即可）
builder.Services.AddScoped<IMedicalOrdersRepository, MedicalOrdersRepository>();
builder.Services.AddScoped<MedicinesRepository>();
builder.Services.AddScoped<ProcurementRepository>();
builder.Services.AddScoped<ReminderRepository>();
builder.Services.AddScoped<HealthThresholdsService>();
builder.Services.AddScoped<AlertsService>();
builder.Services.AddScoped<IMedicalService, MedicalService>();
builder.Services.AddScoped<MedicalService>();
builder.Services.AddScoped<DispenseRepository>();
builder.Services.AddScoped<StockPriceRepository>();
builder.Services.AddScoped<ActivitiesRepository>();
builder.Services.AddScoped<ParticipationRepository>();
builder.Services.AddScoped<IActivitiesService, ActivitiesService>();

// 统一异常处理中间件
builder.Services.AddSingleton<ErrorHandlingMiddleware>();

var app = builder.Build();

// 中间件
app.UseMiddleware<ErrorHandlingMiddleware>();

// Swagger（开发/生产都开放，便于调试）
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.RoutePrefix = "swagger";
});

// 不强制 HTTPS（若有反代/证书再开启）
// app.UseHttpsRedirection();

app.MapControllers();
app.Run();
