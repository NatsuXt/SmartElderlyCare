using ElderlyCareSystem.Data;
using ElderlyCareSystem.Services;
using ElderlyFullRegistrationDto.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. 配置数据库连接字符串，假设你用的是 SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. 注入 CheckInService
builder.Services.AddScoped<ElderlyFullRegistrationService>();
builder.Services.AddScoped<ElderlyRecordService>();
builder.Services.AddScoped<FamilyAuthService>();

// 3. 添加控制器服务
builder.Services.AddControllers();

// 4. 配置 Swagger（API文档，开发环境推荐开启）
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 5. 中间件配置
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
