using ElderlyCareSystem.Data;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Oracle.EntityFrameworkCore;
using ElderlyCareSystem.Services;
var builder = WebApplication.CreateBuilder(args);

// 添加 AppDbContext 注册（在这里添加）
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<ICheckInService, CheckInService>();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var supportedCultures = new[] { new CultureInfo("zh-CN") };
var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("zh-CN"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
};

app.UseRequestLocalization(localizationOptions);

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
