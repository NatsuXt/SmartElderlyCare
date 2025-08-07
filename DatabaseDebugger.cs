using RoomDeviceManagement.Services;
using Oracle.ManagedDataAccess.Client;
using System.Linq;

/// <summary>
/// 数据库连接调试工具
/// </summary>
public class DatabaseDebugger
{
    public static void TestMultipleConnections()
    {
        Console.WriteLine("===========================================");
        Console.WriteLine("数据库连接调试测试");
        Console.WriteLine("===========================================");
        
        // 测试不同的连接字符串配置
        var connectionConfigs = new[]
        {
            new { Name = "Config 1", ConnectionString = "Data Source=47.96.238.102:1521/orcl;User Id=application_user;Password=20252025;" },
            new { Name = "Config 2", ConnectionString = "Data Source=47.96.238.102:1521/ORCL;User Id=application_user;Password=20252025;" },
            new { Name = "Config 3", ConnectionString = "Data Source=47.96.238.102:1521/XE;User Id=application_user;Password=20252025;" },
            new { Name = "Config 4", ConnectionString = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=47.96.238.102)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=orcl)));User Id=application_user;Password=20252025;" },
            new { Name = "Config 5", ConnectionString = "Data Source=47.96.238.102:1521/orcl;User Id=APPLICATION_USER;Password=20252025;" },
        };
        
        foreach (var config in connectionConfigs)
        {
            Console.WriteLine($"\n🔗 测试 {config.Name}:");
            Console.WriteLine($"   连接字符串: {config.ConnectionString}");
            
            try
            {
                using var connection = new OracleConnection(config.ConnectionString);
                connection.Open();
                
                // 测试基本查询
                using var command = new OracleCommand("SELECT USER FROM DUAL", connection);
                var currentUser = command.ExecuteScalar();
                
                Console.WriteLine($"✅ 连接成功！当前用户: {currentUser}");
                
                // 测试表访问
                using var tablesCommand = new OracleCommand("SELECT COUNT(*) FROM USER_TABLES", connection);
                var tableCount = tablesCommand.ExecuteScalar();
                Console.WriteLine($"✅ 表权限正常，用户表数量: {tableCount}");
                
                // 查询所有用户可访问的表
                Console.WriteLine("\n📋 查询所有可访问的表:");
                using var allTablesCommand = new OracleCommand(
                    "SELECT table_name FROM user_tables ORDER BY table_name", 
                    connection);
                using var allTablesReader = allTablesCommand.ExecuteReader();
                
                var tableNames = new List<string>();
                while (allTablesReader.Read())
                {
                    var tableName = allTablesReader["TABLE_NAME"]?.ToString();
                    if (!string.IsNullOrEmpty(tableName))
                    {
                        tableNames.Add(tableName);
                        Console.WriteLine($"   - {tableName}");
                    }
                }
                
                // 查找包含关键字的表
                Console.WriteLine("\n🔍 查找可能相关的表:");
                var relevantTables = tableNames.Where(t => 
                    t.Contains("HEALTH") || 
                    t.Contains("MONITOR") || 
                    t.Contains("ELDER") ||
                    t.Contains("ROOM") ||
                    t.Contains("DEVICE") ||
                    t.Contains("FENCE")).ToList();
                
                if (relevantTables.Any())
                {
                    foreach (var table in relevantTables)
                    {
                        Console.WriteLine($"   🎯 {table}");
                    }
                }
                else
                {
                    Console.WriteLine("   ❌ 未找到相关表");
                }
                
                // 尝试查询所有schema中的表
                Console.WriteLine("\n🔍 查询所有schema中的表:");
                try
                {
                    using var allSchemasCommand = new OracleCommand(@"
                        SELECT owner, table_name 
                        FROM all_tables 
                        WHERE table_name IN ('HEALTHMONITORING', 'ELDERLYINFO', 'DEVICESTATUS', 'ROOMMANAGEMENT', 'ELECTRONICFENCE', 'FENCELOG')
                        ORDER BY owner, table_name", connection);
                    using var allSchemasReader = allSchemasCommand.ExecuteReader();
                    
                    while (allSchemasReader.Read())
                    {
                        var owner = allSchemasReader["OWNER"]?.ToString();
                        var tableName = allSchemasReader["TABLE_NAME"]?.ToString();
                        Console.WriteLine($"   🎯 {owner}.{tableName}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"   ❌ 查询失败: {ex.Message}");
                }
                
                // 尝试直接访问主用户的表
                Console.WriteLine("\n🔑 尝试访问其他用户的表:");
                var possibleOwners = new[] { "SYSTEM", "SMART_ELDERLY", "ELDERLY_CARE", "ORCL", "HR", "SYS" };
                foreach (var owner in possibleOwners)
                {
                    try
                    {
                        using var testCommand = new OracleCommand($"SELECT COUNT(*) FROM {owner}.HEALTHMONITORING", connection);
                        var count = testCommand.ExecuteScalar();
                        Console.WriteLine($"   ✅ {owner}.HEALTHMONITORING: {count} 条记录");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"   ❌ {owner}.HEALTHMONITORING: {ex.Message}");
                    }
                }
                try
                {
                    using var healthTableCommand = new OracleCommand(@"
                        SELECT column_name, data_type, nullable
                        FROM user_tab_columns 
                        WHERE table_name = 'HEALTHMONITORING'
                        ORDER BY column_id", connection);
                    
                    using var reader = healthTableCommand.ExecuteReader();
                    Console.WriteLine("📋 HealthMonitoring 表结构:");
                    while (reader.Read())
                    {
                        Console.WriteLine($"   {reader["COLUMN_NAME"]}: {reader["DATA_TYPE"]} ({reader["NULLABLE"]})");
                    }
                }
                catch (Exception tableEx)
                {
                    Console.WriteLine($"❌ HealthMonitoring 表查询失败: {tableEx.Message}");
                }
                
                return; // 找到成功的连接就退出
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ 连接失败: {ex.Message}");
            }
        }
        
        Console.WriteLine("\n❌ 所有连接配置均失败！");
        Console.WriteLine("\n🔍 故障排查建议:");
        Console.WriteLine("1. 检查服务器 47.96.238.102 是否可访问");
        Console.WriteLine("2. 检查端口 1521 是否开放");
        Console.WriteLine("3. 检查用户名 application_user 是否存在");
        Console.WriteLine("4. 检查密码 20252025 是否正确");
        Console.WriteLine("5. 检查数据库服务名 orcl 是否正确");
    }
    
    /// <summary>
    /// 测试网络连接
    /// </summary>
    public static async Task TestNetworkConnection()
    {
        Console.WriteLine("\n🌐 测试网络连接...");
        
        try
        {
            using var client = new System.Net.Sockets.TcpClient();
            await client.ConnectAsync("47.96.238.102", 1521);
            Console.WriteLine("✅ 网络连接正常，可以访问 47.96.238.102:1521");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ 网络连接失败: {ex.Message}");
        }
    }
}
