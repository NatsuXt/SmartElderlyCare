using RoomDeviceManagement.Services;
using RoomDeviceManagement;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

// 🔧 在程序最开始设置Oracle 19c中文字符环境变量 - 确保在任何Oracle连接之前生效
Environment.SetEnvironmentVariable("NLS_LANG", "SIMPLIFIED CHINESE_CHINA.AL32UTF8");
Environment.SetEnvironmentVariable("ORA_NCHAR_LITERAL_REPLACE", "TRUE");
Console.WriteLine("✅ Oracle 19c 中文字符环境变量已在程序启动时设置");

// 🔧 Oracle 19c 中文字符环境初始化（附加确保）
Oracle19cChineseTestHelper.InitializeOracleEnvironment();

// 🔍 检查是否要运行诊断工具
if (args.Length > 0 && args[0] == "diagnose")
{
    await RoomDeviceManagement.Services.ChineseDiagnosticTool.RunFullDiagnostic();
    Console.WriteLine("\n按任意键退出...");
    Console.ReadKey();
    return;
}

// 🔍 检查是否要测试中文兼容服务
if (args.Length > 0 && args[0] == "test-chinese")
{
    await RoomDeviceManagement.TestChineseService.RunTest();
    return;
}

// 🔍 检查是否要测试健康监测服务
if (args.Length > 0 && args[0] == "test-health")
{
    await RoomDeviceManagement.TestHealthMonitoringService.RunTest();
    return;
}

// 🔍 检查是否要测试电子围栏服务
if (args.Length > 0 && args[0] == "test-fence")
{
    await RoomDeviceManagement.TestElectronicFenceService.RunTest();
    return;
}

// 🔍 检查是否要运行中文编码诊断
if (args.Length > 0 && args[0] == "diagnose-encoding")
{
    await RoomDeviceManagement.ChineseEncodingDiagnostic.RunDiagnostic();
    Console.WriteLine("\n按任意键退出...");
    Console.ReadKey();
    return;
}

// 🔍 检查是否要修复乱码数据
if (args.Length > 0 && args[0] == "fix-encoding")
{
    await RoomDeviceManagement.ChineseEncodingDiagnostic.FixGarbledData();
    Console.WriteLine("\n按任意键退出...");
    Console.ReadKey();
    return;
}

// 🔍 检查是否要修复中文数据
if (args.Length > 0 && args[0] == "repair-chinese")
{
    await ChineseDataRepairTool.RepairChineseData();
    Console.WriteLine("\n按任意键退出...");
    Console.ReadKey();
    return;
}

// 🔍 检查围栏表结构
if (args.Length > 0 && args[0] == "check-fence-tables")
{
    await FenceTableCheck.CheckFenceTableStructure();
    Console.WriteLine("\n按任意键退出...");
    Console.ReadKey();
    return;
}

// 🏥 检查是否要测试健康监测服务
if (args.Length > 0 && args[0] == "test-health")
{
    await RoomDeviceManagement.TestHealthMonitoringService.RunTest();
    return;
}

var builder = WebApplication.CreateBuilder(args);

// 添加服务到容器
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // 🔧 配置JSON序列化以正确处理中文字符
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
        options.JsonSerializerOptions.WriteIndented = true;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 注册服务
builder.Services.AddScoped<DatabaseService>();
builder.Services.AddScoped<ChineseCompatibleDatabaseService>(); // 🆕 新增中文兼容数据库服务
builder.Services.AddScoped<ElectronicFenceService>();
builder.Services.AddScoped<HealthMonitoringService>();
// IoTMonitoringService 已移除，功能迁移到 DeviceManagementService

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
Console.WriteLine("📌 主要业务 API 模块：");
Console.WriteLine("   设备管理：/api/DeviceManagement/* (6个端点)");
Console.WriteLine("   房间管理：/api/RoomManagement/* (6个端点)");
Console.WriteLine("   健康监测：/api/HealthMonitoring/* (5个端点)");
Console.WriteLine("   电子围栏：/api/ElectronicFence/* (11个端点)");
Console.WriteLine("   IoT监控：/api/IoTMonitoring/* (5个端点)");
Console.WriteLine();
Console.WriteLine("📌 核心功能简介：");
Console.WriteLine("   🏠 房间管理：房间信息CRUD、容量统计、状态管理");
Console.WriteLine("   📱 设备管理：设备监控、故障检测、维护记录");
Console.WriteLine("   💓 健康监测：生命体征采集、数据上报、历史记录");
Console.WriteLine("   🔒 电子围栏：GPS追踪、越界警报、活动轨迹");
Console.WriteLine("   🌐 IoT监控：设备轮询、故障上报、状态同步");
Console.WriteLine();
Console.WriteLine("📊 后台服务：设备状态自动轮询检查 (5分钟间隔)");
Console.WriteLine();
app.Run();
