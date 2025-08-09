using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Data;

namespace RoomDeviceManagement;

/// <summary>
/// 中文字符编码问题诊断和修复工具
/// </summary>
public class ChineseEncodingDiagnostic
{
    private static readonly string ConnectionString = "Data Source=47.96.238.102:1521/orcl;User Id=application_user;Password=20252025;Connection Timeout=30;Pooling=false;";

    /// <summary>
    /// 运行中文字符编码诊断
    /// </summary>
    public static async Task RunDiagnostic()
    {
        Console.WriteLine("🔍 开始中文字符编码诊断...");
        Console.WriteLine();

        // 设置Oracle环境变量
        Environment.SetEnvironmentVariable("NLS_LANG", "SIMPLIFIED CHINESE_CHINA.AL32UTF8");
        Environment.SetEnvironmentVariable("ORA_NCHAR_LITERAL_REPLACE", "TRUE");
        
        try
        {
            await TestChineseCharacterInsertion();
            await TestChineseCharacterRetrieval();
            await TestParameterBinding();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ 诊断过程中发生错误: {ex.Message}");
            Console.WriteLine($"详细错误: {ex}");
        }
    }

    /// <summary>
    /// 测试中文字符插入
    /// </summary>
    private static async Task TestChineseCharacterInsertion()
    {
        Console.WriteLine("📝 测试中文字符插入...");

        using var connection = new OracleConnection(ConnectionString);
        await connection.OpenAsync();

        // 测试数据
        var testData = new[]
        {
            new { Name = "智能血压计", Type = "血压监测设备", Location = "301房间", Status = "正常运行" },
            new { Name = "心率监护仪", Type = "心率监测设备", Location = "护士站", Status = "维护中" },
            new { Name = "跌倒检测器", Type = "安全监测设备", Location = "走廊", Status = "离线" }
        };

        foreach (var data in testData)
        {
            try
            {
                // 方法1: 使用NVarchar2参数类型（推荐）
                var sql1 = @"INSERT INTO DeviceStatus (device_name, device_type, location, status, installation_date, last_maintenance_date) 
                           VALUES (:deviceName, :deviceType, :location, :status, SYSDATE, SYSDATE)";

                using var cmd1 = new OracleCommand(sql1, connection);
                cmd1.Parameters.Add(new OracleParameter("deviceName", OracleDbType.NVarchar2) { Value = data.Name });
                cmd1.Parameters.Add(new OracleParameter("deviceType", OracleDbType.NVarchar2) { Value = data.Type });
                cmd1.Parameters.Add(new OracleParameter("location", OracleDbType.NVarchar2) { Value = data.Location });
                cmd1.Parameters.Add(new OracleParameter("status", OracleDbType.NVarchar2) { Value = data.Status });

                var result1 = await cmd1.ExecuteNonQueryAsync();
                Console.WriteLine($"✅ 方法1成功插入: {data.Name} - {data.Type}");

                // 立即读取验证
                var verifySql = @"SELECT device_name, device_type, location, status 
                                FROM DeviceStatus 
                                WHERE device_name = :deviceName 
                                ORDER BY device_id DESC 
                                FETCH FIRST 1 ROWS ONLY";

                using var verifyCmd = new OracleCommand(verifySql, connection);
                verifyCmd.Parameters.Add(new OracleParameter("deviceName", OracleDbType.NVarchar2) { Value = data.Name });

                using var reader = await verifyCmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    var retrievedName = reader.GetString(0);
                    var retrievedType = reader.GetString(1);
                    var retrievedLocation = reader.GetString(2);
                    var retrievedStatus = reader.GetString(3);

                    Console.WriteLine($"🔍 读取验证: 名称={retrievedName}, 类型={retrievedType}, 位置={retrievedLocation}, 状态={retrievedStatus}");
                    
                    // 检查是否包含乱码
                    if (retrievedName.Contains("?") || retrievedType.Contains("?") || 
                        retrievedLocation.Contains("?") || retrievedStatus.Contains("?"))
                    {
                        Console.WriteLine($"⚠️ 发现乱码问题！");
                    }
                    else
                    {
                        Console.WriteLine($"✅ 中文字符正常显示");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ 插入失败: {data.Name} - 错误: {ex.Message}");
            }

            Console.WriteLine();
        }
    }

    /// <summary>
    /// 测试中文字符检索
    /// </summary>
    private static async Task TestChineseCharacterRetrieval()
    {
        Console.WriteLine("📖 测试中文字符检索...");

        using var connection = new OracleConnection(ConnectionString);
        await connection.OpenAsync();

        // 检索包含中文字符的记录
        var sql = @"SELECT device_id, device_name, device_type, location, status 
                   FROM DeviceStatus 
                   WHERE device_name LIKE '%智能%' OR device_name LIKE '%监测%' OR device_name LIKE '%检测%'
                   ORDER BY device_id DESC";

        using var cmd = new OracleCommand(sql, connection);
        using var reader = await cmd.ExecuteReaderAsync();

        var count = 0;
        while (await reader.ReadAsync())
        {
            count++;
            var deviceId = reader.GetInt32(0);
            var deviceName = reader.GetString(1);
            var deviceType = reader.GetString(2);
            var location = reader.GetString(3);
            var status = reader.GetString(4);

            Console.WriteLine($"设备 {deviceId}: {deviceName} | {deviceType} | {location} | {status}");

            // 检查编码问题
            if (deviceName.Contains("?") || deviceType.Contains("?") || location.Contains("?") || status.Contains("?"))
            {
                Console.WriteLine($"⚠️ 发现编码问题: 设备 {deviceId}");
            }
        }

        Console.WriteLine($"📊 总共检索到 {count} 条包含中文的设备记录");
        Console.WriteLine();
    }

    /// <summary>
    /// 测试参数绑定方式
    /// </summary>
    private static async Task TestParameterBinding()
    {
        Console.WriteLine("🔧 测试不同的参数绑定方式...");

        using var connection = new OracleConnection(ConnectionString);
        await connection.OpenAsync();

        var testName = "参数测试设备";
        var testType = "测试设备类型";

        // 方法1: 明确指定参数类型和方向
        Console.WriteLine("方法1: 明确指定参数类型");
        var sql1 = "INSERT INTO DeviceStatus (device_name, device_type, location, status, installation_date, last_maintenance_date) VALUES (:name, :type, :location, :status, SYSDATE, SYSDATE)";
        
        using var cmd1 = new OracleCommand(sql1, connection);
        cmd1.Parameters.Add(new OracleParameter("name", OracleDbType.NVarchar2) { Value = testName });
        cmd1.Parameters.Add(new OracleParameter("type", OracleDbType.NVarchar2) { Value = testType });
        cmd1.Parameters.Add(new OracleParameter("location", OracleDbType.NVarchar2) { Value = "测试位置1" });
        cmd1.Parameters.Add(new OracleParameter("status", OracleDbType.NVarchar2) { Value = "测试状态1" });

        try
        {
            await cmd1.ExecuteNonQueryAsync();
            Console.WriteLine("✅ 方法1: 参数绑定成功");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ 方法1: 参数绑定失败 - {ex.Message}");
        }

        // 方法2: 使用Add方法的重载
        Console.WriteLine("方法2: 使用Add方法重载");
        var sql2 = "INSERT INTO DeviceStatus (device_name, device_type, location, status, installation_date, last_maintenance_date) VALUES (:name, :type, :location, :status, SYSDATE, SYSDATE)";
        
        using var cmd2 = new OracleCommand(sql2, connection);
        cmd2.Parameters.Add(":name", OracleDbType.NVarchar2).Value = testName + "2";
        cmd2.Parameters.Add(":type", OracleDbType.NVarchar2).Value = testType + "2";
        cmd2.Parameters.Add(":location", OracleDbType.NVarchar2).Value = "测试位置2";
        cmd2.Parameters.Add(":status", OracleDbType.NVarchar2).Value = "测试状态2";

        try
        {
            await cmd2.ExecuteNonQueryAsync();
            Console.WriteLine("✅ 方法2: 参数绑定成功");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ 方法2: 参数绑定失败 - {ex.Message}");
        }

        Console.WriteLine();
    }

    /// <summary>
    /// 修复现有的乱码数据
    /// </summary>
    public static async Task FixGarbledData()
    {
        Console.WriteLine("🔧 修复现有的乱码数据...");

        using var connection = new OracleConnection(ConnectionString);
        await connection.OpenAsync();

        // 查找包含问号的记录
        var selectSql = @"SELECT device_id, device_name, device_type, location, status 
                         FROM DeviceStatus 
                         WHERE device_name LIKE '%?%' OR device_type LIKE '%?%' OR location LIKE '%?%' OR status LIKE '%?%'";

        using var selectCmd = new OracleCommand(selectSql, connection);
        using var reader = await selectCmd.ExecuteReaderAsync();

        var corruptedRecords = new List<(int id, string name, string type, string location, string status)>();

        while (await reader.ReadAsync())
        {
            corruptedRecords.Add((
                reader.GetInt32(0),
                reader.GetString(1),
                reader.GetString(2),
                reader.GetString(3),
                reader.GetString(4)
            ));
        }

        Console.WriteLine($"发现 {corruptedRecords.Count} 条乱码记录");

        // 修复已知的记录
        var fixMappings = new Dictionary<int, (string name, string type, string location, string status)>
        {
            { 1, ("智能血压计A1", "血压监测设备", "101房间", "正常运行") },
            { 2, ("心率监护仪B2", "心率监测设备", "202房间", "正常运行") },
            { 3, ("跌倒检测器C3", "安全监测设备", "3楼走廊", "正常运行") },
            { 42, ("API测试设备", "智能传感器", "测试房间", "正常运行") }
        };

        foreach (var record in corruptedRecords)
        {
            if (fixMappings.ContainsKey(record.id))
            {
                var fixData = fixMappings[record.id];
                var updateSql = @"UPDATE DeviceStatus 
                                SET device_name = :deviceName, 
                                    device_type = :deviceType, 
                                    location = :location, 
                                    status = :status 
                                WHERE device_id = :deviceId";

                using var updateCmd = new OracleCommand(updateSql, connection);
                updateCmd.Parameters.Add(new OracleParameter("deviceName", OracleDbType.NVarchar2) { Value = fixData.name });
                updateCmd.Parameters.Add(new OracleParameter("deviceType", OracleDbType.NVarchar2) { Value = fixData.type });
                updateCmd.Parameters.Add(new OracleParameter("location", OracleDbType.NVarchar2) { Value = fixData.location });
                updateCmd.Parameters.Add(new OracleParameter("status", OracleDbType.NVarchar2) { Value = fixData.status });
                updateCmd.Parameters.Add(new OracleParameter("deviceId", OracleDbType.Int32) { Value = record.id });

                var rowsAffected = await updateCmd.ExecuteNonQueryAsync();
                if (rowsAffected > 0)
                {
                    Console.WriteLine($"✅ 修复设备 {record.id}: {fixData.name}");
                }
            }
            else
            {
                Console.WriteLine($"⚠️ 设备 {record.id} 无预定义修复数据，跳过");
            }
        }

        Console.WriteLine("🎉 数据修复完成！");
    }
}
