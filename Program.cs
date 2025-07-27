using RoomDeviceManagement.Services;
using RoomDeviceManagement.Models;
using System;
using System.Data;

namespace RoomDeviceManagement
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("===========================================");
            Console.WriteLine("智慧养老系统 - 房间设备管理模块");
            Console.WriteLine("===========================================");
            Console.WriteLine();

            var dbService = new DatabaseService();

            // 1. 测试数据库连接
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
                Console.WriteLine("请检查：");
                Console.WriteLine("- 网络连接是否正常");
                Console.WriteLine("- Oracle数据库服务是否启动");
                Console.WriteLine("- 连接参数是否正确");
                return;
            }
            Console.WriteLine();

            // 2. 检查数据库表状态
            Console.WriteLine("📊 检查数据库表状态...");
            var tableStatus = dbService.CheckTablesExist();
            foreach (var table in tableStatus)
            {
                string status = table.Value ? "✅ 存在" : "❌ 不存在";
                Console.WriteLine($"   {table.Key}: {status}");
            }
            Console.WriteLine();

            // 3. 查询系统数据概览
            Console.WriteLine("📈 系统数据概览：");
            try
            {
                // 房间管理数据
                var roomQuery = "SELECT COUNT(*) FROM RoomManagement";
                var roomResult = dbService.ExecuteQuery(roomQuery);
                if (roomResult.Rows.Count > 0)
                {
                    Console.WriteLine($"   🏠 房间总数：{roomResult.Rows[0][0]}");
                }

                // 老人信息数据
                var elderlyQuery = "SELECT COUNT(*) FROM ElderlyInfo";
                var elderlyResult = dbService.ExecuteQuery(elderlyQuery);
                if (elderlyResult.Rows.Count > 0)
                {
                    Console.WriteLine($"   👴 老人总数：{elderlyResult.Rows[0][0]}");
                }

                // 设备状态数据
                var deviceQuery = "SELECT COUNT(*) FROM DeviceStatus";
                var deviceResult = dbService.ExecuteQuery(deviceQuery);
                if (deviceResult.Rows.Count > 0)
                {
                    Console.WriteLine($"   📱 设备总数：{deviceResult.Rows[0][0]}");
                }

                // 健康监测数据
                var healthQuery = "SELECT COUNT(*) FROM HealthMonitoring";
                var healthResult = dbService.ExecuteQuery(healthQuery);
                if (healthResult.Rows.Count > 0)
                {
                    Console.WriteLine($"   💓 健康记录：{healthResult.Rows[0][0]}条");
                }

                // 电子围栏数据
                var fenceQuery = "SELECT COUNT(*) FROM ElectronicFence";
                var fenceResult = dbService.ExecuteQuery(fenceQuery);
                if (fenceResult.Rows.Count > 0)
                {
                    Console.WriteLine($"   🚧 电子围栏：{fenceResult.Rows[0][0]}个");
                }

                // 围栏日志数据
                var logQuery = "SELECT COUNT(*) FROM FenceLog";
                var logResult = dbService.ExecuteQuery(logQuery);
                if (logResult.Rows.Count > 0)
                {
                    Console.WriteLine($"   📋 围栏日志：{logResult.Rows[0][0]}条");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ 查询数据时发生错误：{ex.Message}");
            }
            Console.WriteLine();

            // 4. 显示房间状态
            Console.WriteLine("🏠 房间状态详情：");
            try
            {
                var roomStatusQuery = @"
                    SELECT room_number, room_type, status, capacity, rate, floor_num 
                    FROM RoomManagement 
                    ORDER BY floor_num, room_number";
                
                var roomStatusResult = dbService.ExecuteQuery(roomStatusQuery);
                if (roomStatusResult.Rows.Count > 0)
                {
                    Console.WriteLine("   房间号   | 类型   | 状态   | 容量 | 收费   | 楼层");
                    Console.WriteLine("   -------- | ------ | ------ | ---- | ------ | ----");
                    
                    foreach (DataRow row in roomStatusResult.Rows)
                    {
                        var status = row["status"]?.ToString() ?? "";
                        var chineseStatus = status switch
                        {
                            "Available" => "空闲",
                            "Occupied" => "已入住", 
                            "Maintenance" => "维护中",
                            "Cleaning" => "清洁中",
                            _ => status
                        };
                        Console.WriteLine($"   {row["room_number"],-8} | {row["room_type"],-6} | {chineseStatus,-6} | {row["capacity"],-4} | {row["rate"],-6} | {row["floor_num"]}");
                    }
                }
                else
                {
                    Console.WriteLine("   📋 暂无房间数据");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ❌ 查询房间状态失败：{ex.Message}");
            }
            Console.WriteLine();

            // 5. 显示最新健康监测记录
            Console.WriteLine("💓 最新健康监测记录：");
            try
            {
                var healthStatusQuery = @"
                    SELECT 
                        e.name,
                        h.monitoring_date,
                        h.heart_rate,
                        h.blood_pressure,
                        h.oxygen_level,
                        h.temperature,
                        h.status
                    FROM HealthMonitoring h
                    JOIN ElderlyInfo e ON h.elderly_id = e.elderly_id
                    WHERE h.monitoring_date >= SYSDATE - 1
                    ORDER BY h.monitoring_date DESC";
                
                var healthResult = dbService.ExecuteQuery(healthStatusQuery);
                if (healthResult.Rows.Count > 0)
                {
                    Console.WriteLine("   姓名 | 监测时间         | 心率 | 血压    | 血氧  | 体温  | 状态");
                    Console.WriteLine("   ---- | ---------------- | ---- | ------- | ----- | ----- | ----");
                    
                    foreach (DataRow row in healthResult.Rows)
                    {
                        var monitoringDate = Convert.ToDateTime(row["monitoring_date"]).ToString("MM-dd HH:mm");
                        Console.WriteLine($"   {row["name"],-4} | {monitoringDate,-16} | {row["heart_rate"],-4} | {row["blood_pressure"],-7} | {row["oxygen_level"],-5} | {row["temperature"],-5} | {row["status"]}");
                    }
                }
                else
                {
                    Console.WriteLine("   📋 暂无健康监测数据");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ❌ 查询健康数据失败：{ex.Message}");
            }
            Console.WriteLine();

            // 6. 显示设备状态
            Console.WriteLine("📱 设备状态概览：");
            try
            {
                var deviceStatusQuery = @"
                    SELECT status, COUNT(*) as count 
                    FROM DeviceStatus 
                    GROUP BY status 
                    ORDER BY status";
                
                var deviceResult = dbService.ExecuteQuery(deviceStatusQuery);
                if (deviceResult.Rows.Count > 0)
                {
                    foreach (DataRow row in deviceResult.Rows)
                    {
                        var englishStatus = row["status"].ToString() ?? "";
                        var chineseStatus = englishStatus switch
                        {
                            "Normal" => "正常",
                            "Fault" => "故障", 
                            "Maintenance" => "维护中",
                            "Deactivated" => "停用",
                            _ => englishStatus
                        };
                        var statusIcon = englishStatus switch
                        {
                            "Normal" => "✅",
                            "Fault" => "❌",
                            "Maintenance" => "🔧", 
                            "Deactivated" => "⏸️",
                            _ => "❓"
                        };
                        Console.WriteLine($"   {statusIcon} {chineseStatus}：{row["count"]}台");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ❌ 查询设备状态失败：{ex.Message}");
            }
            Console.WriteLine();

            Console.WriteLine("===========================================");
            Console.WriteLine("系统初始化完成，所有功能模块已准备就绪！");
            Console.WriteLine("===========================================");
            
            Console.WriteLine("\n按任意键退出...");
            Console.ReadKey();
        }
    }
}
