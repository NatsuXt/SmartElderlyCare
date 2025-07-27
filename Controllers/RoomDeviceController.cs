using System;
using System.Collections.Generic;
using RoomDeviceManagement.Models;
using RoomDeviceManagement.Interfaces;
using RoomDeviceManagement.Implementation;
using RoomDeviceManagement.Services;

namespace RoomDeviceManagement.Controllers
{
    /// <summary>
    /// 房间设备管理主控制器
    /// </summary>
    public class RoomDeviceController
    {
        private readonly IRoomManagementService _roomService;
        private readonly IDeviceStatusService? _deviceService;
        private readonly IHealthMonitoringService? _healthService;
        private readonly DatabaseService _databaseService;

        public RoomDeviceController()
        {
            _databaseService = new DatabaseService();
            _roomService = new RoomManagementService(_databaseService);
            _deviceService = new DeviceStatusService(_databaseService);
            _healthService = new HealthMonitoringService(_databaseService);
        }

        /// <summary>
        /// 系统主菜单
        /// </summary>
        public void ShowMainMenu()
        {
            Console.Clear();
            Console.WriteLine("===========================================");
            Console.WriteLine("        智慧养老系统 - 房间设备管理模块");
            Console.WriteLine("===========================================");
            Console.WriteLine("1. 房间管理");
            Console.WriteLine("2. 设备状态管理");
            Console.WriteLine("3. 老人信息管理");
            Console.WriteLine("4. 健康监测管理");
            Console.WriteLine("5. 电子围栏管理");
            Console.WriteLine("6. 围栏日志管理");
            Console.WriteLine("7. 系统设置");
            Console.WriteLine("0. 退出系统");
            Console.WriteLine("===========================================");
            Console.Write("请选择功能模块 (0-7): ");
        }

        /// <summary>
        /// 房间管理菜单
        /// </summary>
        public void ShowRoomManagementMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("===========================================");
                Console.WriteLine("              房间管理模块");
                Console.WriteLine("===========================================");
                Console.WriteLine("1. 查看所有房间");
                Console.WriteLine("2. 查看房间详情");
                Console.WriteLine("3. 添加房间");
                Console.WriteLine("4. 更新房间信息");
                Console.WriteLine("5. 删除房间");
                Console.WriteLine("6. 查看空闲房间");
                Console.WriteLine("7. 按楼层查看房间");
                Console.WriteLine("8. 按状态查看房间");
                Console.WriteLine("0. 返回主菜单");
                Console.WriteLine("===========================================");
                Console.Write("请选择操作 (0-8): ");

                var choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        ViewAllRooms();
                        break;
                    case "2":
                        ViewRoomDetails();
                        break;
                    case "3":
                        AddRoom();
                        break;
                    case "4":
                        UpdateRoom();
                        break;
                    case "5":
                        DeleteRoom();
                        break;
                    case "6":
                        ViewAvailableRooms();
                        break;
                    case "7":
                        ViewRoomsByFloor();
                        break;
                    case "8":
                        ViewRoomsByStatus();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("无效选择，请重新输入！");
                        Console.ReadKey();
                        break;
                }
            }
        }

        /// <summary>
        /// 查看所有房间
        /// </summary>
        private void ViewAllRooms()
        {
            Console.Clear();
            Console.WriteLine("===========================================");
            Console.WriteLine("              所有房间信息");
            Console.WriteLine("===========================================");

            try
            {
                var rooms = _roomService.GetAllRooms();
                if (rooms.Count == 0)
                {
                    Console.WriteLine("暂无房间信息。");
                }
                else
                {
                    Console.WriteLine("{0,-8} {1,-8} {2,-6} {3,-10} {4,-10} {5,-6} {6,-8} {7,-8} {8,-10}",
                        "房间ID", "房间号", "楼层", "房间类型", "房间状态", "容量", "面积(㎡)", "床铺类型", "月租金(元)");
                    Console.WriteLine(new string('-', 95));

                    foreach (var room in rooms)
                    {
                        Console.WriteLine("{0,-8} {1,-8} {2,-6} {3,-10} {4,-10} {5,-6} {6,-8} {7,-8} {8,-10}",
                            room.RoomId, room.RoomNumber, room.FloorNum, room.RoomType, room.Status,
                            room.Capacity, "N/A", room.BedType, room.Rate);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取房间信息时发生错误：{ex.Message}");
            }

            Console.WriteLine("\n按任意键返回...");
            Console.ReadKey();
        }

        /// <summary>
        /// 查看房间详情
        /// </summary>
        private void ViewRoomDetails()
        {
            Console.Clear();
            Console.WriteLine("===========================================");
            Console.WriteLine("              查看房间详情");
            Console.WriteLine("===========================================");

            Console.Write("请输入房间ID: ");
            var roomIdInput = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(roomIdInput) || !int.TryParse(roomIdInput, out var roomId))
            {
                Console.WriteLine("房间ID必须是有效的数字！");
                Console.ReadKey();
                return;
            }

            try
            {
                var room = _roomService.GetRoomById(roomId);
                if (room == null)
                {
                    Console.WriteLine("未找到指定的房间！");
                }
                else
                {
                    Console.WriteLine($"房间ID: {room.RoomId}");
                    Console.WriteLine($"房间号: {room.RoomNumber}");
                    Console.WriteLine($"楼层: {room.FloorNum}");
                    Console.WriteLine($"房间类型: {room.RoomType}");
                    Console.WriteLine($"房间状态: {room.Status}");
                    Console.WriteLine($"容量: {room.Capacity}");
                    Console.WriteLine($"收费标准: {room.Rate} 元");
                    Console.WriteLine($"床铺类型: {room.BedType}");
                    Console.WriteLine($"创建时间: {room.CreatedTime:yyyy-MM-dd HH:mm:ss}");
                    Console.WriteLine($"更新时间: {room.UpdatedTime:yyyy-MM-dd HH:mm:ss}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取房间详情时发生错误：{ex.Message}");
            }

            Console.WriteLine("\n按任意键返回...");
            Console.ReadKey();
        }

        /// <summary>
        /// 添加房间
        /// </summary>
        private void AddRoom()
        {
            Console.Clear();
            Console.WriteLine("===========================================");
            Console.WriteLine("              添加新房间");
            Console.WriteLine("===========================================");

            try
            {
                Console.Write("房间号: ");
                var roomNumber = Console.ReadLine();
                
                Console.Write("楼层: ");
                if (!int.TryParse(Console.ReadLine(), out var floor))
                {
                    Console.WriteLine("楼层必须是数字！");
                    Console.ReadKey();
                    return;
                }

                Console.Write("房间类型 (单人间/双人间/套房/护理间): ");
                var roomType = Console.ReadLine();

                Console.Write("容量: ");
                if (!int.TryParse(Console.ReadLine(), out var capacity))
                {
                    Console.WriteLine("容量必须是数字！");
                    Console.ReadKey();
                    return;
                }

                Console.Write("收费标准 (元): ");
                if (!decimal.TryParse(Console.ReadLine(), out var rate))
                {
                    Console.WriteLine("收费标准必须是数字！");
                    Console.ReadKey();
                    return;
                }

                Console.Write("床铺类型 (单人床/双人床): ");
                var bedType = Console.ReadLine();

                var room = new RoomManagement
                {
                    RoomNumber = roomNumber!,
                    FloorNum = floor,
                    RoomType = roomType!,
                    Status = "Available", // 使用英文状态值
                    Capacity = capacity,
                    Rate = rate,
                    BedType = bedType!,
                    CreatedTime = DateTime.Now,
                    UpdatedTime = DateTime.Now
                };

                if (_roomService.AddRoom(room))
                {
                    Console.WriteLine("房间添加成功！");
                }
                else
                {
                    Console.WriteLine("房间添加失败！");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"添加房间时发生错误：{ex.Message}");
            }

            Console.WriteLine("\n按任意键返回...");
            Console.ReadKey();
        }

        /// <summary>
        /// 更新房间信息
        /// </summary>
        private void UpdateRoom()
        {
            Console.Clear();
            Console.WriteLine("===========================================");
            Console.WriteLine("              更新房间信息");
            Console.WriteLine("===========================================");

            Console.Write("请输入要更新的房间ID: ");
            var roomIdInput = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(roomIdInput) || !int.TryParse(roomIdInput, out var roomId))
            {
                Console.WriteLine("房间ID必须是有效的数字！");
                Console.ReadKey();
                return;
            }

            try
            {
                var room = _roomService.GetRoomById(roomId);
                if (room == null)
                {
                    Console.WriteLine("未找到指定的房间！");
                    Console.ReadKey();
                    return;
                }

                Console.WriteLine($"当前房间信息：{room.RoomNumber} - {room.RoomType} - {room.Status}");
                Console.WriteLine();

                Console.Write($"房间状态 (当前: {room.Status}，新值或回车保持不变): ");
                var newStatus = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newStatus))
                {
                    room.Status = newStatus;
                }

                Console.Write($"收费标准 (当前: {room.Rate}，新值或回车保持不变): ");
                var newRate = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newRate) && decimal.TryParse(newRate, out var rate))
                {
                    room.Rate = rate;
                }

                room.UpdatedTime = DateTime.Now;

                if (_roomService.UpdateRoom(room))
                {
                    Console.WriteLine("房间信息更新成功！");
                }
                else
                {
                    Console.WriteLine("房间信息更新失败！");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"更新房间信息时发生错误：{ex.Message}");
            }

            Console.WriteLine("\n按任意键返回...");
            Console.ReadKey();
        }

        /// <summary>
        /// 删除房间
        /// </summary>
        private void DeleteRoom()
        {
            Console.Clear();
            Console.WriteLine("===========================================");
            Console.WriteLine("              删除房间");
            Console.WriteLine("===========================================");

            Console.Write("请输入要删除的房间ID: ");
            var roomIdInput = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(roomIdInput) || !int.TryParse(roomIdInput, out var roomId))
            {
                Console.WriteLine("房间ID必须是有效的数字！");
                Console.ReadKey();
                return;
            }

            try
            {
                var room = _roomService.GetRoomById(roomId);
                if (room == null)
                {
                    Console.WriteLine("未找到指定的房间！");
                    Console.ReadKey();
                    return;
                }

                Console.WriteLine($"确认要删除房间：{room.RoomNumber} - {room.RoomType} 吗？");
                Console.Write("输入 'YES' 确认删除: ");
                var confirmation = Console.ReadLine();

                if (confirmation?.ToUpper() == "YES")
                {
                    if (_roomService.DeleteRoom(roomId))
                    {
                        Console.WriteLine("房间删除成功！");
                    }
                    else
                    {
                        Console.WriteLine("房间删除失败！");
                    }
                }
                else
                {
                    Console.WriteLine("删除操作已取消。");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"删除房间时发生错误：{ex.Message}");
            }

            Console.WriteLine("\n按任意键返回...");
            Console.ReadKey();
        }

        /// <summary>
        /// 查看空闲房间
        /// </summary>
        private void ViewAvailableRooms()
        {
            Console.Clear();
            Console.WriteLine("===========================================");
            Console.WriteLine("              空闲房间列表");
            Console.WriteLine("===========================================");

            try
            {
                var rooms = _roomService.GetAvailableRooms();
                if (rooms.Count == 0)
                {
                    Console.WriteLine("暂无空闲房间。");
                }
                else
                {
                    Console.WriteLine("{0,-8} {1,-8} {2,-6} {3,-10} {4,-6} {5,-10}",
                        "房间ID", "房间号", "楼层", "房间类型", "容量", "收费标准(元)");
                    Console.WriteLine(new string('-', 60));

                    foreach (var room in rooms)
                    {
                        Console.WriteLine("{0,-8} {1,-8} {2,-6} {3,-10} {4,-6} {5,-10}",
                            room.RoomId, room.RoomNumber, room.FloorNum, room.RoomType,
                            room.Capacity, room.Rate);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取空闲房间时发生错误：{ex.Message}");
            }

            Console.WriteLine("\n按任意键返回...");
            Console.ReadKey();
        }

        /// <summary>
        /// 按楼层查看房间
        /// </summary>
        private void ViewRoomsByFloor()
        {
            Console.Clear();
            Console.WriteLine("===========================================");
            Console.WriteLine("              按楼层查看房间");
            Console.WriteLine("===========================================");

            Console.Write("请输入楼层号: ");
            if (!int.TryParse(Console.ReadLine(), out var floor))
            {
                Console.WriteLine("楼层号必须是数字！");
                Console.ReadKey();
                return;
            }

            try
            {
                var rooms = _roomService.GetRoomsByFloor(floor);
                if (rooms.Count == 0)
                {
                    Console.WriteLine($"{floor}楼暂无房间。");
                }
                else
                {
                    Console.WriteLine($"\n{floor}楼房间列表：");
                    Console.WriteLine("{0,-8} {1,-8} {2,-10} {3,-10} {4,-6}",
                        "房间ID", "房间号", "房间类型", "房间状态", "容量");
                    Console.WriteLine(new string('-', 50));

                    foreach (var room in rooms)
                    {
                        Console.WriteLine("{0,-8} {1,-8} {2,-10} {3,-10} {4,-6}",
                            room.RoomId, room.RoomNumber, room.RoomType, room.Status,
                            room.Capacity);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"按楼层查看房间时发生错误：{ex.Message}");
            }

            Console.WriteLine("\n按任意键返回...");
            Console.ReadKey();
        }

        /// <summary>
        /// 按状态查看房间
        /// </summary>
        private void ViewRoomsByStatus()
        {
            Console.Clear();
            Console.WriteLine("===========================================");
            Console.WriteLine("              按状态查看房间");
            Console.WriteLine("===========================================");

            Console.WriteLine("房间状态选项：");
            Console.WriteLine("1. 空闲");
            Console.WriteLine("2. 已占用");
            Console.WriteLine("3. 维护中");
            Console.WriteLine("4. 清洁中");
            Console.Write("请选择状态 (1-4): ");

            var choice = Console.ReadLine();
            var status = choice switch
            {
                "1" => "Available",
                "2" => "Occupied", 
                "3" => "Maintenance",
                "4" => "Cleaning",
                _ => null
            };

            if (status == null)
            {
                Console.WriteLine("无效选择！");
                Console.ReadKey();
                return;
            }

            try
            {
                var rooms = _roomService.GetRoomsByStatus(status);
                if (rooms.Count == 0)
                {
                    Console.WriteLine($"暂无{status}状态的房间。");
                }
                else
                {
                    Console.WriteLine($"\n{status}房间列表：");
                    Console.WriteLine("{0,-8} {1,-8} {2,-6} {3,-10} {4,-6}",
                        "房间ID", "房间号", "楼层", "房间类型", "容量");
                    Console.WriteLine(new string('-', 50));

                    foreach (var room in rooms)
                    {
                        Console.WriteLine("{0,-8} {1,-8} {2,-6} {3,-10} {4,-6}",
                            room.RoomId, room.RoomNumber, room.FloorNum, room.RoomType,
                            room.Capacity);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"按状态查看房间时发生错误：{ex.Message}");
            }

            Console.WriteLine("\n按任意键返回...");
            Console.ReadKey();
        }

        /// <summary>
        /// 测试数据库连接
        /// </summary>
        public bool TestDatabaseConnection()
        {
            Console.WriteLine("正在测试数据库连接...");
            return _databaseService.TestConnection();
        }

        /// <summary>
        /// 显示数据库状态信息
        /// </summary>
        public void ShowDatabaseStatus()
        {
            Console.Clear();
            Console.WriteLine("===========================================");
            Console.WriteLine("              数据库状态检查");
            Console.WriteLine("===========================================");
            Console.WriteLine();
            
            Console.WriteLine("数据库连接信息:");
            Console.WriteLine("服务器: 47.96.238.102:1521");
            Console.WriteLine("服务名: orcl");
            Console.WriteLine("用户名: FIBRE");
            Console.WriteLine();
            
            // 测试连接
            if (!_databaseService.TestConnection())
            {
                Console.WriteLine("❌ 无法连接到数据库！");
                Console.WriteLine("\n按任意键返回...");
                Console.ReadKey();
                return;
            }
            
            Console.WriteLine();
            Console.WriteLine("检查系统表状态:");
            Console.WriteLine(new string('-', 40));
            
            var tableStatus = _databaseService.CheckTablesExist();
            foreach (var table in tableStatus)
            {
                string status = table.Value ? "✅ 存在" : "❌ 不存在";
                Console.WriteLine($"{table.Key,-20} {status}");
            }
            
            Console.WriteLine();
            int existingTables = tableStatus.Values.Count(x => x);
            Console.WriteLine($"表状态总结: {existingTables}/{tableStatus.Count} 个表已创建");
            
            if (existingTables == 0)
            {
                Console.WriteLine();
                Console.WriteLine("💡 提示: 请先执行SQL脚本创建数据库表:");
                Console.WriteLine("   1. 打开SQL Developer或其他Oracle客户端");
                Console.WriteLine("   2. 连接到数据库 (FIBRE/FIBRE2025@47.96.238.102:1521/orcl)");
                Console.WriteLine("   3. 执行 SQL/CreateTables.sql 脚本");
                Console.WriteLine("   4. 执行 SQL/TestData.sql 脚本 (可选)");
            }
            else if (existingTables < tableStatus.Count)
            {
                Console.WriteLine();
                Console.WriteLine("⚠️  警告: 部分表缺失，请检查数据库创建脚本！");
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine("✅ 所有系统表已就绪，可以正常使用系统功能！");
            }
            
            Console.WriteLine("\n按任意键返回...");
            Console.ReadKey();
        }
    }
}
