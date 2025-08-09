using RoomDeviceManagement.Services;
using Microsoft.Extensions.Logging;

namespace RoomDeviceManagement
{
    public class TestChineseService
    {
        public static async Task RunTest()
        {
            // 简单测试中文兼容服务
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("🧪 测试中文兼容数据库服务");

            // 设置Oracle环境变量
            Environment.SetEnvironmentVariable("NLS_LANG", "SIMPLIFIED CHINESE_CHINA.AL32UTF8");
            Environment.SetEnvironmentVariable("ORA_NCHAR_LITERAL_REPLACE", "TRUE");

            // 创建logger
            using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var logger = loggerFactory.CreateLogger<ChineseCompatibleDatabaseService>();

            // 创建服务实例
            var service = new ChineseCompatibleDatabaseService(logger);

            try
            {
                Console.WriteLine("🏠 开始测试创建包含中文的房间...");
                
                // 创建一个包含中文的测试房间
                var roomId = await service.CreateRoomAsync(
                    "测试888", 
                    "豪华套房", 
                    3, 
                    "空闲", 
                    580, 
                    "特大床", 
                    8
                );
                
                Console.WriteLine($"✅ 房间创建成功，ID: {roomId}");
                
                // 立即查询这个房间
                Console.WriteLine("🔍 查询刚创建的房间...");
                var room = await service.GetRoomByNumberAsync("测试888");
                
                if (room != null)
                {
                    Console.WriteLine($"📋 房间详情:");
                    Console.WriteLine($"   房间号: {room.RoomNumber}");
                    Console.WriteLine($"   房间类型: {room.RoomType}");
                    Console.WriteLine($"   状态: {room.Status}");
                    Console.WriteLine($"   床型: {room.BedType}");
                    Console.WriteLine($"   容量: {room.Capacity}");
                    Console.WriteLine($"   楼层: {room.Floor}");
                    
                    if (room.RoomType == "豪华套房" && room.Status == "空闲" && room.BedType == "特大床")
                    {
                        Console.WriteLine("🎉 中文字符完美显示！");
                    }
                    else
                    {
                        Console.WriteLine("❌ 中文字符显示异常");
                    }
                }
                else
                {
                    Console.WriteLine("❌ 未找到刚创建的房间");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ 测试失败: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }

            Console.WriteLine("\n按任意键退出...");
            Console.ReadKey();
        }
    }
}
