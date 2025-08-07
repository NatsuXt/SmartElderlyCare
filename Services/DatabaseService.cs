using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace RoomDeviceManagement.Services
{
    /// <summary>
    /// 数据库连接服务类
    /// </summary>
    public class DatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService(IConfiguration configuration)
        {
            // 从配置文件读取连接字符串
            _connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? throw new InvalidOperationException("数据库连接字符串未配置");
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
        /// 异步查询多个结果
        /// </summary>
        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? parameters = null) where T : new()
        {
            try
            {
                using var connection = GetConnection();
                await connection.OpenAsync();
                
                using var command = new OracleCommand(sql, connection);
                
                if (parameters != null)
                {
                    AddParameters(command, parameters);
                }

                using var reader = await command.ExecuteReaderAsync();
                return MapResults<T>(reader);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"异步查询失败：{ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 异步查询单个结果
        /// </summary>
        public async Task<T?> QueryFirstAsync<T>(string sql, object? parameters = null) where T : new()
        {
            var results = await QueryAsync<T>(sql, parameters);
            return results.FirstOrDefault();
        }

        /// <summary>
        /// 异步执行非查询SQL
        /// </summary>
        public async Task<int> ExecuteAsync(string sql, object? parameters = null)
        {
            try
            {
                using var connection = GetConnection();
                await connection.OpenAsync();
                
                using var command = new OracleCommand(sql, connection);
                
                if (parameters != null)
                {
                    AddParameters(command, parameters);
                }

                return await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"异步执行SQL失败：{ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 异步查询单个结果
        /// </summary>
        public async Task<T> QuerySingleAsync<T>(string sql, object? parameters = null) where T : new()
        {
            var result = await QuerySingleOrDefaultAsync<T>(sql, parameters);
            if (result == null)
            {
                throw new InvalidOperationException("Query returned no results");
            }
            return result;
        }

        /// <summary>
        /// 异步查询单个结果或默认值
        /// </summary>
        public async Task<T?> QuerySingleOrDefaultAsync<T>(string sql, object? parameters = null) where T : new()
        {
            try
            {
                using var connection = GetConnection();
                await connection.OpenAsync();
                
                using var command = new OracleCommand(sql, connection);
                
                if (parameters != null)
                {
                    AddParameters(command, parameters);
                }

                using var reader = await command.ExecuteReaderAsync();
                var results = MapResults<T>(reader);
                return results.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"异步查询单个结果失败：{ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 异步查询第一个结果或默认值
        /// </summary>
        public async Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? parameters = null) where T : new()
        {
            try
            {
                using var connection = GetConnection();
                await connection.OpenAsync();
                
                using var command = new OracleCommand(sql, connection);
                
                if (parameters != null)
                {
                    AddParameters(command, parameters);
                }
                
                using var reader = await command.ExecuteReaderAsync();
                var results = MapResults<T>(reader);
                return results.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"异步查询第一个结果失败：{ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 添加参数到命令
        /// </summary>
        private void AddParameters(OracleCommand command, object parameters)
        {
            var properties = parameters.GetType().GetProperties();
            
            foreach (var prop in properties)
            {
                var value = prop.GetValue(parameters) ?? DBNull.Value;
                command.Parameters.Add(new OracleParameter($"@{prop.Name}", value));
            }
        }

        /// <summary>
        /// 映射查询结果到对象列表
        /// </summary>
        private IEnumerable<T> MapResults<T>(OracleDataReader reader) where T : new()
        {
            var results = new List<T>();
            var properties = typeof(T).GetProperties();
            
            while (reader.Read())
            {
                var obj = new T();
                
                foreach (var prop in properties)
                {
                    try
                    {
                        // 尝试通过属性名或Column特性获取字段名
                        string columnName = GetColumnName(prop);
                        
                        if (HasColumn(reader, columnName))
                        {
                            var value = reader[columnName];
                            if (value != DBNull.Value)
                            {
                                // 处理类型转换
                                if (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                                {
                                    var underlyingType = Nullable.GetUnderlyingType(prop.PropertyType);
                                    prop.SetValue(obj, Convert.ChangeType(value, underlyingType!));
                                }
                                else
                                {
                                    prop.SetValue(obj, Convert.ChangeType(value, prop.PropertyType));
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"映射属性 {prop.Name} 失败: {ex.Message}");
                    }
                }
                
                results.Add(obj);
            }
            
            return results;
        }

        /// <summary>
        /// 获取列名（支持Column特性）
        /// </summary>
        private string GetColumnName(PropertyInfo prop)
        {
            var columnAttr = prop.GetCustomAttribute<System.ComponentModel.DataAnnotations.Schema.ColumnAttribute>();
            return columnAttr?.Name ?? prop.Name.ToUpper();
        }

        /// <summary>
        /// 检查是否存在指定列
        /// </summary>
        private bool HasColumn(OracleDataReader reader, string columnName)
        {
            try
            {
                return reader.GetOrdinal(columnName) >= 0;
            }
            catch
            {
                return false;
            }
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
