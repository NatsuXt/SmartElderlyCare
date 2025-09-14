using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 强制设置端口 - 在构建器阶段设置
builder.WebHost.UseUrls("http://0.0.0.0:9000", "https://0.0.0.0:9443");

// Add services to the container.
builder.Services.AddControllers();

// 配置CORS策略，允许跨域请求
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// 配置数据库上下文
builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("OracleDb");
    options.UseOracle(connectionString);
});

// 注册操作日志与系统公告模块的服务
builder.Services.AddScoped<OperationLogService>();
builder.Services.AddScoped<SystemAnnouncementService>();
builder.Services.AddScoped<VisitorRegistrationService>();
builder.Services.AddScoped<VisitorLoginService>();

// 配置Swagger/OpenAPI文档
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { 
        Title = "智慧养老系统API文档", 
        Version = "v1",
        Description = "包含访客登录、系统公告、操作日志、访客注册等功能模块的完整API文档"
    });
    
    // 自动加载XML注释文档
    var assemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
    var xmlFile = $"{assemblyName}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile)
    
    // 检查当前目录下的所有XML文件
    var xmlFiles = Directory.GetFiles(AppContext.BaseDirectory, "*.xml");
    
    if (File.Exists(xmlPath))
    {
        try
        {
            options.IncludeXmlComments(xmlPath, true);
            Console.WriteLine("XML文档加载成功");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"XML文档加载失败: {ex.Message}");
        }
    }
    else
    {
        Console.WriteLine("XML文档未找到");
    }
    
    // 启用注释显示
    options.EnableAnnotations();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
// 开发环境显示详细错误信息
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// 启用CORS - 必须在其他中间件之前
app.UseCors("AllowAll");

// 生产环境也开放Swagger
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "操作日志与系统公告模块 API v1");
    options.RoutePrefix = "swagger"; // 访问路径 /swagger
});




app.UseAuthorization();

app.MapControllers();

// 确保数据库和表存在
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        // 检查数据库连接是否正常（不创建表）
        var canConnect = context.Database.CanConnect();
        Console.WriteLine($"数据库连接状态: {(canConnect ? "正常" : "异常")}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"数据库连接检查失败: {ex.Message}");
    }
}

Console.WriteLine("应用程序启动中...");
Console.WriteLine("访问地址:");
Console.WriteLine("  HTTP:  http://localhost:9000");
Console.WriteLine("  Swagger: http://localhost:9000/swagger");
Console.WriteLine("  外部访问: http://47.96.238.102:9000");
Console.WriteLine("  CORS已启用，允许跨域请求");

app.Run();
