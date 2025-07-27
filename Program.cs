using ElderlyCareManagement.Services;
using ElderlyCareManagement.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;
using ElderlyCareManagement.Mappings;

var builder = WebApplication.CreateBuilder(args);

// 添加服务到容器中
builder.Services.AddControllers();

// 配置数据库上下文
builder.Services.AddDbContext<ElderlyCareContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 注册服务
builder.Services.AddScoped<IStaffService, StaffService>();
builder.Services.AddScoped<INursingScheduleService, NursingScheduleService>();
builder.Services.AddScoped<IEmergencySosService, EmergencySosService>();
builder.Services.AddScoped<IDisinfectionService, DisinfectionService>();
builder.Services.AddScoped<INotificationService, NotificationService>();

// 配置AutoMapper
builder.Services.AddAutoMapper(typeof(StaffProfile));

// 配置Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Elderly Care Management API",
        Version = "v1",
        Description = "API for managing elderly care facility operations",
        Contact = new OpenApiContact
        {
            Name = "Support Team",
            Email = "support@elderlycare.com"
        }
    });

    // 👇 新增：按路由前缀自动分组（关键代码）
    c.TagActionsBy(api =>
    {
        // 获取路由路径（如 "api/StaffInfo/NursingPlans/Unassigned"）
        var routePath = api.RelativePath?.ToLower() ?? "";
        
        // 根据路径关键词返回不同的分组名称
        if (routePath.Contains("nursing")) return new[] { "Nursing Plans" };
        if (routePath.Contains("emergency") || routePath.Contains("sos")) return new[] { "Emergency SOS" };
        if (routePath.Contains("disinfection")) return new[] { "Disinfection" };
        return new[] { "Staff Management" }; // 默认分组
    });

    // 👇 新增：确保所有API都显示（避免分组后某些接口消失）
    c.DocInclusionPredicate((docName, apiDesc) => true);
    // 👆 新增

    // 加载XML注释
    try
    {
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(xmlPath))
        {
            c.IncludeXmlComments(xmlPath);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"加载XML注释失败: {ex.Message}");
    }
});

var app = builder.Build();

// 启用Swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Elderly Care Management API V1");
    c.RoutePrefix = "swagger";
    
    // 👇 新增：在Swagger UI顶部显示分组标签（可选）
    c.DisplayOperationId();
    c.DisplayRequestDuration();
    // 👆 新增
});

// 根路径自动跳转到/swagger
app.Use(async (context, next) =>
{
    if (context.Request.Path == "/")
    {
        context.Response.Redirect("/swagger");
        return;
    }
    await next();
});

// 开发环境额外配置
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

    // 初始化数据库
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ElderlyCareContext>();
        context.Database.EnsureCreated();
        Console.WriteLine("开发环境数据库初始化完成");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"数据库初始化错误: {ex.Message}");
    }
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();