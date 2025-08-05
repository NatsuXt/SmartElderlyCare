using RoomDeviceManagement.Services;

var builder = WebApplication.CreateBuilder(args);

// 添加服务到容器
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 注册服务
builder.Services.AddScoped<DatabaseService>();
builder.Services.AddScoped<DeviceMonitoringService>();
builder.Services.AddScoped<ElectronicFenceService>();
builder.Services.AddScoped<HealthMonitoringService>();

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
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.WriteLine("===========================================");
Console.WriteLine("智慧养老系统 - IoT监控平台 API 服务");
Console.WriteLine("===========================================");
Console.WriteLine();

// 测试数据库连接
var dbService = new DatabaseService();
Console.WriteLine("🔗 测试数据库连接...");
if (dbService.TestConnection())
{
    Console.WriteLine("✅ 数据库连接成功！");
    Console.WriteLine($"📡 连接服务器：47.96.238.102:1521/orcl");
    Console.WriteLine($"👤 用户名：FIBRE");
}
else
{
    Console.WriteLine("❌ 数据库连接失败！");
}

Console.WriteLine();
Console.WriteLine("🚀 智慧养老系统 - 核心业务逻辑 API 服务已启动");
Console.WriteLine("📍 API文档地址：http://localhost:5000/swagger");
Console.WriteLine();
Console.WriteLine("📌 设备监控 API端点：");
Console.WriteLine("   GET  /api/DeviceMonitoring/poll-status      - 轮询设备状态");
Console.WriteLine("   POST /api/DeviceMonitoring/fault-report     - 设备故障上报");
Console.WriteLine("   GET  /api/DeviceMonitoring/{id}/status      - 获取设备详情");
Console.WriteLine("   POST /api/DeviceMonitoring/sync             - 同步设备状态");
Console.WriteLine();
Console.WriteLine("📌 电子围栏 API端点：");
Console.WriteLine("   POST /api/ElectronicFence/gps-report        - GPS位置上报");
Console.WriteLine("   GET  /api/ElectronicFence/logs              - 围栏进出记录");
Console.WriteLine("   GET  /api/ElectronicFence/current-status    - 当前位置状态");
Console.WriteLine("   GET  /api/ElectronicFence/alerts            - 围栏警报");
Console.WriteLine();
Console.WriteLine("📌 健康监测 API端点：");
Console.WriteLine("   POST /api/HealthMonitoring/data-report      - 健康数据上报");
Console.WriteLine("   GET  /api/HealthMonitoring/elderly/{id}/history - 健康历史数据");
Console.WriteLine("   GET  /api/HealthMonitoring/statistics        - 健康统计");
Console.WriteLine("   GET  /api/HealthMonitoring/alerts           - 健康警报");
Console.WriteLine();
app.Run();
Console.WriteLine();
app.Run();
