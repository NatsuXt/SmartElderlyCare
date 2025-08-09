using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Data;

namespace RoomDeviceManagement.Services
{
    /// <summary>
    /// 中文兼容数据库服务 - 完全仿造ChineseDiagnosticTool的成功实现
    /// 解决Oracle 19c中文字符显示为"???"的问题
    /// </summary>
    public class ChineseCompatibleDatabaseService
    {
        private readonly ILogger<ChineseCompatibleDatabaseService> _logger;
        
        // 🔑 使用与诊断工具完全相同的连接字符串
        private const string ConnectionString = "Data Source=47.96.238.102:1521/orcl;User Id=application_user;Password=20252025;Connection Timeout=30;Pooling=false;";

        public ChineseCompatibleDatabaseService(ILogger<ChineseCompatibleDatabaseService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 房间数据模型 - 简化的数据传输对象
        /// </summary>
        public class RoomData
        {
            public int RoomId { get; set; }
            public string RoomNumber { get; set; } = "";
            public string RoomType { get; set; } = "";
            public int Capacity { get; set; }
            public string Status { get; set; } = "";
            public decimal Rate { get; set; }
            public string BedType { get; set; } = "";
            public int Floor { get; set; }
        }

        /// <summary>
        /// 创建房间 - 完全仿造诊断工具的NVarchar2参数方法
        /// </summary>
        public async Task<int> CreateRoomAsync(string roomNumber, string roomType, int capacity, 
            string status, decimal rate, string bedType, int floor)
        {
            try
            {
                _logger.LogInformation($"🏠 中文兼容服务创建房间: {roomNumber} - {roomType}");

                using var connection = new OracleConnection(ConnectionString);
                await connection.OpenAsync();

                // 🔑 使用与诊断工具完全相同的SQL和参数处理方式
                var sql = @"INSERT INTO RoomManagement (room_number, room_type, capacity, status, rate, bed_type, floor) 
                           VALUES (:roomNumber, :roomType, :capacity, :status, :rate, :bedType, :floor)";

                using var command = new OracleCommand(sql, connection);
                
                // 🔑 关键：使用NVarchar2参数类型处理中文字符
                command.Parameters.Add(":roomNumber", OracleDbType.NVarchar2).Value = roomNumber;
                command.Parameters.Add(":roomType", OracleDbType.NVarchar2).Value = roomType;
                command.Parameters.Add(":capacity", OracleDbType.Int32).Value = capacity;
                command.Parameters.Add(":status", OracleDbType.NVarchar2).Value = status;
                command.Parameters.Add(":rate", OracleDbType.Decimal).Value = rate;
                command.Parameters.Add(":bedType", OracleDbType.NVarchar2).Value = bedType;
                command.Parameters.Add(":floor", OracleDbType.Int32).Value = floor;

                var rowsAffected = await command.ExecuteNonQueryAsync();
                
                if (rowsAffected > 0)
                {
                    // 获取新创建的房间ID
                    var idSql = "SELECT room_id FROM RoomManagement WHERE room_number = :roomNumber";
                    using var idCommand = new OracleCommand(idSql, connection);
                    idCommand.Parameters.Add(":roomNumber", OracleDbType.NVarchar2).Value = roomNumber;
                    
                    var result = await idCommand.ExecuteScalarAsync();
                    var roomId = Convert.ToInt32(result);
                    
                    _logger.LogInformation($"✅ 中文兼容服务成功创建房间: ID={roomId}, 房间号={roomNumber}, 类型={roomType}");
                    return roomId;
                }
                else
                {
                    throw new Exception("插入房间数据失败，影响行数为0");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"中文兼容服务创建房间失败: {roomNumber}");
                throw;
            }
        }

        /// <summary>
        /// 获取房间列表 - 完全仿造诊断工具的字段读取方式
        /// </summary>
        public async Task<List<RoomData>> GetRoomsAsync(string? searchTerm = null)
        {
            try
            {
                _logger.LogInformation($"🔍 中文兼容服务获取房间列表，搜索条件: '{searchTerm}'");

                using var connection = new OracleConnection(ConnectionString);
                await connection.OpenAsync();

                var sql = "SELECT room_id, room_number, room_type, capacity, status, rate, bed_type, floor FROM RoomManagement";
                var parameters = new List<OracleParameter>();

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    sql += " WHERE UPPER(room_number) LIKE UPPER(:search) OR UPPER(room_type) LIKE UPPER(:search) OR UPPER(status) LIKE UPPER(:search)";
                    parameters.Add(new OracleParameter(":search", OracleDbType.NVarchar2) { Value = $"%{searchTerm}%" });
                }

                sql += " ORDER BY room_id";

                using var command = new OracleCommand(sql, connection);
                foreach (var param in parameters)
                {
                    command.Parameters.Add(param);
                }

                using var reader = await command.ExecuteReaderAsync();
                var rooms = new List<RoomData>();

                while (await reader.ReadAsync())
                {
                    // 🔑 使用与诊断工具完全相同的索引读取方式
                    var room = new RoomData
                    {
                        RoomId = reader.GetInt32(0),           // room_id
                        RoomNumber = reader.GetString(1),      // room_number  
                        RoomType = reader.GetString(2),        // room_type
                        Capacity = reader.GetInt32(3),         // capacity
                        Status = reader.GetString(4),          // status
                        Rate = reader.GetDecimal(5),           // rate
                        BedType = reader.GetString(6),         // bed_type
                        Floor = reader.GetInt32(7)             // floor
                    };
                    
                    rooms.Add(room);
                }

                _logger.LogInformation($"✅ 中文兼容服务成功获取 {rooms.Count} 个房间");
                
                // 记录第一个房间的详细信息用于调试
                if (rooms.Count > 0)
                {
                    var firstRoom = rooms[0];
                    _logger.LogInformation($"🔍 第一个房间详情: ID={firstRoom.RoomId}, 房间号='{firstRoom.RoomNumber}', 类型='{firstRoom.RoomType}', 状态='{firstRoom.Status}', 床型='{firstRoom.BedType}'");
                }

                return rooms;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "中文兼容服务获取房间列表失败");
                throw;
            }
        }

        /// <summary>
        /// 根据房间号获取房间信息
        /// </summary>
        public async Task<RoomData?> GetRoomByNumberAsync(string roomNumber)
        {
            try
            {
                _logger.LogInformation($"🔍 中文兼容服务根据房间号获取房间: {roomNumber}");

                using var connection = new OracleConnection(ConnectionString);
                await connection.OpenAsync();

                var sql = "SELECT room_id, room_number, room_type, capacity, status, rate, bed_type, floor FROM RoomManagement WHERE UPPER(room_number) = UPPER(:roomNumber)";
                
                using var command = new OracleCommand(sql, connection);
                command.Parameters.Add(":roomNumber", OracleDbType.NVarchar2).Value = roomNumber;

                using var reader = await command.ExecuteReaderAsync();
                
                if (await reader.ReadAsync())
                {
                    var room = new RoomData
                    {
                        RoomId = reader.GetInt32(0),
                        RoomNumber = reader.GetString(1),
                        RoomType = reader.GetString(2),
                        Capacity = reader.GetInt32(3),
                        Status = reader.GetString(4),
                        Rate = reader.GetDecimal(5),
                        BedType = reader.GetString(6),
                        Floor = reader.GetInt32(7)
                    };

                    _logger.LogInformation($"✅ 中文兼容服务成功获取房间: {room.RoomNumber} - {room.RoomType}");
                    return room;
                }

                _logger.LogInformation($"❌ 中文兼容服务未找到房间: {roomNumber}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"中文兼容服务根据房间号获取房间失败: {roomNumber}");
                throw;
            }
        }

        /// <summary>
        /// 根据房间ID获取房间信息
        /// </summary>
        public async Task<RoomData?> GetRoomByIdAsync(int roomId)
        {
            try
            {
                _logger.LogInformation($"🔍 中文兼容服务根据房间ID获取房间: {roomId}");

                using var connection = new OracleConnection(ConnectionString);
                await connection.OpenAsync();

                var sql = "SELECT room_id, room_number, room_type, capacity, status, rate, bed_type, floor FROM RoomManagement WHERE room_id = :roomId";
                
                using var command = new OracleCommand(sql, connection);
                command.Parameters.Add(":roomId", OracleDbType.Int32).Value = roomId;

                using var reader = await command.ExecuteReaderAsync();
                
                if (await reader.ReadAsync())
                {
                    var room = new RoomData
                    {
                        RoomId = reader.GetInt32(0),
                        RoomNumber = reader.GetString(1),
                        RoomType = reader.GetString(2),
                        Capacity = reader.GetInt32(3),
                        Status = reader.GetString(4),
                        Rate = reader.GetDecimal(5),
                        BedType = reader.GetString(6),
                        Floor = reader.GetInt32(7)
                    };

                    _logger.LogInformation($"✅ 中文兼容服务成功获取房间: {room.RoomNumber} - {room.RoomType}");
                    return room;
                }

                _logger.LogInformation($"❌ 中文兼容服务未找到房间ID: {roomId}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"中文兼容服务根据房间ID获取房间失败: {roomId}");
                throw;
            }
        }

        /// <summary>
        /// 更新房间信息 - 中文兼容版本
        /// </summary>
        public async Task<int> UpdateRoomAsync(int roomId, Dictionary<string, object> updateFields)
        {
            try
            {
                _logger.LogInformation($"📝 中文兼容服务更新房间: ID={roomId}");

                if (!updateFields.Any())
                {
                    _logger.LogWarning("没有提供更新字段");
                    return 0;
                }

                using var connection = new OracleConnection(ConnectionString);
                await connection.OpenAsync();

                // 构建动态SQL更新语句
                var setParts = new List<string>();
                var parameters = new List<OracleParameter>();

                foreach (var field in updateFields)
                {
                    string columnName = field.Key switch
                    {
                        "roomNumber" => "room_number",
                        "roomType" => "room_type",
                        "capacity" => "capacity",
                        "status" => "status",
                        "rate" => "rate",
                        "bedType" => "bed_type",
                        "floor" => "floor",
                        _ => field.Key.ToLower()
                    };

                    setParts.Add($"{columnName} = :{field.Key}");

                    // 根据字段类型设置正确的参数类型
                    if (field.Value is string stringValue)
                    {
                        parameters.Add(new OracleParameter(field.Key, OracleDbType.NVarchar2) { Value = stringValue });
                    }
                    else if (field.Value is int intValue)
                    {
                        parameters.Add(new OracleParameter(field.Key, OracleDbType.Int32) { Value = intValue });
                    }
                    else if (field.Value is decimal decimalValue)
                    {
                        parameters.Add(new OracleParameter(field.Key, OracleDbType.Decimal) { Value = decimalValue });
                    }
                    else
                    {
                        parameters.Add(new OracleParameter(field.Key, OracleDbType.NVarchar2) { Value = field.Value?.ToString() ?? "" });
                    }
                }

                var sql = $"UPDATE RoomManagement SET {string.Join(", ", setParts)} WHERE room_id = :roomId";
                parameters.Add(new OracleParameter("roomId", OracleDbType.Int32) { Value = roomId });

                using var command = new OracleCommand(sql, connection);
                foreach (var param in parameters)
                {
                    command.Parameters.Add(param);
                }

                var rowsAffected = await command.ExecuteNonQueryAsync();
                
                _logger.LogInformation($"✅ 中文兼容服务成功更新房间: ID={roomId}, 影响行数={rowsAffected}");
                return rowsAffected;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"中文兼容服务更新房间失败: ID={roomId}");
                throw;
            }
        }

        /// <summary>
        /// 测试数据库连接和中文字符支持
        /// </summary>
        public async Task<bool> TestChineseCharacterSupportAsync()
        {
            try
            {
                _logger.LogInformation("🧪 测试中文兼容服务的中文字符支持");

                using var connection = new OracleConnection(ConnectionString);
                await connection.OpenAsync();

                // 测试简单的中文查询
                var testSql = "SELECT '测试中文' as test_chinese FROM DUAL";
                using var command = new OracleCommand(testSql, connection);
                
                var result = await command.ExecuteScalarAsync();
                var chineseText = result?.ToString() ?? "";

                _logger.LogInformation($"✅ 中文字符测试结果: '{chineseText}'");
                
                return chineseText == "测试中文";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "中文兼容服务测试失败");
                return false;
            }
        }

        /// <summary>
        /// 诊断当前数据库中的中文显示情况
        /// </summary>
        public async Task<string> DiagnoseChineseDataAsync()
        {
            try
            {
                var rooms = await GetRoomsAsync();
                var diagnosticInfo = new System.Text.StringBuilder();
                
                diagnosticInfo.AppendLine("🔍 中文兼容服务数据库诊断结果:");
                diagnosticInfo.AppendLine($"总房间数: {rooms.Count}");
                
                foreach (var room in rooms.Take(5)) // 只显示前5个房间
                {
                    diagnosticInfo.AppendLine($"房间 {room.RoomId}: 房间号='{room.RoomNumber}', 类型='{room.RoomType}', 状态='{room.Status}', 床型='{room.BedType}'");
                }

                var result = diagnosticInfo.ToString();
                _logger.LogInformation(result);
                return result;
            }
            catch (Exception ex)
            {
                var error = $"诊断失败: {ex.Message}";
                _logger.LogError(ex, error);
                return error;
            }
        }

        #region 设备管理相关方法

        /// <summary>
        /// 设备数据模型
        /// </summary>
        public class DeviceData
        {
            public int DeviceId { get; set; }
            public string DeviceName { get; set; } = "";
            public string DeviceType { get; set; } = "";
            public string Location { get; set; } = "";
            public string Status { get; set; } = "";
            public DateTime LastMaintenanceDate { get; set; }
            public DateTime InstallationDate { get; set; }
        }

        /// <summary>
        /// 获取设备列表 - 中文兼容版本
        /// </summary>
        public async Task<List<DeviceData>> GetDevicesAsync(string searchTerm = "")
        {
            try
            {
                _logger.LogInformation($"🔍 中文兼容服务获取设备列表，搜索条件: '{searchTerm}'");

                using var connection = new OracleConnection(ConnectionString);
                await connection.OpenAsync();

                var whereClause = "";
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    whereClause = "WHERE UPPER(device_name) LIKE '%' || UPPER(:search) || '%' OR UPPER(device_type) LIKE '%' || UPPER(:search) || '%'";
                }

                var sql = $@"
                    SELECT device_id, device_name, device_type, location, status,
                           last_maintenance_date, installation_date
                    FROM DeviceStatus 
                    {whereClause}
                    ORDER BY device_id ASC";

                using var command = new OracleCommand(sql, connection);
                
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    command.Parameters.Add(new OracleParameter("search", OracleDbType.NVarchar2) { Value = searchTerm });
                }

                var devices = new List<DeviceData>();
                using var reader = await command.ExecuteReaderAsync();
                
                while (await reader.ReadAsync())
                {
                    var device = new DeviceData
                    {
                        DeviceId = reader.GetInt32(0),  // device_id
                        DeviceName = reader.IsDBNull(1) ? "" : reader.GetString(1),  // device_name
                        DeviceType = reader.IsDBNull(2) ? "" : reader.GetString(2),  // device_type
                        Location = reader.IsDBNull(3) ? "" : reader.GetString(3),    // location
                        Status = reader.IsDBNull(4) ? "" : reader.GetString(4),      // status
                        LastMaintenanceDate = reader.IsDBNull(5) ? DateTime.MinValue : reader.GetDateTime(5),
                        InstallationDate = reader.IsDBNull(6) ? DateTime.MinValue : reader.GetDateTime(6)
                    };
                    devices.Add(device);
                }

                _logger.LogInformation($"✅ 中文兼容服务成功获取 {devices.Count} 个设备");
                
                // 显示第一个设备的详情用于诊断
                if (devices.Count > 0)
                {
                    var firstDevice = devices[0];
                    _logger.LogInformation($"🔍 第一个设备详情: ID={firstDevice.DeviceId}, 名称='{firstDevice.DeviceName}', 类型='{firstDevice.DeviceType}', 状态='{firstDevice.Status}'");
                }

                return devices;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ 中文兼容服务获取设备列表失败: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 根据ID获取设备 - 中文兼容版本
        /// </summary>
        public async Task<DeviceData?> GetDeviceByIdAsync(int deviceId)
        {
            try
            {
                _logger.LogInformation($"🔍 中文兼容服务根据ID获取设备: {deviceId}");

                using var connection = new OracleConnection(ConnectionString);
                await connection.OpenAsync();

                var sql = @"
                    SELECT device_id, device_name, device_type, location, status,
                           last_maintenance_date, installation_date
                    FROM DeviceStatus 
                    WHERE device_id = :deviceId";

                using var command = new OracleCommand(sql, connection);
                command.Parameters.Add(new OracleParameter("deviceId", OracleDbType.Int32) { Value = deviceId });

                using var reader = await command.ExecuteReaderAsync();
                
                if (await reader.ReadAsync())
                {
                    var device = new DeviceData
                    {
                        DeviceId = reader.GetInt32(0),  // device_id
                        DeviceName = reader.IsDBNull(1) ? "" : reader.GetString(1),  // device_name
                        DeviceType = reader.IsDBNull(2) ? "" : reader.GetString(2),  // device_type
                        Location = reader.IsDBNull(3) ? "" : reader.GetString(3),    // location
                        Status = reader.IsDBNull(4) ? "" : reader.GetString(4),      // status
                        LastMaintenanceDate = reader.IsDBNull(5) ? DateTime.MinValue : reader.GetDateTime(5),
                        InstallationDate = reader.IsDBNull(6) ? DateTime.MinValue : reader.GetDateTime(6)
                    };

                    _logger.LogInformation($"✅ 中文兼容服务成功获取设备: {deviceId} - {device.DeviceType}");
                    return device;
                }

                _logger.LogInformation($"❌ 中文兼容服务未找到设备: {deviceId}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ 中文兼容服务获取设备失败: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 根据设备名称获取设备 - 中文兼容版本
        /// </summary>
        public async Task<DeviceData?> GetDeviceByNameAsync(string deviceName)
        {
            try
            {
                _logger.LogInformation($"🔍 中文兼容服务根据名称获取设备: {deviceName}");

                using var connection = new OracleConnection(ConnectionString);
                await connection.OpenAsync();

                var sql = @"
                    SELECT device_id, device_name, device_type, location, status,
                           last_maintenance_date, installation_date
                    FROM DeviceStatus 
                    WHERE device_name = :deviceName";

                using var command = new OracleCommand(sql, connection);
                command.Parameters.Add(new OracleParameter("deviceName", OracleDbType.NVarchar2) { Value = deviceName });

                using var reader = await command.ExecuteReaderAsync();
                
                if (await reader.ReadAsync())
                {
                    var device = new DeviceData
                    {
                        DeviceId = reader.GetInt32(0),  // device_id
                        DeviceName = reader.IsDBNull(1) ? "" : reader.GetString(1),  // device_name
                        DeviceType = reader.IsDBNull(2) ? "" : reader.GetString(2),  // device_type
                        Location = reader.IsDBNull(3) ? "" : reader.GetString(3),    // location
                        Status = reader.IsDBNull(4) ? "" : reader.GetString(4),      // status
                        LastMaintenanceDate = reader.IsDBNull(5) ? DateTime.MinValue : reader.GetDateTime(5),
                        InstallationDate = reader.IsDBNull(6) ? DateTime.MinValue : reader.GetDateTime(6)
                    };

                    _logger.LogInformation($"✅ 中文兼容服务成功获取设备: {deviceName} - {device.DeviceType}");
                    return device;
                }

                _logger.LogInformation($"❌ 中文兼容服务未找到设备: {deviceName}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ 中文兼容服务获取设备失败: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 更新设备状态 - 中文兼容版本
        /// </summary>
        public async Task UpdateDeviceStatusAsync(int deviceId, string status)
        {
            try
            {
                _logger.LogInformation($"🔄 中文兼容服务更新设备状态: {deviceId} -> {status}");

                using var connection = new OracleConnection(ConnectionString);
                await connection.OpenAsync();

                var sql = @"
                    UPDATE DeviceStatus 
                    SET status = :status, last_maintenance_date = :maintenanceDate
                    WHERE device_id = :deviceId";

                using var command = new OracleCommand(sql, connection);
                
                // 🔑 使用NVarchar2参数传递中文字符串
                command.Parameters.Add(new OracleParameter("status", OracleDbType.NVarchar2) { Value = status });
                command.Parameters.Add(new OracleParameter("maintenanceDate", OracleDbType.Date) { Value = DateTime.Now });
                command.Parameters.Add(new OracleParameter("deviceId", OracleDbType.Int32) { Value = deviceId });

                var rowsAffected = await command.ExecuteNonQueryAsync();
                
                if (rowsAffected > 0)
                {
                    _logger.LogInformation($"✅ 中文兼容服务成功更新设备状态: {deviceId} -> {status}");
                }
                else
                {
                    _logger.LogWarning($"⚠️ 中文兼容服务未找到要更新的设备: {deviceId}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ 中文兼容服务更新设备状态失败: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 创建设备 - 中文兼容版本
        /// </summary>
        public async Task<int> CreateDeviceAsync(string deviceName, string deviceType, string location, 
            string status, DateTime lastMaintenanceDate, DateTime installationDate)
        {
            try
            {
                _logger.LogInformation($"📱 中文兼容服务创建设备: {deviceName} - {deviceType}");

                using var connection = new OracleConnection(ConnectionString);
                await connection.OpenAsync();

                var sql = @"
                    INSERT INTO DeviceStatus (
                        device_name, device_type, location, status, 
                        last_maintenance_date, installation_date
                    ) VALUES (
                        :deviceName, :deviceType, :location, :status,
                        :lastMaintenanceDate, :installationDate
                    )";

                using var command = new OracleCommand(sql, connection);
                
                // 使用NVarchar2参数类型确保中文字符正确处理
                command.Parameters.Add(new OracleParameter("deviceName", OracleDbType.NVarchar2) { Value = deviceName });
                command.Parameters.Add(new OracleParameter("deviceType", OracleDbType.NVarchar2) { Value = deviceType });
                command.Parameters.Add(new OracleParameter("location", OracleDbType.NVarchar2) { Value = location });
                command.Parameters.Add(new OracleParameter("status", OracleDbType.NVarchar2) { Value = status });
                command.Parameters.Add(new OracleParameter("lastMaintenanceDate", OracleDbType.Date) { Value = lastMaintenanceDate });
                command.Parameters.Add(new OracleParameter("installationDate", OracleDbType.Date) { Value = installationDate });

                var result = await command.ExecuteNonQueryAsync();
                
                _logger.LogInformation($"✅ 中文兼容服务成功创建设备: 名称={deviceName}, 类型={deviceType}, 状态={status}");
                
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ 中文兼容服务创建设备失败: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 更新设备 - 中文兼容版本
        /// </summary>
        public async Task<int> UpdateDeviceAsync(int deviceId, Dictionary<string, object> updateFields)
        {
            try
            {
                _logger.LogInformation($"🔧 中文兼容服务更新设备: {deviceId}");

                using var connection = new OracleConnection(ConnectionString);
                await connection.OpenAsync();

                var setParts = new List<string>();
                var parameters = new List<OracleParameter>();

                // 动态构建SET子句
                foreach (var field in updateFields)
                {
                    setParts.Add($"{field.Key} = :{field.Key}");
                    var paramType = field.Key.Contains("date") ? OracleDbType.Date : OracleDbType.NVarchar2;
                    parameters.Add(new OracleParameter(field.Key, paramType) { Value = field.Value });
                }

                if (!setParts.Any())
                {
                    return 0;
                }

                var sql = $"UPDATE DeviceStatus SET {string.Join(", ", setParts)} WHERE device_id = :deviceId";
                parameters.Add(new OracleParameter("deviceId", OracleDbType.Int32) { Value = deviceId });

                using var command = new OracleCommand(sql, connection);
                command.Parameters.AddRange(parameters.ToArray());

                var result = await command.ExecuteNonQueryAsync();
                
                _logger.LogInformation($"✅ 中文兼容服务成功更新设备: {deviceId}, 影响行数={result}");
                
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ 中文兼容服务更新设备失败: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 删除设备 - 中文兼容版本
        /// </summary>
        public async Task<int> DeleteDeviceAsync(int deviceId)
        {
            try
            {
                _logger.LogInformation($"🗑️ 中文兼容服务删除设备: {deviceId}");

                using var connection = new OracleConnection(ConnectionString);
                await connection.OpenAsync();

                var sql = "DELETE FROM DeviceStatus WHERE device_id = :deviceId";

                using var command = new OracleCommand(sql, connection);
                command.Parameters.Add(new OracleParameter("deviceId", OracleDbType.Int32) { Value = deviceId });

                var result = await command.ExecuteNonQueryAsync();
                
                _logger.LogInformation($"✅ 中文兼容服务成功删除设备: {deviceId}, 影响行数={result}");
                
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ 中文兼容服务删除设备失败: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 获取设备统计信息 - 中文兼容版本
        /// </summary>
        public async Task<object> GetDeviceStatisticsAsync()
        {
            try
            {
                _logger.LogInformation($"📊 中文兼容服务获取设备统计信息");

                using var connection = new OracleConnection(ConnectionString);
                await connection.OpenAsync();

                // 统计查询
                var statisticsSql = @"
                    SELECT 
                        COUNT(*) as total_devices,
                        SUM(CASE WHEN UPPER(status) LIKE '%正常%' OR UPPER(status) = 'NORMAL' OR UPPER(status) LIKE '%运行%' THEN 1 ELSE 0 END) as normal_devices,
                        SUM(CASE WHEN UPPER(status) LIKE '%故障%' OR UPPER(status) = 'ERROR' OR UPPER(status) = 'FAULT' THEN 1 ELSE 0 END) as fault_devices,
                        SUM(CASE WHEN UPPER(status) LIKE '%维护%' OR UPPER(status) = 'MAINTENANCE' THEN 1 ELSE 0 END) as maintenance_devices
                    FROM DeviceStatus";

                using var statsCommand = new OracleCommand(statisticsSql, connection);
                using var statsReader = await statsCommand.ExecuteReaderAsync();
                
                int totalDevices = 0, normalDevices = 0, faultDevices = 0, maintenanceDevices = 0;
                
                if (await statsReader.ReadAsync())
                {
                    totalDevices = statsReader.IsDBNull(0) ? 0 : statsReader.GetInt32(0);
                    normalDevices = statsReader.IsDBNull(1) ? 0 : statsReader.GetInt32(1);
                    faultDevices = statsReader.IsDBNull(2) ? 0 : statsReader.GetInt32(2);
                    maintenanceDevices = statsReader.IsDBNull(3) ? 0 : statsReader.GetInt32(3);
                }

                // 类型分布查询
                var typeDistributionSql = @"
                    SELECT device_type, COUNT(*) as count
                    FROM DeviceStatus
                    GROUP BY device_type
                    ORDER BY count DESC";

                using var typeCommand = new OracleCommand(typeDistributionSql, connection);
                using var typeReader = await typeCommand.ExecuteReaderAsync();
                
                var typeDistribution = new List<object>();
                while (await typeReader.ReadAsync())
                {
                    var deviceType = typeReader.IsDBNull(0) ? "" : typeReader.GetString(0);
                    var count = typeReader.IsDBNull(1) ? 0 : typeReader.GetInt32(1);
                    
                    typeDistribution.Add(new { 设备类型 = deviceType, 数量 = count });
                }

                var result = new
                {
                    总设备数 = totalDevices,
                    正常设备 = normalDevices,
                    故障设备 = faultDevices,
                    维护中设备 = maintenanceDevices,
                    设备类型分布 = typeDistribution
                };

                _logger.LogInformation($"✅ 中文兼容服务成功获取设备统计: 总数={totalDevices}, 正常={normalDevices}");
                
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ 中文兼容服务获取设备统计失败: {ex.Message}");
                throw;
            }
        }

        #endregion

        #region 健康监测相关方法

        /// <summary>
        /// 健康监测数据模型
        /// </summary>
        public class HealthData
        {
            public int MonitoringId { get; set; }
            public int ElderlyId { get; set; }
            public int HeartRate { get; set; }
            public string BloodPressure { get; set; } = "";
            public decimal OxygenLevel { get; set; }
            public decimal Temperature { get; set; }
            public DateTime MonitoringDate { get; set; }
            public string Status { get; set; } = "";
        }

        /// <summary>
        /// 创建健康监测记录 - 中文兼容版本
        /// </summary>
        public async Task<int> CreateHealthRecordAsync(int elderlyId, int heartRate, string bloodPressure, 
            decimal oxygenLevel, decimal temperature, DateTime monitoringDate, string status)
        {
            try
            {
                _logger.LogInformation($"💓 中文兼容服务创建健康记录: 老人{elderlyId}, 状态={status}");

                using var connection = new OracleConnection(ConnectionString);
                await connection.OpenAsync();

                var sql = @"
                    INSERT INTO HealthMonitoring (elderly_id, heart_rate, blood_pressure, oxygen_level, temperature, monitoring_date, status)
                    VALUES (:elderlyId, :heartRate, :bloodPressure, :oxygenLevel, :temperature, :monitoringDate, :status)";

                using var command = new OracleCommand(sql, connection);
                
                // 使用NVarchar2参数类型确保中文字符正确处理
                command.Parameters.Add(new OracleParameter("elderlyId", OracleDbType.Int32) { Value = elderlyId });
                command.Parameters.Add(new OracleParameter("heartRate", OracleDbType.Int32) { Value = heartRate });
                command.Parameters.Add(new OracleParameter("bloodPressure", OracleDbType.NVarchar2) { Value = bloodPressure });
                command.Parameters.Add(new OracleParameter("oxygenLevel", OracleDbType.Decimal) { Value = oxygenLevel });
                command.Parameters.Add(new OracleParameter("temperature", OracleDbType.Decimal) { Value = temperature });
                command.Parameters.Add(new OracleParameter("monitoringDate", OracleDbType.Date) { Value = monitoringDate });
                command.Parameters.Add(new OracleParameter("status", OracleDbType.NVarchar2) { Value = status });

                var result = await command.ExecuteNonQueryAsync();
                
                _logger.LogInformation($"✅ 中文兼容服务成功创建健康记录: 老人{elderlyId}, 状态={status}");
                
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ 中文兼容服务创建健康记录失败: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 获取健康监测记录列表 - 中文兼容版本
        /// </summary>
        public async Task<List<HealthData>> GetHealthRecordsAsync(int? elderlyId = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                _logger.LogInformation($"🔍 中文兼容服务获取健康记录: 老人ID={elderlyId}");

                using var connection = new OracleConnection(ConnectionString);
                await connection.OpenAsync();

                var whereConditions = new List<string>();
                var parameters = new List<OracleParameter>();

                if (elderlyId.HasValue)
                {
                    whereConditions.Add("elderly_id = :elderlyId");
                    parameters.Add(new OracleParameter("elderlyId", OracleDbType.Int32) { Value = elderlyId.Value });
                }

                if (startDate.HasValue)
                {
                    whereConditions.Add("monitoring_date >= :startDate");
                    parameters.Add(new OracleParameter("startDate", OracleDbType.Date) { Value = startDate.Value });
                }

                if (endDate.HasValue)
                {
                    whereConditions.Add("monitoring_date <= :endDate");
                    parameters.Add(new OracleParameter("endDate", OracleDbType.Date) { Value = endDate.Value });
                }

                var whereClause = whereConditions.Any() ? "WHERE " + string.Join(" AND ", whereConditions) : "";

                var sql = $@"
                    SELECT monitoring_id, elderly_id, heart_rate, blood_pressure, oxygen_level, 
                           temperature, monitoring_date, status
                    FROM HealthMonitoring 
                    {whereClause}
                    ORDER BY monitoring_date DESC";

                using var command = new OracleCommand(sql, connection);
                command.Parameters.AddRange(parameters.ToArray());

                var healthRecords = new List<HealthData>();
                using var reader = await command.ExecuteReaderAsync();
                
                while (await reader.ReadAsync())
                {
                    var record = new HealthData
                    {
                        MonitoringId = reader.GetInt32(0),     // monitoring_id
                        ElderlyId = reader.GetInt32(1),        // elderly_id
                        HeartRate = reader.GetInt32(2),        // heart_rate
                        BloodPressure = reader.IsDBNull(3) ? "" : reader.GetString(3),  // blood_pressure
                        OxygenLevel = reader.IsDBNull(4) ? 0 : reader.GetDecimal(4),    // oxygen_level
                        Temperature = reader.IsDBNull(5) ? 0 : reader.GetDecimal(5),    // temperature
                        MonitoringDate = reader.GetDateTime(6),   // monitoring_date
                        Status = reader.IsDBNull(7) ? "" : reader.GetString(7)          // status
                    };
                    healthRecords.Add(record);
                }

                _logger.LogInformation($"✅ 中文兼容服务成功获取 {healthRecords.Count} 条健康记录");
                
                return healthRecords;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ 中文兼容服务获取健康记录失败: {ex.Message}");
                throw;
            }
        }

        #endregion

        #region === 电子围栏管理方法 ===

        /// <summary>
        /// 电子围栏数据模型 - 用于中文兼容的数据传输
        /// </summary>
        public class FenceData
        {
            public int FenceId { get; set; }
            public string AreaDefinition { get; set; } = "";
        }

        /// <summary>
        /// 围栏日志数据模型 - 用于中文兼容的数据传输
        /// </summary>
        public class FenceLogData
        {
            public int EventLogId { get; set; }
            public int ElderlyId { get; set; }
            public int FenceId { get; set; }
            public DateTime EntryTime { get; set; }
            public DateTime? ExitTime { get; set; }
            public string EventType { get; set; } = "";
        }

        /// <summary>
        /// 获取老人当前围栏状态 - 中文兼容版本
        /// </summary>
        public async Task<List<dynamic>> GetElderlyLocationStatusAsync()
        {
            try
            {
                _logger.LogInformation("🎯 中文兼容服务获取老人围栏状态");

                using var connection = new OracleConnection(ConnectionString);
                await connection.OpenAsync();

                const string sql = @"
                    SELECT DISTINCT 
                        f.elderly_id,
                        f.fence_id,
                        f.entry_time,
                        f.exit_time,
                        CASE WHEN f.exit_time IS NULL THEN :status_inside ELSE :status_outside END as status
                    FROM (
                        SELECT elderly_id, fence_id, entry_time, exit_time,
                               ROW_NUMBER() OVER (PARTITION BY elderly_id ORDER BY entry_time DESC) as rn
                        FROM FenceLog
                    ) f 
                    WHERE f.rn = 1
                    ORDER BY f.elderly_id";

                using var command = new OracleCommand(sql, connection);

                // 🔑 使用NVarchar2参数传递中文字符串
                command.Parameters.Add(new OracleParameter("status_inside", OracleDbType.NVarchar2) { Value = "在围栏内" });
                command.Parameters.Add(new OracleParameter("status_outside", OracleDbType.NVarchar2) { Value = "已离开围栏" });

                var results = new List<dynamic>();
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var result = new
                    {
                        ElderlyId = reader.GetInt32(0),           // elderly_id
                        Name = $"老人{reader.GetInt32(0)}",       // 构造名称
                        CurrentFenceId = reader.GetInt32(1),      // fence_id
                        LastEntryTime = reader.GetDateTime(2),    // entry_time
                        ExitTime = reader.IsDBNull(3) ? (DateTime?)null : reader.GetDateTime(3), // exit_time
                        EventType = "围栏进出",                    // 固定值，因为表中没有 event_type 字段
                        Status = reader.IsDBNull(4) ? "未知" : reader.GetString(4)                // status
                    };
                    results.Add(result);
                }

                _logger.LogInformation($"✅ 中文兼容服务成功获取 {results.Count} 条围栏状态记录");
                return results;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ 中文兼容服务获取围栏状态失败: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 获取围栏日志 - 中文兼容版本
        /// </summary>
        public async Task<List<FenceLogData>> GetFenceLogsAsync(int? elderlyId = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                _logger.LogInformation($"📋 中文兼容服务获取围栏日志: 老人ID={elderlyId}");

                using var connection = new OracleConnection(ConnectionString);
                await connection.OpenAsync();

                var sql = "SELECT event_log_id, elderly_id, fence_id, entry_time, exit_time FROM FenceLog WHERE 1=1";
                var parameters = new List<OracleParameter>();

                if (elderlyId.HasValue)
                {
                    sql += " AND elderly_id = :elderly_id";
                    parameters.Add(new OracleParameter("elderly_id", OracleDbType.Int32) { Value = elderlyId.Value });
                }

                if (startDate.HasValue)
                {
                    sql += " AND entry_time >= :start_date";
                    parameters.Add(new OracleParameter("start_date", OracleDbType.Date) { Value = startDate.Value });
                }

                if (endDate.HasValue)
                {
                    sql += " AND entry_time <= :end_date";
                    parameters.Add(new OracleParameter("end_date", OracleDbType.Date) { Value = endDate.Value });
                }

                sql += " ORDER BY entry_time DESC";

                using var command = new OracleCommand(sql, connection);
                command.Parameters.AddRange(parameters.ToArray());

                var logs = new List<FenceLogData>();
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var log = new FenceLogData
                    {
                        EventLogId = reader.GetInt32(0),          // event_log_id
                        ElderlyId = reader.GetInt32(1),           // elderly_id
                        FenceId = reader.GetInt32(2),             // fence_id
                        EntryTime = reader.GetDateTime(3),        // entry_time
                        ExitTime = reader.IsDBNull(4) ? null : reader.GetDateTime(4), // exit_time
                        EventType = "围栏进出"                     // 固定值，因为表中没有此字段
                    };
                    logs.Add(log);
                }

                _logger.LogInformation($"✅ 中文兼容服务成功获取 {logs.Count} 条围栏日志");
                return logs;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ 中文兼容服务获取围栏日志失败: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 创建围栏进入记录 - 中文兼容版本
        /// </summary>
        public async Task<int> CreateFenceEntryAsync(int elderlyId, int fenceId, DateTime entryTime, string eventType)
        {
            try
            {
                _logger.LogInformation($"🚪 中文兼容服务创建围栏进入记录: 老人{elderlyId} -> 围栏{fenceId}");

                using var connection = new OracleConnection(ConnectionString);
                await connection.OpenAsync();

                const string sql = @"
                    INSERT INTO FenceLog (elderly_id, fence_id, entry_time) 
                    VALUES (:elderly_id, :fence_id, :entry_time)";

                using var command = new OracleCommand(sql, connection);

                command.Parameters.Add(new OracleParameter("elderly_id", OracleDbType.Int32) { Value = elderlyId });
                command.Parameters.Add(new OracleParameter("fence_id", OracleDbType.Int32) { Value = fenceId });
                command.Parameters.Add(new OracleParameter("entry_time", OracleDbType.Date) { Value = entryTime });
                // 注意：eventType 参数保留以维持接口兼容性，但实际不插入数据库

                var result = await command.ExecuteNonQueryAsync();
                
                _logger.LogInformation($"✅ 中文兼容服务成功创建围栏进入记录: {result} 行受影响");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ 中文兼容服务创建围栏进入记录失败: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 更新围栏离开时间 - 中文兼容版本
        /// </summary>
        public async Task<int> UpdateFenceExitAsync(int elderlyId, int fenceId, DateTime exitTime)
        {
            try
            {
                _logger.LogInformation($"🚪 中文兼容服务更新围栏离开时间: 老人{elderlyId} <- 围栏{fenceId}");

                using var connection = new OracleConnection(ConnectionString);
                await connection.OpenAsync();

                const string sql = @"
                    UPDATE FenceLog 
                    SET exit_time = :exit_time 
                    WHERE elderly_id = :elderly_id 
                      AND fence_id = :fence_id 
                      AND exit_time IS NULL";

                using var command = new OracleCommand(sql, connection);

                command.Parameters.Add(new OracleParameter("exit_time", OracleDbType.Date) { Value = exitTime });
                command.Parameters.Add(new OracleParameter("elderly_id", OracleDbType.Int32) { Value = elderlyId });
                command.Parameters.Add(new OracleParameter("fence_id", OracleDbType.Int32) { Value = fenceId });

                var result = await command.ExecuteNonQueryAsync();
                
                _logger.LogInformation($"✅ 中文兼容服务成功更新围栏离开时间: {result} 行受影响");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ 中文兼容服务更新围栏离开时间失败: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 获取所有围栏信息 - 中文兼容版本
        /// </summary>
        public async Task<List<FenceData>> GetAllFencesAsync()
        {
            try
            {
                _logger.LogInformation("🏰 中文兼容服务获取所有围栏信息");

                using var connection = new OracleConnection(ConnectionString);
                await connection.OpenAsync();

                const string sql = "SELECT fence_id, area_definition FROM ElectronicFence ORDER BY fence_id";

                using var command = new OracleCommand(sql, connection);

                var fences = new List<FenceData>();
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var fence = new FenceData
                    {
                        FenceId = reader.GetInt32(0),             // fence_id
                        AreaDefinition = reader.GetString(1)      // area_definition
                    };
                    fences.Add(fence);
                }

                _logger.LogInformation($"✅ 中文兼容服务成功获取 {fences.Count} 个围栏信息");
                return fences;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ 中文兼容服务获取围栏信息失败: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 获取围栏警报信息 - 中文兼容版本
        /// </summary>
        public async Task<List<dynamic>> GetFenceAlertsAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                _logger.LogInformation("🚨 中文兼容服务获取围栏警报信息");

                using var connection = new OracleConnection(ConnectionString);
                await connection.OpenAsync();

                var sql = @"
                    SELECT 
                        f.event_log_id,
                        f.elderly_id,
                        f.fence_id,
                        f.entry_time,
                        f.exit_time
                    FROM FenceLog f
                    WHERE 1=1";

                var parameters = new List<OracleParameter>();

                if (startDate.HasValue)
                {
                    sql += " AND f.entry_time >= :start_date";
                    parameters.Add(new OracleParameter("start_date", OracleDbType.Date) { Value = startDate.Value });
                }

                if (endDate.HasValue)
                {
                    sql += " AND f.entry_time <= :end_date";
                    parameters.Add(new OracleParameter("end_date", OracleDbType.Date) { Value = endDate.Value });
                }

                sql += " ORDER BY f.entry_time DESC";

                using var command = new OracleCommand(sql, connection);
                command.Parameters.AddRange(parameters.ToArray());

                var alerts = new List<dynamic>();
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var entryTime = reader.GetDateTime(3);
                    var exitTime = reader.IsDBNull(4) ? (DateTime?)null : reader.GetDateTime(4);
                    
                    // 确定警报状态
                    var alertStatus = "正常";
                    var eventType = "围栏进出";
                    
                    if (!exitTime.HasValue && entryTime < DateTime.Now.AddHours(-24))
                    {
                        alertStatus = "长时间滞留警报";
                        eventType = "滞留警报";
                    }
                    
                    var alert = new
                    {
                        EventLogId = reader.GetInt32(0),          // event_log_id
                        ElderlyId = reader.GetInt32(1),           // elderly_id
                        FenceId = reader.GetInt32(2),             // fence_id
                        EntryTime = entryTime,                    // entry_time
                        ExitTime = exitTime,                      // exit_time
                        EventType = eventType,                    // 动态确定
                        AlertStatus = alertStatus                 // 动态确定
                    };
                    alerts.Add(alert);
                }

                _logger.LogInformation($"✅ 中文兼容服务成功获取 {alerts.Count} 条围栏警报");
                return alerts;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ 中文兼容服务获取围栏警报失败: {ex.Message}");
                throw;
            }
        }

        #endregion
    }
}
