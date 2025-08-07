using RoomDeviceManagement.Services;
using RoomDeviceManagement;

var builder = WebApplication.CreateBuilder(args);

// 添加服务到容器
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 注册服务
builder.Services.AddScoped<DatabaseService>();
builder.Services.AddScoped<ElectronicFenceService>();
builder.Services.AddScoped<HealthMonitoringService>();
builder.Services.AddScoped<IoTMonitoringService>();

// 注册后台服务
builder.Services.AddHostedService<DeviceMonitoringBackgroundService>();

// 注册数据管理相关服务
builder.Services.AddScoped<RoomManagementService>();
builder.Services.AddScoped<DeviceManagementService>();
// FenceManagementService 和 FenceLogService 已合并到 ElectronicFenceService

// 添加CORS支持
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// 配置HTTP请求管道
// 在所有环境中启用Swagger以便测试API
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors();
// app.UseHttpsRedirection(); // 暂时禁用HTTPS重定向便于测试
app.UseAuthorization();
app.MapControllers();

Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.WriteLine("===========================================");
Console.WriteLine("智慧养老系统 - 房间与设备管理模块");
Console.WriteLine("===========================================");
Console.WriteLine();

// 检查启动参数
if (args.Length > 0 && args[0] == "--test-db")
{
    // 运行数据库调试测试
    DatabaseDebugger.TestMultipleConnections();
    return;
}
if (args.Length > 0 && args[0] == "--debug-db")
{
    // 运行数据库调试
    await DatabaseDebugger.TestNetworkConnection();
    DatabaseDebugger.TestMultipleConnections();
    return;
}

// 测试数据库连接
Console.WriteLine("🔗 测试数据库连接...");
try
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    if (string.IsNullOrEmpty(connectionString))
    {
        Console.WriteLine("❌ 数据库连接字符串未配置！");
    }
    else
    {
        var dbService = new DatabaseService(connectionString);
        if (dbService.TestConnection())
        {
            Console.WriteLine("✅ 数据库连接成功！");
            Console.WriteLine($"📡 连接服务器：47.96.238.102:1521/orcl");
            Console.WriteLine($"👤 用户名：application_user");
        }
        else
        {
            Console.WriteLine("❌ 数据库连接失败！");
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine($"❌ 数据库连接失败！错误: {ex.Message}");
}

Console.WriteLine();
Console.WriteLine("🚀 智慧养老系统 - 房间与设备管理模块 API 服务已启动");
Console.WriteLine("📍 API文档地址：http://localhost:5000/swagger");
Console.WriteLine();
Console.WriteLine("📌 IoT监控业务 API：");
Console.WriteLine("   设备监控：/api/DeviceMonitoring/*");
Console.WriteLine("   电子围栏：/api/ElectronicFence/*");
Console.WriteLine("   健康监测：/api/HealthMonitoring/*");
Console.WriteLine();
Console.WriteLine("📌 数据管理 API：");
Console.WriteLine("   房间管理：/api/RoomManagement/*");
Console.WriteLine("   设备管理：/api/DeviceManagement/*");
Console.WriteLine("   围栏管理：/api/FenceManagement/*");
Console.WriteLine("   围栏日志：/api/FenceLog/*");
Console.WriteLine("   健康数据：/api/HealthData/*");
Console.WriteLine();
app.Run();
