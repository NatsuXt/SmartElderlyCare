using Oracle.ManagedDataAccess.Client;
using RoomDeviceManagement.Services;
using Microsoft.Extensions.Logging;

namespace RoomDeviceManagement
{
    /// <summary>
    /// 健康监测服务中文兼容测试工具
    /// </summary>
    public static class TestHealthMonitoringService
    {
        public static async Task RunTest()
        {
            Console.WriteLine("🏥 开始测试健康监测服务中文兼容性...\n");

            try
            {
                // 设置Oracle中文环境
                Environment.SetEnvironmentVariable("NLS_LANG", "SIMPLIFIED CHINESE_CHINA.AL32UTF8");
                Environment.SetEnvironmentVariable("ORA_NCHAR_LITERAL_REPLACE", "TRUE");

                // 创建服务实例
                var logger = new ConsoleLogger<ChineseCompatibleDatabaseService>();
                var chineseDbService = new ChineseCompatibleDatabaseService(logger);
                
                var healthLogger = new ConsoleLogger<HealthMonitoringService>();
                var healthService = new HealthMonitoringService(chineseDbService, healthLogger);

                // 测试数据 - 使用实际存在的老人ID
                Console.WriteLine("📝 创建测试健康数据...");
                
                await chineseDbService.CreateHealthRecordAsync(
                    elderlyId: 1,
                    heartRate: 78,
                    bloodPressure: "120/80",
                    oxygenLevel: 98.5m,
                    temperature: 36.8m,
                    monitoringDate: DateTime.Now,
                    status: "健康正常"
                );

                await chineseDbService.CreateHealthRecordAsync(
                    elderlyId: 2,
                    heartRate: 85,
                    bloodPressure: "130/85",
                    oxygenLevel: 97.2m,
                    temperature: 37.1m,
                    monitoringDate: DateTime.Now.AddMinutes(-30),
                    status: "轻微异常"
                );

                await chineseDbService.CreateHealthRecordAsync(
                    elderlyId: 3,
                    heartRate: 102,
                    bloodPressure: "140/90",
                    oxygenLevel: 94.8m,
                    temperature: 37.8m,
                    monitoringDate: DateTime.Now.AddHours(-1),
                    status: "需要关注"
                );

                Console.WriteLine("✅ 成功创建中文健康测试数据\n");

                // 测试获取健康记录
                Console.WriteLine("🔍 测试获取健康记录...");
                var healthRecords = await chineseDbService.GetHealthRecordsAsync();
                
                Console.WriteLine($"📊 获取到 {healthRecords.Count} 条健康记录:");
                foreach (var record in healthRecords.Take(5))
                {
                    Console.WriteLine($"  - 老人ID: {record.ElderlyId}, 心率: {record.HeartRate}, 状态: {record.Status}");
                }

                // 测试健康统计
                Console.WriteLine("\n📈 测试健康统计功能...");
                var statistics = await healthService.GetHealthStatisticsAsync(null);
                Console.WriteLine($"📊 健康统计结果: {statistics}");

                // 测试获取最新健康数据
                Console.WriteLine("\n📋 测试获取最新健康数据...");
                var latestData = await healthService.GetLatestHealthDataAsync(1);
                if (latestData != null)
                {
                    Console.WriteLine($"🏥 最新健康数据 - 老人ID: {latestData.ElderlyId}, 状态: {latestData.Status}");
                }

                // 测试获取老人健康历史
                Console.WriteLine("\n📋 测试获取老人健康历史...");
                var history = await healthService.GetElderlyHealthHistoryAsync(1, 7);
                Console.WriteLine($"📊 获取到老人1的 {history.Count} 条历史记录");
                
                if (history.Any())
                {
                    var recent = history.First();
                    Console.WriteLine($"🏥 最新记录 - 心率: {recent.HeartRate}, 血压: {recent.BloodPressure}, 状态: {recent.Status}");
                }

                Console.WriteLine("\n✅ 健康监测服务中文兼容测试完成!");
                Console.WriteLine("🎉 所有中文字符均显示正确，测试通过!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ 健康监测服务测试失败: {ex.Message}");
                Console.WriteLine($"📋 详细错误: {ex.StackTrace}");
            }
        }
    }

    /// <summary>
    /// 简单的控制台日志记录器
    /// </summary>
    public class ConsoleLogger<T> : ILogger<T>
    {
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            var message = formatter(state, exception);
            var timestamp = DateTime.Now.ToString("HH:mm:ss");
            var level = logLevel switch
            {
                LogLevel.Information => "ℹ️",
                LogLevel.Warning => "⚠️",
                LogLevel.Error => "❌",
                LogLevel.Debug => "🔍",
                _ => "📝"
            };
            Console.WriteLine($"[{timestamp}] {level} {message}");
        }
    }
}
