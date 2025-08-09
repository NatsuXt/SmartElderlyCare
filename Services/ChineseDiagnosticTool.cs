using Oracle.ManagedDataAccess.Client;
using System.Text;

namespace RoomDeviceManagement.Services
{
    /// <summary>
    /// 全面的中文字符编码诊断工具
    /// </summary>
    public static class ChineseDiagnosticTool
    {
        private static readonly string ConnectionString = "Data Source=47.96.238.102:1521/orcl;User Id=application_user;Password=20252025;Connection Timeout=30;Pooling=false;";
        
        public static async Task RunFullDiagnostic()
        {
            Console.WriteLine("🔍 开始全面中文字符编码诊断...\n");
            
            // 1. 检查环境设置
            await CheckEnvironmentSettings();
            
            // 2. 检查数据库字符集
            await CheckDatabaseCharacterSet();
            
            // 3. 测试不同的插入方法
            await TestDifferentInsertMethods();
            
            // 4. 检查现有数据
            await CheckExistingData();
            
            Console.WriteLine("\n✅ 诊断完成");
        }
        
        private static async Task CheckEnvironmentSettings()
        {
            Console.WriteLine("1. 📊 环境设置检查:");
            Console.WriteLine($"   NLS_LANG: {Environment.GetEnvironmentVariable("NLS_LANG") ?? "未设置"}");
            Console.WriteLine($"   ORA_NCHAR_LITERAL_REPLACE: {Environment.GetEnvironmentVariable("ORA_NCHAR_LITERAL_REPLACE") ?? "未设置"}");
            Console.WriteLine($"   控制台编码: {Console.OutputEncoding.EncodingName}");
            Console.WriteLine($"   系统编码: {Encoding.Default.EncodingName}");
            Console.WriteLine();
        }
        
        private static async Task CheckDatabaseCharacterSet()
        {
            Console.WriteLine("2. 🗄️ 数据库字符集检查:");
            try
            {
                using var connection = new OracleConnection(ConnectionString);
                await connection.OpenAsync();
                
                var sql = @"
                    SELECT PARAMETER, VALUE 
                    FROM V$NLS_PARAMETERS 
                    WHERE PARAMETER IN ('NLS_CHARACTERSET', 'NLS_NCHAR_CHARACTERSET')";
                
                using var command = new OracleCommand(sql, connection);
                using var reader = await command.ExecuteReaderAsync();
                
                while (await reader.ReadAsync())
                {
                    Console.WriteLine($"   {reader.GetString(0)}: {reader.GetString(1)}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ❌ 错误: {ex.Message}");
            }
            Console.WriteLine();
        }
        
        private static async Task TestDifferentInsertMethods()
        {
            Console.WriteLine("3. 🧪 测试不同插入方法:");
            
            await TestMethod1_DirectString();
            await TestMethod2_ParameterizedQuery();
            await TestMethod3_UnistrFunction();
            await TestMethod4_NVarchar2Parameter();
        }
        
        private static async Task TestMethod1_DirectString()
        {
            Console.WriteLine("   方法1: 直接字符串插入");
            try
            {
                using var connection = new OracleConnection(ConnectionString);
                await connection.OpenAsync();
                
                var sql = @"
                    INSERT INTO RoomManagement (
                        room_number, room_type, capacity, status, rate, bed_type, floor
                    ) VALUES (
                        'DIAG01', '标准间', 2, '空闲', 200.00, '双人床', 1
                    )";
                
                using var command = new OracleCommand(sql, connection);
                var result = await command.ExecuteNonQueryAsync();
                Console.WriteLine($"      ✅ 插入成功，影响行数: {result}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"      ❌ 错误: {ex.Message}");
            }
        }
        
        private static async Task TestMethod2_ParameterizedQuery()
        {
            Console.WriteLine("   方法2: 参数化查询（默认类型）");
            try
            {
                using var connection = new OracleConnection(ConnectionString);
                await connection.OpenAsync();
                
                var sql = @"
                    INSERT INTO RoomManagement (
                        room_number, room_type, capacity, status, rate, bed_type, floor
                    ) VALUES (
                        :roomNumber, :roomType, :capacity, :status, :rate, :bedType, :floor
                    )";
                
                using var command = new OracleCommand(sql, connection);
                command.Parameters.Add(new OracleParameter("roomNumber", "DIAG02"));
                command.Parameters.Add(new OracleParameter("roomType", "标准间"));
                command.Parameters.Add(new OracleParameter("capacity", 2));
                command.Parameters.Add(new OracleParameter("status", "空闲"));
                command.Parameters.Add(new OracleParameter("rate", 200.00));
                command.Parameters.Add(new OracleParameter("bedType", "双人床"));
                command.Parameters.Add(new OracleParameter("floor", 1));
                
                var result = await command.ExecuteNonQueryAsync();
                Console.WriteLine($"      ✅ 插入成功，影响行数: {result}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"      ❌ 错误: {ex.Message}");
            }
        }
        
        private static async Task TestMethod3_UnistrFunction()
        {
            Console.WriteLine("   方法3: UNISTR函数");
            try
            {
                using var connection = new OracleConnection(ConnectionString);
                await connection.OpenAsync();
                
                var sql = @"
                    INSERT INTO RoomManagement (
                        room_number, room_type, capacity, status, rate, bed_type, floor
                    ) VALUES (
                        'DIAG03', UNISTR('\6807\51C6\95F4'), 2, UNISTR('\7A7A\95F2'), 200.00, UNISTR('\53CC\4EBA\5E8A'), 1
                    )";
                
                using var command = new OracleCommand(sql, connection);
                var result = await command.ExecuteNonQueryAsync();
                Console.WriteLine($"      ✅ 插入成功，影响行数: {result}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"      ❌ 错误: {ex.Message}");
            }
        }
        
        private static async Task TestMethod4_NVarchar2Parameter()
        {
            Console.WriteLine("   方法4: NVarchar2参数类型");
            try
            {
                using var connection = new OracleConnection(ConnectionString);
                await connection.OpenAsync();
                
                var sql = @"
                    INSERT INTO RoomManagement (
                        room_number, room_type, capacity, status, rate, bed_type, floor
                    ) VALUES (
                        :roomNumber, :roomType, :capacity, :status, :rate, :bedType, :floor
                    )";
                
                using var command = new OracleCommand(sql, connection);
                command.Parameters.Add(new OracleParameter("roomNumber", "DIAG04"));
                
                var roomTypeParam = new OracleParameter("roomType", OracleDbType.NVarchar2);
                roomTypeParam.Value = "标准间";
                command.Parameters.Add(roomTypeParam);
                
                command.Parameters.Add(new OracleParameter("capacity", 2));
                
                var statusParam = new OracleParameter("status", OracleDbType.NVarchar2);
                statusParam.Value = "空闲";
                command.Parameters.Add(statusParam);
                
                command.Parameters.Add(new OracleParameter("rate", 200.00));
                
                var bedTypeParam = new OracleParameter("bedType", OracleDbType.NVarchar2);
                bedTypeParam.Value = "双人床";
                command.Parameters.Add(bedTypeParam);
                
                command.Parameters.Add(new OracleParameter("floor", 1));
                
                var result = await command.ExecuteNonQueryAsync();
                Console.WriteLine($"      ✅ 插入成功，影响行数: {result}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"      ❌ 错误: {ex.Message}");
            }
            Console.WriteLine();
        }
        
        private static async Task CheckExistingData()
        {
            Console.WriteLine("4. 📋 检查测试数据显示:");
            try
            {
                using var connection = new OracleConnection(ConnectionString);
                await connection.OpenAsync();
                
                var sql = @"
                    SELECT room_number, room_type, status, bed_type 
                    FROM RoomManagement 
                    WHERE room_number LIKE 'DIAG%' 
                    ORDER BY room_number";
                
                using var command = new OracleCommand(sql, connection);
                using var reader = await command.ExecuteReaderAsync();
                
                while (await reader.ReadAsync())
                {
                    var roomNumber = reader.GetString(0);
                    var roomType = reader.GetString(1);
                    var status = reader.GetString(2);
                    var bedType = reader.GetString(3);
                    
                    Console.WriteLine($"   房间 {roomNumber}: {roomType} | {status} | {bedType}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ❌ 错误: {ex.Message}");
            }
        }
    }
}
