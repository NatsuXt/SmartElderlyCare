using Oracle.ManagedDataAccess.Client;

public class FenceTableCheck
{
    public static async Task CheckFenceTableStructure()
    {
        var connectionString = "Data Source=47.96.238.102:1521/orcl;User Id=application_user;Password=20252025;";
        
        try
        {
            using var connection = new OracleConnection(connectionString);
            await connection.OpenAsync();
            
            Console.WriteLine("🔍 查询围栏相关表结构：");
            
            // 查询电子围栏相关的表
            var tablesSql = @"
                SELECT table_name 
                FROM all_tables 
                WHERE owner = 'SYS' AND table_name LIKE '%FENCE%'
                ORDER BY table_name";
            
            using var tablesCommand = new OracleCommand(tablesSql, connection);
            using var tablesReader = await tablesCommand.ExecuteReaderAsync();
            
            Console.WriteLine("\n📋 找到的围栏相关表：");
            while (await tablesReader.ReadAsync())
            {
                var tableName = tablesReader["TABLE_NAME"]?.ToString();
                Console.WriteLine($"   📄 {tableName}");
            }
            
            // 查询FENCE_LOG表结构
            Console.WriteLine("\n🔍 查询 SYS.FENCELOG 表结构：");
            
            var sql = @"
                SELECT column_name, data_type, nullable 
                FROM all_tab_columns 
                WHERE owner = 'SYS' AND table_name = 'FENCELOG'
                ORDER BY column_id";
            
            using var command = new OracleCommand(sql, connection);
            using var reader = await command.ExecuteReaderAsync();
            
            bool hasData = false;
            while (await reader.ReadAsync())
            {
                hasData = true;
                var columnName = reader["COLUMN_NAME"]?.ToString();
                var dataType = reader["DATA_TYPE"]?.ToString();
                var nullable = reader["NULLABLE"]?.ToString();
                Console.WriteLine($"   {columnName} - {dataType} ({nullable})");
            }
            
            if (!hasData)
            {
                Console.WriteLine("   ⚠️ 没有找到 FENCELOG 表或表为空");
            }
            
            // 查询ELECTRONIC_FENCE表结构
            Console.WriteLine("\n🔍 查询 SYS.ELECTRONICFENCE 表结构：");
            
            var fenceSql = @"
                SELECT column_name, data_type, nullable 
                FROM all_tab_columns 
                WHERE owner = 'SYS' AND table_name = 'ELECTRONICFENCE'
                ORDER BY column_id";
            
            using var fenceCommand = new OracleCommand(fenceSql, connection);
            using var fenceReader = await fenceCommand.ExecuteReaderAsync();
            
            bool hasFenceData = false;
            while (await fenceReader.ReadAsync())
            {
                hasFenceData = true;
                var columnName = fenceReader["COLUMN_NAME"]?.ToString();
                var dataType = fenceReader["DATA_TYPE"]?.ToString();
                var nullable = fenceReader["NULLABLE"]?.ToString();
                Console.WriteLine($"   {columnName} - {dataType} ({nullable})");
            }
            
            if (!hasFenceData)
            {
                Console.WriteLine("   ⚠️ 没有找到 ELECTRONICFENCE 表或表为空");
            }
            
            // 查询STAFF_LOCATION表结构
            Console.WriteLine("\n🔍 查询 SYS.STAFFLOCATION 表结构：");
            
            var staffSql = @"
                SELECT column_name, data_type, nullable 
                FROM all_tab_columns 
                WHERE owner = 'SYS' AND table_name = 'STAFFLOCATION'
                ORDER BY column_id";
            
            using var staffCommand = new OracleCommand(staffSql, connection);
            using var staffReader = await staffCommand.ExecuteReaderAsync();
            
            bool hasStaffData = false;
            while (await staffReader.ReadAsync())
            {
                hasStaffData = true;
                var columnName = staffReader["COLUMN_NAME"]?.ToString();
                var dataType = staffReader["DATA_TYPE"]?.ToString();
                var nullable = staffReader["NULLABLE"]?.ToString();
                Console.WriteLine($"   {columnName} - {dataType} ({nullable})");
            }
            
            if (!hasStaffData)
            {
                Console.WriteLine("   ⚠️ 没有找到 STAFFLOCATION 表或表为空");
            }
            
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ 错误: {ex.Message}");
        }
    }
}
