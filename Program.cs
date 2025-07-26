using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

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

// 配置Swagger/OpenAPI文档
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "智慧养老系统API文档 - 操作日志与系统公告模块", Version = "v1" });
    // 自动加载XML注释（需在项目属性-生成-输出XML文档文件）
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        options.IncludeXmlComments(xmlPath, true);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
// 开发环境显示详细错误信息
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// 生产环境也开放Swagger（如需安全可加权限）
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "操作日志与系统公告模块 API v1");
    options.RoutePrefix = "swagger"; // 访问路径 /swagger
});

app.UseHttpsRedirection();

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

app.Run();
