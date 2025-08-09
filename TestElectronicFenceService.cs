using RoomDeviceManagement.Services;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Collections.Generic;
using System;

namespace RoomDeviceManagement
{
    /// <summary>
    /// 电子围栏服务中文兼容性测试工具
    /// 验证转换后的 ElectronicFenceService 是否能正确处理中文字符
    /// </summary>
    public static class TestElectronicFenceService
    {
        public static async Task RunTest()
        {
            Console.WriteLine("🎯 开始电子围栏服务中文兼容性测试");
            Console.WriteLine("=" + new string('=', 50));

            // 创建日志记录器
            var loggerFactory = LoggerFactory.Create(builder =>
                builder.AddConsole().SetMinimumLevel(LogLevel.Information));
            
            var dbLogger = loggerFactory.CreateLogger<ChineseCompatibleDatabaseService>();
            var serviceLogger = loggerFactory.CreateLogger<ElectronicFenceService>();

            try
            {
                // 创建服务实例
                var chineseDbService = new ChineseCompatibleDatabaseService(dbLogger);
                var fenceService = new ElectronicFenceService(chineseDbService, serviceLogger);

                Console.WriteLine("\n📋 测试1: 获取老人围栏状态");
                Console.WriteLine("-" + new string('-', 40));
                
                var locationStatus = await fenceService.GetElderlyLocationStatusAsync();
                Console.WriteLine($"📊 获取到 {locationStatus.Count} 条围栏状态记录:");
                
                foreach (var status in locationStatus.Take(5))  // 只显示前5条
                {
                    var dynamicStatus = (dynamic)status;
                    Console.WriteLine($"  老人ID: {dynamicStatus.ElderlyId}, " +
                                      $"姓名: {dynamicStatus.Name}, " +
                                      $"围栏ID: {dynamicStatus.CurrentFenceId}, " +
                                      $"状态: {dynamicStatus.Status}");
                }

                Console.WriteLine("\n🚨 测试2: 获取围栏警报信息");
                Console.WriteLine("-" + new string('-', 40));
                
                var alerts = await fenceService.GetFenceAlertsAsync(true);
                Console.WriteLine($"🚨 获取到 {alerts.Count} 条围栏警报:");
                
                foreach (var alert in alerts.Take(3))  // 只显示前3条
                {
                    var dynamicAlert = (dynamic)alert;
                    Console.WriteLine($"  老人ID: {dynamicAlert.ElderlyId}, " +
                                      $"围栏ID: {dynamicAlert.FenceId}, " +
                                      $"警报类型: {dynamicAlert.AlertType}, " +
                                      $"状态: {dynamicAlert.AlertStatus}");
                }

                Console.WriteLine("\n🗺️ 测试3: 获取老人轨迹信息");
                Console.WriteLine("-" + new string('-', 40));
                
                if (locationStatus.Count > 0)
                {
                    var firstElderly = ((dynamic)locationStatus.First()).ElderlyId;
                    var trajectory = await fenceService.GetElderlyTrajectoryAsync(firstElderly, 24);
                    Console.WriteLine($"🗺️ 老人 {firstElderly} 的24小时轨迹记录: {trajectory.Count} 条");
                    
                    foreach (var track in ((IEnumerable<object>)trajectory).Take(3))  // 只显示前3条
                    {
                        var dynamicTrack = (dynamic)track;
                        Console.WriteLine($"  围栏ID: {dynamicTrack.FenceId}, " +
                                          $"进入时间: {dynamicTrack.EntryTime:yyyy-MM-dd HH:mm}, " +
                                          $"状态: {dynamicTrack.Status}");
                    }
                }

                Console.WriteLine("\n✅ 所有测试完成！");
                Console.WriteLine("💡 如果以上信息中的中文字符显示正常（不是???），则转换成功！");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ 测试过程中发生错误: {ex.Message}");
                Console.WriteLine($"详细错误: {ex}");
            }

            Console.WriteLine("\n按任意键退出...");
            Console.ReadKey();
        }
    }
}
