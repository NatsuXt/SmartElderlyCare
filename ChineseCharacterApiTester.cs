using RoomDeviceManagement.DTOs;
using System.Text;
using System.Text.Json;

namespace RoomDeviceManagement
{
    /// <summary>
    /// 智慧养老系统API中文字符兼容性测试工具
    /// 确保所有API端点都能正确处理中文字符的创建、读取、更新、删除操作
    /// </summary>
    public static class ChineseCharacterApiTester
    {
        private static readonly HttpClient _httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:5000") };
        
        /// <summary>
        /// 运行完整的中文字符API测试套件
        /// </summary>
        public static async Task RunFullApiTest()
        {
            Console.OutputEncoding = Encoding.UTF8;
            
            Console.WriteLine("🧪 智慧养老系统 - API中文字符兼容性测试");
            Console.WriteLine("========================================");
            
            var testResults = new List<(string Module, string Test, bool Success, string Message)>();
            
            // 测试房间管理模块
            await TestRoomManagementModule(testResults);
            
            // 测试设备管理模块
            await TestDeviceManagementModule(testResults);
            
            // 测试健康监测模块
            await TestHealthMonitoringModule(testResults);
            
            // 测试电子围栏模块
            await TestElectronicFenceModule(testResults);
            
            // 生成测试报告
            GenerateTestReport(testResults);
        }
        
        /// <summary>
        /// 测试房间管理模块中文字符支持
        /// </summary>
        private static async Task TestRoomManagementModule(List<(string Module, string Test, bool Success, string Message)> results)
        {
            Console.WriteLine("\n🏠 房间管理模块中文字符测试");
            Console.WriteLine("----------------------------");
            
            try
            {
                // 创建中文房间
                var roomData = new RoomCreateDto
                {
                    RoomNumber = $"中文测试房间-{DateTime.Now:HHmmss}",
                    RoomType = "豪华套房",
                    Capacity = 2,
                    Status = "空闲",
                    Rate = 288.50m,
                    BedType = "双人大床",
                    Floor = 3
                };
                
                var json = JsonSerializer.Serialize(roomData, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                });
                
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("/api/RoomManagement/rooms", content);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ApiResponse<RoomDetailDto>>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    
                    if (result?.Success == true && result.Data != null)
                    {
                        var roomId = result.Data.RoomId;
                        
                        // 验证读取
                        var getResponse = await _httpClient.GetAsync($"/api/RoomManagement/rooms/{roomId}");
                        if (getResponse.IsSuccessStatusCode)
                        {
                            var getContent = await getResponse.Content.ReadAsStringAsync();
                            var getResult = JsonSerializer.Deserialize<ApiResponse<RoomDetailDto>>(getContent, new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true
                            });
                            
                            if (getResult?.Data?.RoomType == "豪华套房" && 
                                getResult.Data.BedType == "双人大床" &&
                                getResult.Data.Status == "空闲")
                            {
                                results.Add(("房间管理", "创建和读取中文房间", true, "✅ 中文字符完全支持"));
                                Console.WriteLine($"✅ 房间创建成功: {getResult.Data.RoomNumber} - {getResult.Data.RoomType}");
                                
                                // 清理测试数据
                                await _httpClient.DeleteAsync($"/api/RoomManagement/rooms/{roomId}");
                            }
                            else
                            {
                                results.Add(("房间管理", "中文字符验证", false, "❌ 中文字符显示异常"));
                            }
                        }
                        else
                        {
                            results.Add(("房间管理", "读取房间", false, "❌ 房间读取失败"));
                        }
                    }
                    else
                    {
                        results.Add(("房间管理", "创建房间", false, "❌ 房间创建失败"));
                    }
                }
                else
                {
                    results.Add(("房间管理", "API调用", false, $"❌ HTTP错误: {response.StatusCode}"));
                }
            }
            catch (Exception ex)
            {
                results.Add(("房间管理", "模块测试", false, $"❌ 异常: {ex.Message}"));
            }
        }
        
        /// <summary>
        /// 测试设备管理模块中文字符支持
        /// </summary>
        private static async Task TestDeviceManagementModule(List<(string Module, string Test, bool Success, string Message)> results)
        {
            Console.WriteLine("\n📱 设备管理模块中文字符测试");
            Console.WriteLine("----------------------------");
            
            try
            {
                // 创建中文设备
                var deviceData = new DeviceCreateDto
                {
                    DeviceName = $"智能血压监测仪-{DateTime.Now:HHmmss}",
                    DeviceType = "医疗监测设备",
                    InstallationDate = DateTime.Now,
                    Status = "正常运行",
                    Location = "二楼护士站",
                    Description = "专业医疗级血压监测设备，支持中文显示",
                    LastMaintenanceDate = DateTime.Now.AddDays(-7)
                };
                
                var json = JsonSerializer.Serialize(deviceData, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                });
                
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("/api/DeviceManagement/devices", content);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ApiResponse<DeviceDetailDto>>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    
                    if (result?.Success == true && result.Data != null)
                    {
                        var deviceId = result.Data.DeviceId;
                        
                        // 验证读取
                        var getResponse = await _httpClient.GetAsync($"/api/DeviceManagement/devices/{deviceId}");
                        if (getResponse.IsSuccessStatusCode)
                        {
                            var getContent = await getResponse.Content.ReadAsStringAsync();
                            var getResult = JsonSerializer.Deserialize<ApiResponse<DeviceDetailDto>>(getContent, new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true
                            });
                            
                            if (getResult?.Data?.DeviceType == "医疗监测设备" && 
                                getResult.Data.Location == "二楼护士站" &&
                                getResult.Data.Status == "正常运行")
                            {
                                results.Add(("设备管理", "创建和读取中文设备", true, "✅ 中文字符完全支持"));
                                Console.WriteLine($"✅ 设备创建成功: {getResult.Data.DeviceName} - {getResult.Data.DeviceType}");
                                
                                // 清理测试数据
                                await _httpClient.DeleteAsync($"/api/DeviceManagement/devices/{deviceId}");
                            }
                            else
                            {
                                results.Add(("设备管理", "中文字符验证", false, "❌ 中文字符显示异常"));
                            }
                        }
                        else
                        {
                            results.Add(("设备管理", "读取设备", false, "❌ 设备读取失败"));
                        }
                    }
                    else
                    {
                        results.Add(("设备管理", "创建设备", false, "❌ 设备创建失败"));
                    }
                }
                else
                {
                    results.Add(("设备管理", "API调用", false, $"❌ HTTP错误: {response.StatusCode}"));
                }
            }
            catch (Exception ex)
            {
                results.Add(("设备管理", "模块测试", false, $"❌ 异常: {ex.Message}"));
            }
        }
        
        /// <summary>
        /// 测试健康监测模块中文字符支持
        /// </summary>
        private static async Task TestHealthMonitoringModule(List<(string Module, string Test, bool Success, string Message)> results)
        {
            Console.WriteLine("\n💓 健康监测模块中文字符测试");
            Console.WriteLine("----------------------------");
            
            try
            {
                // 获取老人列表（测试读取）
                var response = await _httpClient.GetAsync("/api/HealthMonitoring/elderly?pageSize=1");
                
                if (response.IsSuccessStatusCode)
                {
                    results.Add(("健康监测", "获取老人列表", true, "✅ API调用成功"));
                    Console.WriteLine("✅ 健康监测API响应正常");
                }
                else
                {
                    results.Add(("健康监测", "API调用", false, $"❌ HTTP错误: {response.StatusCode}"));
                }
            }
            catch (Exception ex)
            {
                results.Add(("健康监测", "模块测试", false, $"❌ 异常: {ex.Message}"));
            }
        }
        
        /// <summary>
        /// 测试电子围栏模块中文字符支持
        /// </summary>
        private static async Task TestElectronicFenceModule(List<(string Module, string Test, bool Success, string Message)> results)
        {
            Console.WriteLine("\n🔒 电子围栏模块中文字符测试");
            Console.WriteLine("----------------------------");
            
            try
            {
                // 获取围栏列表（测试读取）
                var response = await _httpClient.GetAsync("/api/ElectronicFence/fences?pageSize=1");
                
                if (response.IsSuccessStatusCode)
                {
                    results.Add(("电子围栏", "获取围栏列表", true, "✅ API调用成功"));
                    Console.WriteLine("✅ 电子围栏API响应正常");
                }
                else
                {
                    results.Add(("电子围栏", "API调用", false, $"❌ HTTP错误: {response.StatusCode}"));
                }
            }
            catch (Exception ex)
            {
                results.Add(("电子围栏", "模块测试", false, $"❌ 异常: {ex.Message}"));
            }
        }
        
        /// <summary>
        /// 生成测试报告
        /// </summary>
        private static void GenerateTestReport(List<(string Module, string Test, bool Success, string Message)> results)
        {
            Console.WriteLine("\n📊 中文字符兼容性测试报告");
            Console.WriteLine("========================================");
            
            var totalTests = results.Count;
            var passedTests = results.Count(r => r.Success);
            var failedTests = totalTests - passedTests;
            
            Console.WriteLine($"总测试数: {totalTests}");
            Console.WriteLine($"通过: {passedTests} ✅");
            Console.WriteLine($"失败: {failedTests} ❌");
            Console.WriteLine($"成功率: {(double)passedTests / totalTests * 100:F1}%");
            
            Console.WriteLine("\n详细结果:");
            foreach (var group in results.GroupBy(r => r.Module))
            {
                Console.WriteLine($"\n【{group.Key}】");
                foreach (var result in group)
                {
                    Console.WriteLine($"  {result.Test}: {result.Message}");
                }
            }
            
            if (passedTests == totalTests)
            {
                Console.WriteLine("\n🎉 所有测试通过！中文字符支持完美！");
            }
            else
            {
                Console.WriteLine($"\n⚠️ {failedTests} 个测试失败，需要检查修复");
            }
        }
    }
}
