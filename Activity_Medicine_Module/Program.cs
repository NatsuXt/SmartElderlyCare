using Dapper;
using ElderCare.Api.Infrastructure;
using ElderCare.Api.Modules.Activities;
using ElderCare.Api.Modules.Medical;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// �̶����� 0.0.0.0:6006
builder.WebHost.ConfigureKestrel(o => o.ListenAnyIP(6006));

// ��ȡ����
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// ��������
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ElderCare API �� Medical & Activities",
        Version = "v1",
        Description = "�ǻ�����Ժϵͳ��ҽ����ҩƷ���������ģ�� API"
    });
});

// Dapper�������»���ӳ�䣨MEDICINE_ID -> medicine_id��
DefaultTypeMap.MatchNamesWithUnderscores = true;

// ע�� Oracle ���ӹ�����ֻ�������򿪣�
builder.Services.AddSingleton<IDbConnectionFactory>(sp =>
{
    var cs = builder.Configuration.GetConnectionString("DefaultConnection");
    if (string.IsNullOrWhiteSpace(cs))
        throw new InvalidOperationException("�����ַ��� 'DefaultConnection' δ���û�Ϊ�ա�");
    return new OracleConnectionFactory(cs!);
});

// ��ѡ������ Schema ǰ׺���ִ����� Tables.Schema ƴ����ȫ�޶�����
var schema = builder.Configuration.GetSection("Oracle:Schema").Value;
if (!string.IsNullOrWhiteSpace(schema))
    Tables.Schema = schema.Trim().ToUpperInvariant();

// �ִ� / ����ע�ᣨ����������ʵ���༴�ɣ�
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

// ͳһ�쳣�����м��
builder.Services.AddSingleton<ErrorHandlingMiddleware>();

var app = builder.Build();

// �м��
app.UseMiddleware<ErrorHandlingMiddleware>();

// Swagger������/���������ţ����ڵ��ԣ�
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.RoutePrefix = "swagger";
});

// ��ǿ�� HTTPS�����з���/֤���ٿ�����
// app.UseHttpsRedirection();

app.MapControllers();
app.Run();
