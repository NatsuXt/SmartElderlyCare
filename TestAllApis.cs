using System.Text;
using System.Text.Json;
using System.Net.Http;
using System.Net.Http.Json;

namespace RoomDeviceManagement
{
    /// <summary>
    /// 全面API端点测试工具
    /// 检查每个Controller的每个端点是否正常工作
    /// </summary>
    public class TestAllApis
    {
        private static readonly HttpClient client = new HttpClient();
        private static readonly string baseUrl = "http://localhost:5000/api";
        
        public static async Task Main(string[] args)
        {
            Console.WriteLine("🚀 开始全面测试智慧养老系统所有API端点");
            Console.WriteLine("=" + new string('=', 60));

            try
            {
                // 1. 测试设备管理API
                await TestDeviceManagementApis();
                
                // 2. 测试房间管理API  
                await TestRoomManagementApis();
                
                // 3. 测试健康监测API
                await TestHealthMonitoringApis();
                
                // 4. 测试电子围栏API
                await TestElectronicFenceApis();

                Console.WriteLine("\n🎉 所有API测试完成！");
                Console.WriteLine("💡 如果以上测试中有错误，请检查相应的Controller和Service");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ 测试过程中发生错误: {ex.Message}");
                Console.WriteLine($"详细错误: {ex}");
            }

            Console.WriteLine("\n按任意键退出...");
            Console.ReadKey();
        }

        // 测试设备管理API
        static async Task TestDeviceManagementApis()
        {
            Console.WriteLine("\n📱 测试设备管理API");
            Console.WriteLine("-" + new string('-', 40));

            // GET /api/DeviceManagement (获取设备列表)
            await TestGetRequest("/DeviceManagement?page=1&pageSize=10", "获取设备列表");

            // GET /api/DeviceManagement/statistics (设备统计)
            await TestGetRequest("/DeviceManagement/statistics", "设备统计信息");

            // GET /api/DeviceManagement/1 (获取单个设备)
            await TestGetRequest("/DeviceManagement/1", "获取设备详情(ID=1)");

            // GET /api/DeviceManagement/poll-status (轮询设备状态)
            await TestGetRequest("/DeviceManagement/poll-status", "轮询设备状态");

            // POST /api/DeviceManagement (创建设备)
            var createDeviceDto = new
            {
                DeviceName = "测试设备API",
                DeviceType = "API测试设备",
                Location = "测试位置",
                Status = "正常"
            };
            await TestPostRequest("/DeviceManagement", createDeviceDto, "创建新设备");

            // POST /api/DeviceManagement/fault-report (故障上报)
            var faultReportDto = new
            {
                DeviceId = 1,
                DeviceType = "智能床垫",
                FaultStatus = "故障",
                FaultDescription = "API测试故障",
                ReportTime = DateTime.Now
            };
            await TestPostRequest("/DeviceManagement/fault-report", faultReportDto, "设备故障上报");

            // POST /api/DeviceManagement/sync (手动同步)
            await TestPostRequest("/DeviceManagement/sync", null, "手动同步设备状态");
        }

        // 测试房间管理API
        static async Task TestRoomManagementApis()
        {
            Console.WriteLine("\n🏠 测试房间管理API");
            Console.WriteLine("-" + new string('-', 40));

            // GET /api/RoomManagement (获取房间列表)
            await TestGetRequest("/RoomManagement?page=1&pageSize=10", "获取房间列表");

            // GET /api/RoomManagement/statistics (房间统计)
            await TestGetRequest("/RoomManagement/statistics", "房间统计信息");

            // GET /api/RoomManagement/1 (获取单个房间)
            await TestGetRequest("/RoomManagement/1", "获取房间详情(ID=1)");

            // POST /api/RoomManagement (创建房间)
            var createRoomDto = new
            {
                RoomNumber = "API-TEST-001",
                RoomType = "API测试房间",
                BedCount = 1,
                Status = "可用",
                BedType = "单人床"
            };
            await TestPostRequest("/RoomManagement", createRoomDto, "创建新房间");
        }

        // 测试健康监测API
        static async Task TestHealthMonitoringApis()
        {
            Console.WriteLine("\n💓 测试健康监测API");
            Console.WriteLine("-" + new string('-', 40));

            // GET /api/HealthMonitoring/statistics (健康统计)
            await TestGetRequest("/HealthMonitoring/statistics", "健康监测统计");

            // GET /api/HealthMonitoring/elderly/1/latest (最新健康数据)
            await TestGetRequest("/HealthMonitoring/elderly/1/latest", "获取老人最新健康数据");

            // GET /api/HealthMonitoring/elderly/1/history (健康历史)
            await TestGetRequest("/HealthMonitoring/elderly/1/history", "获取老人健康历史");

            // POST /api/HealthMonitoring/report (健康数据上报)
            var healthReportDto = new
            {
                ElderlyId = 1,
                DeviceId = 1,
                HeartRate = 75,
                BloodPressure = "120/80",
                Temperature = 36.5,
                OxygenLevel = 98,
                MonitoringDate = DateTime.Now
            };
            await TestPostRequest("/HealthMonitoring/report", healthReportDto, "健康数据上报");
        }

        // 测试电子围栏API
        static async Task TestElectronicFenceApis()
        {
            Console.WriteLine("\n🔒 测试电子围栏API");
            Console.WriteLine("-" + new string('-', 40));

            // GET /api/ElectronicFence/elderly/location-status (位置状态)
            await TestGetRequest("/ElectronicFence/elderly/location-status", "获取老人位置状态");

            // GET /api/ElectronicFence/alerts (围栏警报)
            await TestGetRequest("/ElectronicFence/alerts", "获取围栏警报");

            // GET /api/ElectronicFence/elderly/1/trajectory (老人轨迹)
            await TestGetRequest("/ElectronicFence/elderly/1/trajectory?hours=24", "获取老人轨迹");

            // POST /api/ElectronicFence/gps-report (GPS位置上报)
            var gpsReportDto = new
            {
                ElderlyId = 1,
                Latitude = 31.230391,
                Longitude = 121.473701,
                LocationTime = DateTime.Now
            };
            await TestPostRequest("/ElectronicFence/gps-report", gpsReportDto, "GPS位置上报");
        }

        // 测试GET请求
        static async Task TestGetRequest(string endpoint, string description)
        {
            try
            {
                Console.Write($"  🔍 {description}: ");
                var response = await client.GetAsync($"{baseUrl}{endpoint}");
                var content = await response.Content.ReadAsStringAsync();
                
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("✅ 成功");
                    
                    // 尝试解析JSON以检查格式
                    try
                    {
                        var jsonDoc = JsonDocument.Parse(content);
                        if (jsonDoc.RootElement.TryGetProperty("success", out var successProp))
                        {
                            var success = successProp.GetBoolean();
                            if (!success && jsonDoc.RootElement.TryGetProperty("message", out var msgProp))
                            {
                                Console.WriteLine($"    ⚠️ 业务失败: {msgProp.GetString()}");
                            }
                        }
                    }
                    catch
                    {
                        // JSON解析失败也是可以接受的
                    }
                }
                else
                {
                    Console.WriteLine($"❌ HTTP {response.StatusCode}");
                    if (!string.IsNullOrEmpty(content))
                    {
                        Console.WriteLine($"    错误信息: {content.Substring(0, Math.Min(200, content.Length))}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ 异常: {ex.Message}");
            }
        }

        // 测试POST请求
        static async Task TestPostRequest(string endpoint, object? data, string description)
        {
            try
            {
                Console.Write($"  📤 {description}: ");
                
                HttpResponseMessage response;
                if (data != null)
                {
                    var json = JsonSerializer.Serialize(data);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    response = await client.PostAsync($"{baseUrl}{endpoint}", content);
                }
                else
                {
                    response = await client.PostAsync($"{baseUrl}{endpoint}", null);
                }
                
                var responseContent = await response.Content.ReadAsStringAsync();
                
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("✅ 成功");
                    
                    // 尝试解析JSON以检查格式
                    try
                    {
                        var jsonDoc = JsonDocument.Parse(responseContent);
                        if (jsonDoc.RootElement.TryGetProperty("success", out var successProp))
                        {
                            var success = successProp.GetBoolean();
                            if (!success && jsonDoc.RootElement.TryGetProperty("message", out var msgProp))
                            {
                                Console.WriteLine($"    ⚠️ 业务失败: {msgProp.GetString()}");
                            }
                        }
                    }
                    catch
                    {
                        // JSON解析失败也是可以接受的
                    }
                }
                else
                {
                    Console.WriteLine($"❌ HTTP {response.StatusCode}");
                    if (!string.IsNullOrEmpty(responseContent))
                    {
                        Console.WriteLine($"    错误信息: {responseContent.Substring(0, Math.Min(200, responseContent.Length))}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ 异常: {ex.Message}");
            }
        }
    }
}
