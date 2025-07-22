using ElderlyCareSystem.Data;
using ElderlyCareSystem.Services;
using ElderlyFullRegistrationDto.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. �������ݿ������ַ������������õ��� SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. ע�� CheckInService
builder.Services.AddScoped<ElderlyFullRegistrationService>();
builder.Services.AddScoped<ElderlyRecordService>();
builder.Services.AddScoped<FamilyAuthService>();

// 3. ��ӿ���������
builder.Services.AddControllers();

// 4. ���� Swagger��API�ĵ������������Ƽ�������
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 5. �м������
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
