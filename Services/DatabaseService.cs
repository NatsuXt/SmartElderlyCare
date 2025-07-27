using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace RoomDeviceManagement.Services
{
    /// <summary>
    /// 数据库连接服务类
    /// </summary>
    public class DatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService()
        {
            // Oracle 18c 连接字符串配置
            _connectionString = "Data Source=47.96.238.102:1521/orcl;User Id=FIBRE;Password=FIBRE2025;";
        }

        public DatabaseService(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// 获取数据库连接
        /// </summary>
        /// <returns>Oracle数据库连接对象</returns>
        public OracleConnection GetConnection()
        {
            return new OracleConnection(_connectionString);
        }

        /// <summary>
        /// 测试数据库连接
        /// </summary>
        /// <returns>连接是否成功</returns>
        public bool TestConnection()
        {
            try
            {
                using var connection = GetConnection();
                connection.Open();
                Console.WriteLine("数据库连接成功！");
                
                // 获取数据库基本信息
                using var command = new OracleCommand("SELECT USER FROM DUAL", connection);
                var currentUser = command.ExecuteScalar();
                Console.WriteLine($"当前用户: {currentUser}");
                
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"数据库连接失败：{ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 检查系统表是否存在
        /// </summary>
        /// <returns>表存在状态字典</returns>
        public Dictionary<string, bool> CheckTablesExist()
        {
            var tableStatus = new Dictionary<string, bool>();
            string[] tables = { "ROOMMANAGEMENT", "DEVICESTATUS", "ELDERLYINFO", "HEALTHMONITORING", "ELECTRONICFENCE", "FENCELOG" };
            
            try
            {
                using var connection = GetConnection();
                connection.Open();
                
                foreach (string table in tables)
                {
                    using var command = new OracleCommand($"SELECT COUNT(*) FROM USER_TABLES WHERE TABLE_NAME = '{table}'", connection);
                    var exists = Convert.ToInt32(command.ExecuteScalar()) > 0;
                    tableStatus[table] = exists;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"检查表状态时发生错误：{ex.Message}");
            }
            
            return tableStatus;
        }

        /// <summary>
        /// 执行查询SQL语句
        /// </summary>
        /// <param name="sql">SQL查询语句</param>
        /// <param name="parameters">参数数组</param>
        /// <returns>查询结果DataTable</returns>
        public DataTable ExecuteQuery(string sql, params OracleParameter[] parameters)
        {
            try
            {
                using var connection = GetConnection();
                connection.Open();
                
                using var command = new OracleCommand(sql, connection);
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }

                using var adapter = new OracleDataAdapter(command);
                var dataTable = new DataTable();
                adapter.Fill(dataTable);
                
                return dataTable;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"执行查询失败：{ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 执行非查询SQL语句（INSERT、UPDATE、DELETE）
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">参数数组</param>
        /// <returns>受影响的行数</returns>
        public int ExecuteNonQuery(string sql, params OracleParameter[] parameters)
        {
            try
            {
                using var connection = GetConnection();
                connection.Open();
                
                using var command = new OracleCommand(sql, connection);
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }

                return command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"执行SQL语句失败：{ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 执行标量查询（返回单个值）
        /// </summary>
        /// <param name="sql">SQL查询语句</param>
        /// <param name="parameters">参数数组</param>
        /// <returns>查询结果的第一行第一列值</returns>
        public object? ExecuteScalar(string sql, params OracleParameter[] parameters)
        {
            try
            {
                using var connection = GetConnection();
                connection.Open();
                
                using var command = new OracleCommand(sql, connection);
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }

                return command.ExecuteScalar();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"执行标量查询失败：{ex.Message}");
                throw;
            }
        }
    }
}
