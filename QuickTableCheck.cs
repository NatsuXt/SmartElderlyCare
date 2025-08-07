using Oracle.ManagedDataAccess.Client;

public class QuickTableCheck
{
    public static async Task CheckTableStructure()
    {
        var connectionString = "Data Source=47.96.238.102:1521/orcl;User Id=application_user;Password=20252025;";
        
        try
        {
            using var connection = new OracleConnection(connectionString);
            await connection.OpenAsync();
            
            Console.WriteLine("🔍 查询 SYS.HEALTHMONITORING 表结构：");
            
            var sql = @"
                SELECT column_name, data_type, nullable 
                FROM all_tab_columns 
                WHERE owner = 'SYS' AND table_name = 'HEALTHMONITORING'
                ORDER BY column_id";
            
            using var command = new OracleCommand(sql, connection);
            using var reader = await command.ExecuteReaderAsync();
            
            while (await reader.ReadAsync())
            {
                var columnName = reader["COLUMN_NAME"]?.ToString();
                var dataType = reader["DATA_TYPE"]?.ToString();
                var nullable = reader["NULLABLE"]?.ToString();
                Console.WriteLine($"   {columnName} - {dataType} ({nullable})");
            }
            
            Console.WriteLine("\n📄 查询表中的数据示例：");
            var dataSql = "SELECT * FROM SYS.HEALTHMONITORING WHERE ROWNUM <= 3";
            using var dataCommand = new OracleCommand(dataSql, connection);
            using var dataReader = await dataCommand.ExecuteReaderAsync();
            
            // 显示列名
            for (int i = 0; i < dataReader.FieldCount; i++)
            {
                Console.Write($"{dataReader.GetName(i)}\t");
            }
            Console.WriteLine();
            
            // 显示数据
            while (await dataReader.ReadAsync())
            {
                for (int i = 0; i < dataReader.FieldCount; i++)
                {
                    Console.Write($"{dataReader[i]}\t");
                }
                Console.WriteLine();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ 错误: {ex.Message}");
        }
    }
}
