using ElderlyCareSystem.Data;
using ElderlyCareSystem.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.JsonPatch;

using Microsoft.AspNetCore.Mvc;  // ���� MVC
using Microsoft.Extensions.DependencyInjection; // IServiceCollection ��չ����
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("DefaultConnection")));


// 2. ע�� CheckInService
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

// 3. ��ӿ���������
builder.Services.AddControllers();

// 4. ���� Swagger��API�ĵ������������Ƽ�������
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 5. �м������
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
// Ĭ��ҳ����ת�� Swagger
app.MapGet("/", context =>
{
    context.Response.Redirect("/swagger");
    return Task.CompletedTask;
});
app.UseAuthorization();

app.MapControllers();

app.Run();
