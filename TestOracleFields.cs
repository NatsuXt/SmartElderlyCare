using Oracle.ManagedDataAccess.Client;

public class TestOracleFields
{
    public static async Task TestDeviceStatusFields()
    {
        var connectionString = "Data Source=47.96.238.102:1521/orcl;User Id=application_user;Password=20252025;";
        
        try
        {
            using var connection = new OracleConnection(connectionString);
            await connection.OpenAsync();
            
            Console.WriteLine("🔍 测试 DeviceStatus 表字段名：");
            
            var sql = "SELECT * FROM DeviceStatus WHERE ROWNUM <= 1";
            
            using var command = new OracleCommand(sql, connection);
            using var reader = await command.ExecuteReaderAsync();
            
            // 显示列名
            Console.WriteLine("实际字段名：");
            for (int i = 0; i < reader.FieldCount; i++)
            {
                Console.WriteLine($"  [{i}] {reader.GetName(i)}");
            }
            
            // 显示数据
            if (await reader.ReadAsync())
            {
                Console.WriteLine("\n示例数据：");
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    Console.WriteLine($"  {reader.GetName(i)}: {reader[i]}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"错误: {ex.Message}");
        }
    }
}
