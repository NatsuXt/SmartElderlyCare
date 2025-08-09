using Oracle.ManagedDataAccess.Client;
using System.Text;

namespace RoomDeviceManagement.Services
{
    /// <summary>
    /// Oracle 19c 中文字符编码测试工具
    /// </summary>
    public static class Oracle19cChineseTestHelper
    {
        /// <summary>
        /// 测试并修复Oracle 19c中文字符问题
        /// </summary>
        public static void InitializeOracleEnvironment()
        {
            try
            {
                // 设置控制台编码为UTF-8
                Console.OutputEncoding = Encoding.UTF8;
                Console.InputEncoding = Encoding.UTF8;
                
                // 设置Oracle环境变量
                Environment.SetEnvironmentVariable("NLS_LANG", "SIMPLIFIED CHINESE_CHINA.AL32UTF8");
                Environment.SetEnvironmentVariable("ORA_NCHAR_LITERAL_REPLACE", "TRUE");
                Environment.SetEnvironmentVariable("NLS_NCHAR", "AL32UTF8");
                
                Console.WriteLine("✅ Oracle 19c 中文字符环境初始化完成");
                Console.WriteLine($"📝 NLS_LANG: {Environment.GetEnvironmentVariable("NLS_LANG")}");
                Console.WriteLine($"📝 ORA_NCHAR_LITERAL_REPLACE: {Environment.GetEnvironmentVariable("ORA_NCHAR_LITERAL_REPLACE")}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Oracle环境初始化失败: {ex.Message}");
            }
        }
        
        /// <summary>
        /// 测试中文字符插入
        /// </summary>
        public static async Task TestChineseCharacterInsert(string connectionString)
        {
            try
            {
                using var connection = new OracleConnection(connectionString);
                await connection.OpenAsync();
                
                var testSql = @"
                    INSERT INTO RoomManagement (
                        room_number, room_type, capacity, status, rate, bed_type, floor
                    ) VALUES (
                        'TEST_中文', N'标准间测试', 2, N'空闲测试', 200.00, N'单人床测试', 1
                    )";
                
                using var command = new OracleCommand(testSql, connection);
                command.BindByName = true;
                
                var result = await command.ExecuteNonQueryAsync();
                Console.WriteLine($"✅ 中文字符测试插入成功，影响行数: {result}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ 中文字符测试失败: {ex.Message}");
            }
        }
    }
}
