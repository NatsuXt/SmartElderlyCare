using RoomDeviceManagement.Services;
using RoomDeviceManagement.Models;

namespace RoomDeviceManagement
{
    /// <summary>
    /// 数据库连接和业务测试程序
    /// </summary>
    public class DatabaseConnectionTest
    {
        public static async Task TestDatabaseConnection()
        {
            var connectionString = "Data Source=47.96.238.102:1521/orcl;User Id=application_user;Password=20252025;Connection Timeout=30;";
            var dbService = new DatabaseService(connectionString);
            
            Console.WriteLine("===========================================");
            Console.WriteLine("数据库连接测试");
            Console.WriteLine("===========================================");
            
            // 1. 测试基本连接
            Console.WriteLine("1. 测试数据库连接...");
            bool connectionSuccess = dbService.TestConnection();
            
            if (!connectionSuccess)
            {
                Console.WriteLine("❌ 数据库连接失败，请检查连接配置");
                return;
            }
            
            // 2. 检查表是否存在
            Console.WriteLine("\n2. 检查系统表是否存在...");
            var tableStatus = dbService.CheckTablesExist();
            foreach (var table in tableStatus)
            {
                string status = table.Value ? "✅ 存在" : "❌ 不存在";
                Console.WriteLine($"   {table.Key}: {status}");
            }
            
            // 3. 检查员工表中是否有维修人员
            Console.WriteLine("\n3. 检查维修人员数据...");
            try
            {
                var staffSql = "SELECT STAFF_ID, NAME, POSITION FROM STAFFINFO WHERE POSITION = '维修人员'";
                var maintenanceStaff = await dbService.QueryAsync<STAFFINFO>(staffSql);
                
                if (maintenanceStaff.Any())
                {
                    Console.WriteLine($"✅ 找到 {maintenanceStaff.Count()} 个维修人员:");
                    foreach (var staff in maintenanceStaff)
                    {
                        Console.WriteLine($"   - ID: {staff.STAFF_ID}, 姓名: {staff.NAME}");
                    }
                }
                else
                {
                    Console.WriteLine("⚠️  未找到维修人员，将创建测试数据...");
                    await CreateTestMaintenanceStaff(dbService);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ 查询维修人员失败: {ex.Message}");
            }
            
            // 4. 检查设备状态表
            Console.WriteLine("\n4. 检查设备状态数据...");
            try
            {
                var deviceSql = "SELECT COUNT(*) FROM DeviceStatus";
                var deviceResult = await dbService.QueryFirstAsync<dynamic>(deviceSql);
                var deviceCount = deviceResult?.COUNT ?? 0;
                Console.WriteLine($"✅ 设备状态表中有 {deviceCount} 条记录");
                
                // 如果没有设备，创建测试设备
                if (Convert.ToInt32(deviceCount) == 0)
                {
                    Console.WriteLine("⚠️  未找到设备数据，将创建测试设备...");
                    await CreateTestDevices(dbService);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ 查询设备状态失败: {ex.Message}");
            }
            
            Console.WriteLine("\n===========================================");
            Console.WriteLine("数据库测试完成");
            Console.WriteLine("===========================================");
        }
        
        /// <summary>
        /// 创建测试维修人员数据
        /// </summary>
        private static async Task CreateTestMaintenanceStaff(DatabaseService dbService)
        {
            try
            {
                var insertSql = @"
                    INSERT INTO STAFFINFO (
                        STAFF_ID, NAME, GENDER, POSITION, CONTACT_PHONE, EMAIL, 
                        HIRE_DATE, SALARY, SKILL_LEVEL, WORK_SCHEDULE
                    ) VALUES (
                        :StaffId, :Name, :Gender, :Position, :ContactPhone, :Email,
                        :HireDate, :Salary, :SkillLevel, :WorkSchedule
                    )";
                
                var staffData = new[]
                {
                    new {
                        StaffId = 1001,
                        Name = "张维修",
                        Gender = "男",
                        Position = "维修人员",
                        ContactPhone = "13812345678",
                        Email = "zhangweixiu@example.com",
                        HireDate = DateTime.Now.AddMonths(-6),
                        Salary = 5000m,
                        SkillLevel = "高级",
                        WorkSchedule = "8:00-18:00"
                    },
                    new {
                        StaffId = 1002,
                        Name = "李技师",
                        Gender = "男", 
                        Position = "维修人员",
                        ContactPhone = "13987654321",
                        Email = "lijishi@example.com",
                        HireDate = DateTime.Now.AddMonths(-3),
                        Salary = 4500m,
                        SkillLevel = "中级",
                        WorkSchedule = "9:00-17:00"
                    }
                };
                
                foreach (var staff in staffData)
                {
                    await dbService.ExecuteAsync(insertSql, staff);
                    Console.WriteLine($"✅ 创建维修人员: {staff.Name}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ 创建维修人员失败: {ex.Message}");
            }
        }
        
        /// <summary>
        /// 创建测试设备数据
        /// </summary>
        private static async Task CreateTestDevices(DatabaseService dbService)
        {
            try
            {
                var insertSql = @"
                    INSERT INTO DeviceStatus (
                        device_id, device_name, device_type, installation_date,
                        status, last_maintenance_date, location
                    ) VALUES (
                        :DeviceId, :DeviceName, :DeviceType, :InstallationDate,
                        :Status, :LastMaintenanceDate, :Location
                    )";
                
                var deviceData = new[]
                {
                    new {
                        DeviceId = 1,
                        DeviceName = "智能床垫001",
                        DeviceType = "智能床垫",
                        InstallationDate = DateTime.Now.AddDays(-30),
                        Status = "正常",
                        LastMaintenanceDate = DateTime.Now.AddDays(-1),
                        Location = "101房间"
                    },
                    new {
                        DeviceId = 2,
                        DeviceName = "门锁002",
                        DeviceType = "智能门锁",
                        InstallationDate = DateTime.Now.AddDays(-45),
                        Status = "正常",
                        LastMaintenanceDate = DateTime.Now.AddDays(-2),
                        Location = "102房间"
                    },
                    new {
                        DeviceId = 3,
                        DeviceName = "心率监测仪003",
                        DeviceType = "心率监测仪",
                        InstallationDate = DateTime.Now.AddDays(-20),
                        Status = "正常",
                        LastMaintenanceDate = DateTime.Now.AddDays(-1),
                        Location = "103房间"
                    }
                };
                
                foreach (var device in deviceData)
                {
                    await dbService.ExecuteAsync(insertSql, device);
                    Console.WriteLine($"✅ 创建测试设备: {device.DeviceName}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ 创建测试设备失败: {ex.Message}");
            }
        }
    }
}
