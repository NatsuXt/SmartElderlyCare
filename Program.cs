using ElderlyCareSystem.Data;
using ElderlyCareSystem.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.JsonPatch;

using Microsoft.AspNetCore.Mvc;  // 基础 MVC
using Microsoft.Extensions.DependencyInjection; // IServiceCollection 扩展方法
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("DefaultConnection")));


// 2. 注入 CheckInService
builder.Services.AddScoped<ElderlyFullRegistrationService>();
builder.Services.AddScoped<ElderlyRecordService>();
builder.Services.AddScoped<FamilyAuthService>();
builder.Services.AddScoped<FamilyQueryService>();
builder.Services.AddScoped<DietRecommendationService>();
builder.Services.AddHttpClient();
builder.Services.AddScoped<QianfanService>();
builder.Services.AddScoped<DietRecommendationService>();
builder.Services.AddScoped<ElderlyInfoService>();

builder.Services.AddSwaggerGen(c =>
{
    var xmlFilename = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
    c.IncludeXmlComments(xmlPath); 
});

// 3. 添加控制器服务
builder.Services.AddControllers();

// 4. 配置 Swagger（API文档，开发环境推荐开启）
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 5. 中间件配置
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
// 默认页面跳转到 Swagger
app.MapGet("/", context =>
{
    context.Response.Redirect("/swagger");
    return Task.CompletedTask;
});
app.UseAuthorization();

app.MapControllers();

app.Run();
