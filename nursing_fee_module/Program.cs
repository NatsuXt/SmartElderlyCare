using Microsoft.EntityFrameworkCore;
using Oracle.EntityFrameworkCore;
using api.Models;
using api;
using api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Oracle Web API",
        Description = "基于.NET 8和Oracle的Web API示例",
        Version = "v1"
    });
});

// Configure Oracle database connection
var connectionString = builder.Configuration.GetConnectionString("OracleConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseOracle(connectionString));

// 注册费用计算服务
builder.Services.AddScoped<FeeCalculationService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
// 让Swagger在所有环境都可访问
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
