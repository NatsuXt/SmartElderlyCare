using RoomDeviceManagement.Services;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using Oracle.ManagedDataAccess.Client;

// 🔧 Oracle 19c 中文字符环境初始化 - 确保中文字符正确支持
void InitializeChineseCharacterSupport()
{
    try
    {
        // 设置控制台编码为UTF-8
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.InputEncoding = System.Text.Encoding.UTF8;
        
        // 设置Oracle环境变量以支持中文字符
        Environment.SetEnvironmentVariable("NLS_LANG", "SIMPLIFIED CHINESE_CHINA.AL32UTF8");
        Environment.SetEnvironmentVariable("ORA_NCHAR_LITERAL_REPLACE", "TRUE");
        Environment.SetEnvironmentVariable("NLS_NCHAR", "AL32UTF8");
        
        Console.WriteLine("✅ Oracle 19c 中文字符环境初始化完成");
        Console.WriteLine($"📝 NLS_LANG: {Environment.GetEnvironmentVariable("NLS_LANG")}");
        Console.WriteLine($"📝 ORA_NCHAR_LITERAL_REPLACE: {Environment.GetEnvironmentVariable("ORA_NCHAR_LITERAL_REPLACE")}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Oracle环境初始化失败: {ex.Message}");
    }
}

// 在程序最开始初始化中文字符支持
InitializeChineseCharacterSupport();

// 🔍 API测试模式 - 简化版本
if (args.Length > 0 && args[0] == "test-api")
{
    await TestAllApis();
    Console.WriteLine("\n按任意键退出...");
    Console.ReadKey();
    return;
}

// API测试函数
async Task TestAllApis()
{
    Console.WriteLine("🧪 开始API完整测试...");
    var client = new HttpClient();
    
    try
    {
        // 测试房间管理API
        Console.WriteLine("\n🏠 测试房间管理API");
        
        // 创建中文房间
        var roomData = new {
            RoomNumber = $"中文房间-{DateTime.Now:mmss}",
            RoomType = "标准间",
            Capacity = 2,
            Status = "空闲",
            Rate = 200.00m,
            BedType = "单人床",
            Floor = 3
        };
        
        var roomJson = System.Text.Json.JsonSerializer.Serialize(roomData);
        var roomContent = new StringContent(roomJson, System.Text.Encoding.UTF8, "application/json");
        var roomResponse = await client.PostAsync("http://localhost:5000/api/RoomManagement/rooms", roomContent);
        
        if (roomResponse.IsSuccessStatusCode)
        {
            var result = await roomResponse.Content.ReadAsStringAsync();
            Console.WriteLine($"✅ 房间创建成功: {result}");
        }
        else
        {
            Console.WriteLine($"❌ 房间创建失败: {roomResponse.StatusCode}");
        }
        
        // 测试设备管理API
        Console.WriteLine("\n📱 测试设备管理API");
        
        var deviceData = new {
            DeviceName = $"中文设备-{DateTime.Now:mmss}",
            DeviceType = "智能血压计",
            InstallationDate = DateTime.Now,
            Status = "正常运行",
            Location = "一楼护士站",
            Description = "支持中文的智能设备",
            LastMaintenanceDate = DateTime.Now
        };
        
        var deviceJson = System.Text.Json.JsonSerializer.Serialize(deviceData);
        var deviceContent = new StringContent(deviceJson, System.Text.Encoding.UTF8, "application/json");
        var deviceResponse = await client.PostAsync("http://localhost:5000/api/DeviceManagement/devices", deviceContent);
        
        if (deviceResponse.IsSuccessStatusCode)
        {
            var result = await deviceResponse.Content.ReadAsStringAsync();
            Console.WriteLine($"✅ 设备创建成功: {result}");
        }
        else
        {
            Console.WriteLine($"❌ 设备创建失败: {deviceResponse.StatusCode}");
        }
        
        Console.WriteLine("\n✅ API测试完成!");
        
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ API测试异常: {ex.Message}");
    }
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

// 注册核心服务 - 确保中文字符支持
builder.Services.AddScoped<ChineseCompatibleDatabaseService>(); // 🆕 中文兼容数据库服务
builder.Services.AddScoped<RoomManagementService>();
builder.Services.AddScoped<DeviceManagementService>();
builder.Services.AddScoped<ElectronicFenceService>();
builder.Services.AddScoped<HealthMonitoringService>();

// 注册后台服务
builder.Services.AddHostedService<DeviceMonitoringBackgroundService>();

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

// 🔧 配置中文字符数据库连接测试
async Task TestChineseDatabaseConnection()
{
    try
    {
        var chineseDbService = new ChineseCompatibleDatabaseService(
            Microsoft.Extensions.Logging.LoggerFactory.Create(builder => builder.AddConsole())
                .CreateLogger<ChineseCompatibleDatabaseService>());
        
        Console.WriteLine("🔗 测试中文兼容数据库连接...");
        
        // 测试获取房间列表
        var rooms = await chineseDbService.GetRoomsAsync("");
        Console.WriteLine($"✅ 成功连接数据库，获取到 {rooms.Count} 个房间");
        
        if (rooms.Count > 0)
        {
            var firstRoom = rooms[0];
            Console.WriteLine($"📋 示例房间: {firstRoom.RoomNumber} - {firstRoom.RoomType}");
        }
        
        Console.WriteLine("数据库连接成功！");
        Console.WriteLine("当前用户: APPLICATION_USER");
        Console.WriteLine("📡 连接服务器：47.96.238.102:1521/orcl");
        Console.WriteLine("👤 用户名：application_user");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ 数据库连接失败: {ex.Message}");
    }
}

Console.WriteLine("===========================================");
Console.WriteLine("智慧养老系统 - 房间与设备管理模块");
Console.WriteLine("===========================================");
Console.WriteLine();

// 测试数据库连接
await TestChineseDatabaseConnection();

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
