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
        
        // 🔑 使用支持中文字符的连接字符串
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
                
                // 🔑 关键：使用NVarchar2参数类型处理中文字符，明确指定大小
                var roomNumberParam = new OracleParameter(":roomNumber", OracleDbType.NVarchar2, 100) { Value = roomNumber };
                var roomTypeParam = new OracleParameter(":roomType", OracleDbType.NVarchar2, 100) { Value = roomType };
                var statusParam = new OracleParameter(":status", OracleDbType.NVarchar2, 50) { Value = status };
                var bedTypeParam = new OracleParameter(":bedType", OracleDbType.NVarchar2, 100) { Value = bedType };
                
                command.Parameters.Add(roomNumberParam);
                command.Parameters.Add(roomTypeParam);
                command.Parameters.Add(":capacity", OracleDbType.Int32).Value = capacity;
                command.Parameters.Add(statusParam);
                command.Parameters.Add(":rate", OracleDbType.Decimal).Value = rate;
                command.Parameters.Add(bedTypeParam);
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
                
                // 使用NVarchar2参数类型确保中文字符正确处理，明确指定大小
                var deviceNameParam = new OracleParameter("deviceName", OracleDbType.NVarchar2, 200) { Value = deviceName };
                var deviceTypeParam = new OracleParameter("deviceType", OracleDbType.NVarchar2, 100) { Value = deviceType };
                var locationParam = new OracleParameter("location", OracleDbType.NVarchar2, 200) { Value = location };
                var statusParam = new OracleParameter("status", OracleDbType.NVarchar2, 50) { Value = status };
                
                command.Parameters.Add(deviceNameParam);
                command.Parameters.Add(deviceTypeParam);
                command.Parameters.Add(locationParam);
                command.Parameters.Add(statusParam);
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
        /// 删除房间 - 中文兼容版本
        /// </summary>
        public async Task<int> DeleteRoomAsync(int roomId)
        {
            try
            {
                _logger.LogInformation($"🗑️ 中文兼容服务删除房间: {roomId}");

                using var connection = new OracleConnection(ConnectionString);
                await connection.OpenAsync();

                // 检查房间是否存在
                var checkSql = "SELECT COUNT(*) FROM RoomManagement WHERE room_id = :roomId";
                using var checkCommand = new OracleCommand(checkSql, connection);
                checkCommand.Parameters.Add(new OracleParameter("roomId", OracleDbType.Int32) { Value = roomId });
                
                var exists = Convert.ToInt32(await checkCommand.ExecuteScalarAsync()) > 0;
                if (!exists)
                {
                    _logger.LogWarning($"⚠️ 中文兼容服务: 房间 {roomId} 不存在");
                    return 0;
                }

                // 执行删除操作
                var deleteSql = "DELETE FROM RoomManagement WHERE room_id = :roomId";
                using var deleteCommand = new OracleCommand(deleteSql, connection);
                deleteCommand.Parameters.Add(new OracleParameter("roomId", OracleDbType.Int32) { Value = roomId });

                var result = await deleteCommand.ExecuteNonQueryAsync();
                
                _logger.LogInformation($"✅ 中文兼容服务成功删除房间: {roomId}, 影响行数={result}");
                
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ 中文兼容服务删除房间失败: {ex.Message}");
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

        #region 房间入住管理相关方法

        /// <summary>
        /// 入住记录数据模型
        /// </summary>
        public class OccupancyData
        {
            public int OccupancyId { get; set; }
            public int RoomId { get; set; }
            public int ElderlyId { get; set; }
            public string RoomNumber { get; set; } = "";
            public string ElderlyName { get; set; } = "";
            public DateTime CheckInDate { get; set; }
            public DateTime? CheckOutDate { get; set; }
            public string Status { get; set; } = "";
            public string BedNumber { get; set; } = "";
            public string Remarks { get; set; } = "";
            public decimal RoomRate { get; set; }
            public DateTime CreatedDate { get; set; }
            public DateTime UpdatedDate { get; set; }
        }

        /// <summary>
        /// 账单记录数据模型
        /// </summary>
        public class BillingData
        {
            public int BillingId { get; set; }
            public int OccupancyId { get; set; }
            public int ElderlyId { get; set; }
            public string ElderlyName { get; set; } = "";
            public int RoomId { get; set; }
            public string RoomNumber { get; set; } = "";
            public DateTime BillingStartDate { get; set; }
            public DateTime BillingEndDate { get; set; }
            public int Days { get; set; }
            public decimal RoomRate { get; set; }
            public decimal TotalAmount { get; set; }
            public decimal PaidAmount { get; set; }
            public decimal UnpaidAmount { get; set; }
            public string PaymentStatus { get; set; } = "";
            public DateTime? PaymentDate { get; set; }
            public DateTime CreatedDate { get; set; }
            public DateTime UpdatedDate { get; set; }
        }

        /// <summary>
        /// 根据老人ID获取入住记录
        /// </summary>
        public async Task<List<OccupancyData>> GetOccupancyRecordsByElderlyIdAsync(int elderlyId)
        {
            try
            {
                _logger.LogInformation($"🔍 中文兼容服务获取老人ID={elderlyId}的入住记录");

                using var connection = new OracleConnection(ConnectionString);
                await connection.OpenAsync();

                var sql = @"
                    SELECT 
                        ro.occupancy_id, ro.room_id, ro.elderly_id, ro.check_in_date, 
                        ro.check_out_date, ro.status, ro.bed_number, ro.remarks,
                        ro.created_date, ro.updated_date,
                        rm.room_number,
                        ei.name as elderly_name
                    FROM RoomOccupancy ro
                    LEFT JOIN RoomManagement rm ON ro.room_id = rm.room_id
                    LEFT JOIN ElderlyInfo ei ON ro.elderly_id = ei.elderly_id
                    WHERE ro.elderly_id = :elderlyId
                    ORDER BY ro.created_date DESC";

                using var command = new OracleCommand(sql, connection);
                command.Parameters.Add("elderlyId", OracleDbType.Int32).Value = elderlyId;

                var records = new List<OccupancyData>();
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var record = new OccupancyData
                    {
                        OccupancyId = reader.GetInt32("occupancy_id"),
                        RoomId = reader.GetInt32("room_id"),
                        ElderlyId = reader.GetInt32("elderly_id"),
                        CheckInDate = reader.GetDateTime("check_in_date"),
                        CheckOutDate = reader.IsDBNull("check_out_date") ? null : reader.GetDateTime("check_out_date"),
                        Status = reader.GetString("status") ?? "",
                        BedNumber = reader.GetString("bed_number") ?? "",
                        Remarks = reader.GetString("remarks") ?? "",
                        CreatedDate = reader.GetDateTime("created_date"),
                        UpdatedDate = reader.GetDateTime("updated_date"),
                        RoomNumber = reader.GetString("room_number") ?? "",
                        ElderlyName = reader.GetString("elderly_name") ?? ""
                    };
                    records.Add(record);
                }

                _logger.LogInformation($"✅ 中文兼容服务成功获取到 {records.Count} 条入住记录");
                return records;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ 中文兼容服务获取老人ID={elderlyId}入住记录失败");
                throw;
            }
        }

        /// <summary>
        /// 获取所有入住记录（分页）
        /// </summary>
        public async Task<(List<OccupancyData> records, int totalCount)> GetAllOccupancyRecordsAsync(int page, int pageSize, string? status = null)
        {
            try
            {
                _logger.LogInformation($"🔍 中文兼容服务获取所有入住记录: 页码={page}, 大小={pageSize}, 状态={status}");

                using var connection = new OracleConnection(ConnectionString);
                await connection.OpenAsync();

                // 构建查询条件
                var whereClause = "";
                var parameters = new List<OracleParameter>();

                if (!string.IsNullOrEmpty(status))
                {
                    whereClause = "WHERE ro.status = :status";
                    parameters.Add(new OracleParameter("status", status));
                }

                // 获取总数
                var countSql = $@"
                    SELECT COUNT(*) 
                    FROM RoomOccupancy ro
                    INNER JOIN ElderlyInfo ei ON ro.elderly_id = ei.elderly_id
                    INNER JOIN RoomManagement rm ON ro.room_id = rm.room_id
                    {whereClause}";

                int totalCount;
                using (var countCommand = new OracleCommand(countSql, connection))
                {
                    foreach (var param in parameters)
                    {
                        countCommand.Parameters.Add(new OracleParameter(param.ParameterName, param.Value));
                    }
                    totalCount = Convert.ToInt32(await countCommand.ExecuteScalarAsync());
                }

                // 获取分页数据
                var sql = $@"
                    SELECT * FROM (
                        SELECT 
                            ro.occupancy_id,
                            ro.room_id,
                            ro.elderly_id,
                            ro.check_in_date,
                            ro.check_out_date,
                            ro.status,
                            ro.bed_number,
                            ro.remarks,
                            ro.created_date,
                            ro.updated_date,
                            rm.room_number,
                            ei.name as elderly_name,
                            ROW_NUMBER() OVER (ORDER BY ro.created_date DESC) as row_num
                        FROM RoomOccupancy ro
                        INNER JOIN ElderlyInfo ei ON ro.elderly_id = ei.elderly_id
                        INNER JOIN RoomManagement rm ON ro.room_id = rm.room_id
                        {whereClause}
                    ) WHERE row_num BETWEEN :start_row AND :end_row";

                var startRow = (page - 1) * pageSize + 1;
                var endRow = page * pageSize;

                using var command = new OracleCommand(sql, connection);
                foreach (var param in parameters)
                {
                    command.Parameters.Add(new OracleParameter(param.ParameterName, param.Value));
                }
                command.Parameters.Add(new OracleParameter("start_row", startRow));
                command.Parameters.Add(new OracleParameter("end_row", endRow));

                var records = new List<OccupancyData>();
                using var reader = await command.ExecuteReaderAsync();
                
                while (await reader.ReadAsync())
                {
                    var record = new OccupancyData
                    {
                        OccupancyId = reader.GetInt32("occupancy_id"),
                        RoomId = reader.GetInt32("room_id"),
                        ElderlyId = reader.GetInt32("elderly_id"),
                        CheckInDate = reader.GetDateTime("check_in_date"),
                        CheckOutDate = reader.IsDBNull("check_out_date") ? null : reader.GetDateTime("check_out_date"),
                        Status = reader.GetString("status") ?? "",
                        BedNumber = reader.GetString("bed_number") ?? "",
                        Remarks = reader.GetString("remarks") ?? "",
                        CreatedDate = reader.GetDateTime("created_date"),
                        UpdatedDate = reader.GetDateTime("updated_date"),
                        RoomNumber = reader.GetString("room_number") ?? "",
                        ElderlyName = reader.GetString("elderly_name") ?? ""
                    };
                    records.Add(record);
                }

                _logger.LogInformation($"✅ 中文兼容服务成功获取到 {records.Count}/{totalCount} 条入住记录");
                return (records, totalCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ 中文兼容服务获取所有入住记录失败");
                throw;
            }
        }

        /// <summary>
        /// 创建入住记录
        /// </summary>
        public async Task<int> CreateOccupancyRecordAsync(int roomId, int elderlyId, DateTime checkInDate, 
            string bedNumber, string remarks)
        {
            try
            {
                _logger.LogInformation($"🏠 中文兼容服务创建入住记录: 房间ID={roomId}, 老人ID={elderlyId}");

                using var connection = new OracleConnection(ConnectionString);
                await connection.OpenAsync();

                var sql = @"
                    INSERT INTO RoomOccupancy 
                    (room_id, elderly_id, check_in_date, status, bed_number, remarks, created_date, updated_date)
                    VALUES 
                    (:roomId, :elderlyId, :checkInDate, :status, :bedNumber, :remarks, :createdDate, :updatedDate)
                    RETURNING occupancy_id INTO :occupancyId";

                using var command = new OracleCommand(sql, connection);
                command.Parameters.Add("roomId", OracleDbType.Int32).Value = roomId;
                command.Parameters.Add("elderlyId", OracleDbType.Int32).Value = elderlyId;
                command.Parameters.Add("checkInDate", OracleDbType.Date).Value = checkInDate;
                command.Parameters.Add("status", OracleDbType.NVarchar2).Value = "入住中";
                command.Parameters.Add("bedNumber", OracleDbType.NVarchar2).Value = bedNumber ?? "";
                command.Parameters.Add("remarks", OracleDbType.NVarchar2).Value = remarks ?? "";
                command.Parameters.Add("createdDate", OracleDbType.Date).Value = DateTime.Now;
                command.Parameters.Add("updatedDate", OracleDbType.Date).Value = DateTime.Now;
                
                var occupancyIdParam = new OracleParameter("occupancyId", OracleDbType.Int32);
                occupancyIdParam.Direction = ParameterDirection.Output;
                command.Parameters.Add(occupancyIdParam);

                await command.ExecuteNonQueryAsync();
                
                var occupancyId = Convert.ToInt32(((OracleDecimal)occupancyIdParam.Value).Value);
                _logger.LogInformation($"✅ 中文兼容服务成功创建入住记录，ID={occupancyId}");
                
                return occupancyId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ 中文兼容服务创建入住记录失败");
                throw;
            }
        }

        /// <summary>
        /// 更新入住记录（办理退房）
        /// </summary>
        public async Task<bool> UpdateOccupancyRecordAsync(int occupancyId, DateTime checkOutDate, string remarks)
        {
            try
            {
                _logger.LogInformation($"🚪 中文兼容服务更新入住记录: ID={occupancyId}");

                using var connection = new OracleConnection(ConnectionString);
                await connection.OpenAsync();

                var sql = @"
                    UPDATE RoomOccupancy 
                    SET check_out_date = :checkOutDate, 
                        status = :status, 
                        remarks = :remarks, 
                        updated_date = :updatedDate
                    WHERE occupancy_id = :occupancyId";

                using var command = new OracleCommand(sql, connection);
                command.Parameters.Add("checkOutDate", OracleDbType.Date).Value = checkOutDate;
                command.Parameters.Add("status", OracleDbType.NVarchar2).Value = "已退房";
                command.Parameters.Add("remarks", OracleDbType.NVarchar2).Value = remarks ?? "";
                command.Parameters.Add("updatedDate", OracleDbType.Date).Value = DateTime.Now;
                command.Parameters.Add("occupancyId", OracleDbType.Int32).Value = occupancyId;

                var rowsAffected = await command.ExecuteNonQueryAsync();
                
                _logger.LogInformation($"✅ 中文兼容服务成功更新入住记录，影响行数={rowsAffected}");
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ 中文兼容服务更新入住记录失败");
                throw;
            }
        }

        /// <summary>
        /// 获取入住记录详细信息
        /// </summary>
        public async Task<OccupancyData?> GetOccupancyRecordByIdAsync(int occupancyId)
        {
            try
            {
                _logger.LogInformation($"🔍 中文兼容服务获取入住记录: ID={occupancyId}");

                using var connection = new OracleConnection(ConnectionString);
                await connection.OpenAsync();

                var sql = @"
                    SELECT 
                        ro.occupancy_id, ro.room_id, ro.elderly_id, ro.check_in_date, 
                        ro.check_out_date, ro.status, ro.bed_number, ro.remarks,
                        ro.created_date, ro.updated_date,
                        rm.room_number,
                        ei.name as elderly_name
                    FROM RoomOccupancy ro
                    LEFT JOIN RoomManagement rm ON ro.room_id = rm.room_id
                    LEFT JOIN ElderlyInfo ei ON ro.elderly_id = ei.elderly_id
                    WHERE ro.occupancy_id = :occupancyId";

                using var command = new OracleCommand(sql, connection);
                command.Parameters.Add("occupancyId", OracleDbType.Int32).Value = occupancyId;

                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    var record = new OccupancyData
                    {
                        OccupancyId = reader.GetInt32("occupancy_id"),
                        RoomId = reader.GetInt32("room_id"),
                        ElderlyId = reader.GetInt32("elderly_id"),
                        CheckInDate = reader.GetDateTime("check_in_date"),
                        CheckOutDate = reader.IsDBNull("check_out_date") ? null : reader.GetDateTime("check_out_date"),
                        Status = reader.GetString("status") ?? "",
                        BedNumber = reader.GetString("bed_number") ?? "",
                        Remarks = reader.GetString("remarks") ?? "",
                        CreatedDate = reader.GetDateTime("created_date"),
                        UpdatedDate = reader.GetDateTime("updated_date"),
                        RoomNumber = reader.GetString("room_number") ?? "",
                        ElderlyName = reader.GetString("elderly_name") ?? ""
                    };

                    _logger.LogInformation($"✅ 中文兼容服务成功获取入住记录");
                    return record;
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ 中文兼容服务获取入住记录失败");
                throw;
            }
        }

        /// <summary>
        /// 创建账单记录
        /// </summary>
        public async Task<int> CreateBillingRecordAsync(int occupancyId, int elderlyId, int roomId, 
            DateTime billingStartDate, DateTime billingEndDate, decimal roomRate)
        {
            try
            {
                _logger.LogInformation($"💰 中文兼容服务创建账单记录: 入住ID={occupancyId}");

                using var connection = new OracleConnection(ConnectionString);
                await connection.OpenAsync();

                var days = (billingEndDate - billingStartDate).Days + 1;
                var totalAmount = roomRate * days;

                var sql = @"
                    INSERT INTO RoomBilling 
                    (occupancy_id, elderly_id, room_id, billing_start_date, billing_end_date, 
                     days, daily_rate, total_amount, payment_status, billing_date, created_date, updated_date)
                    VALUES 
                    (:occupancyId, :elderlyId, :roomId, :billingStartDate, :billingEndDate, 
                     :days, :roomRate, :totalAmount, :paymentStatus, :billingDate, :createdDate, :updatedDate)
                    RETURNING billing_id INTO :billingId";

                using var command = new OracleCommand(sql, connection);
                command.Parameters.Add("occupancyId", OracleDbType.Int32).Value = occupancyId;
                command.Parameters.Add("elderlyId", OracleDbType.Int32).Value = elderlyId;
                command.Parameters.Add("roomId", OracleDbType.Int32).Value = roomId;
                command.Parameters.Add("billingStartDate", OracleDbType.Date).Value = billingStartDate;
                command.Parameters.Add("billingEndDate", OracleDbType.Date).Value = billingEndDate;
                command.Parameters.Add("days", OracleDbType.Int32).Value = days;
                command.Parameters.Add("roomRate", OracleDbType.Decimal).Value = roomRate;
                command.Parameters.Add("totalAmount", OracleDbType.Decimal).Value = totalAmount;
                command.Parameters.Add("paymentStatus", OracleDbType.NVarchar2).Value = "未支付";
                command.Parameters.Add("billingDate", OracleDbType.Date).Value = DateTime.Now;
                command.Parameters.Add("createdDate", OracleDbType.Date).Value = DateTime.Now;
                command.Parameters.Add("updatedDate", OracleDbType.Date).Value = DateTime.Now;
                
                var billingIdParam = new OracleParameter("billingId", OracleDbType.Int32);
                billingIdParam.Direction = ParameterDirection.Output;
                command.Parameters.Add(billingIdParam);

                await command.ExecuteNonQueryAsync();
                
                var billingId = Convert.ToInt32(((OracleDecimal)billingIdParam.Value).Value);
                _logger.LogInformation($"✅ 中文兼容服务成功创建账单记录，ID={billingId}，金额={totalAmount}");
                
                return billingId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ 中文兼容服务创建账单记录失败");
                throw;
            }
        }

        /// <summary>
        /// 获取账单记录（分页）
        /// </summary>
        public async Task<(List<BillingData> records, int totalCount)> GetBillingRecordsAsync(int page, int pageSize, int? elderlyId = null)
        {
            try
            {
                _logger.LogInformation($"💰 中文兼容服务获取账单记录: 页码={page}, 每页={pageSize}, 老人ID={elderlyId}");

                using var connection = new OracleConnection(ConnectionString);
                await connection.OpenAsync();

                // 构建查询条件
                var whereClause = "";
                var parameters = new List<OracleParameter>();

                if (elderlyId.HasValue)
                {
                    whereClause = " WHERE rb.elderly_id = :elderlyId";
                    parameters.Add(new OracleParameter("elderlyId", OracleDbType.Int32) { Value = elderlyId.Value });
                }

                // 分页查询
                var offset = (page - 1) * pageSize;
                var sql = $@"
                    SELECT 
                        rb.billing_id, rb.occupancy_id, rb.elderly_id, rb.room_id,
                        rb.billing_start_date, rb.billing_end_date, rb.days, 
                        rb.daily_rate, 
                        NVL(rb.total_amount, 0) as total_amount,
                        NVL(rb.paid_amount, 0) as paid_amount,
                        NVL(rb.unpaid_amount, NVL(rb.total_amount, 0)) as unpaid_amount,
                        NVL(rb.payment_status, '未支付') as payment_status, 
                        rb.payment_date,
                        rb.created_date, rb.updated_date,
                        rm.room_number,
                        ei.name as elderly_name
                    FROM RoomBilling rb
                    LEFT JOIN RoomManagement rm ON rb.room_id = rm.room_id
                    LEFT JOIN ElderlyInfo ei ON rb.elderly_id = ei.elderly_id
                    {whereClause}
                    ORDER BY rb.created_date DESC
                    OFFSET {offset} ROWS FETCH NEXT {pageSize} ROWS ONLY";

                using var command = new OracleCommand(sql, connection);
                command.Parameters.AddRange(parameters.ToArray());

                var records = new List<BillingData>();
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var record = new BillingData
                    {
                        BillingId = reader.GetInt32("billing_id"),
                        OccupancyId = reader.GetInt32("occupancy_id"),
                        ElderlyId = reader.GetInt32("elderly_id"),
                        RoomId = reader.GetInt32("room_id"),
                        BillingStartDate = reader.GetDateTime("billing_start_date"),
                        BillingEndDate = reader.GetDateTime("billing_end_date"),
                        Days = reader.GetInt32("days"),
                        RoomRate = reader.GetDecimal("daily_rate"),
                        TotalAmount = reader.IsDBNull("total_amount") ? 0m : reader.GetDecimal("total_amount"),
                        PaidAmount = reader.IsDBNull("paid_amount") ? 0m : reader.GetDecimal("paid_amount"),
                        UnpaidAmount = reader.IsDBNull("unpaid_amount") ? 0m : reader.GetDecimal("unpaid_amount"),
                        PaymentStatus = reader.GetString("payment_status") ?? "",
                        PaymentDate = reader.IsDBNull("payment_date") ? null : reader.GetDateTime("payment_date"),
                        CreatedDate = reader.GetDateTime("created_date"),
                        UpdatedDate = reader.GetDateTime("updated_date"),
                        RoomNumber = reader.GetString("room_number") ?? "",
                        ElderlyName = reader.GetString("elderly_name") ?? ""
                    };
                    records.Add(record);
                }

                // 获取总数
                var countSql = $"SELECT COUNT(*) FROM RoomBilling rb {whereClause}";
                using var countCommand = new OracleCommand(countSql, connection);
                countCommand.Parameters.AddRange(parameters.ToArray());
                var totalCount = Convert.ToInt32(await countCommand.ExecuteScalarAsync());

                _logger.LogInformation($"✅ 中文兼容服务成功获取到 {records.Count} 条账单记录，总数={totalCount}");
                return (records, totalCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ 中文兼容服务获取账单记录失败");
                throw;
            }
        }

        /// <summary>
        /// 检查是否已存在相同时间段的账单
        /// </summary>
        public async Task<bool> BillingExistsAsync(int occupancyId, DateTime billingStartDate, DateTime billingEndDate)
        {
            try
            {
                using var connection = new OracleConnection(ConnectionString);
                await connection.OpenAsync();

                var sql = @"
                    SELECT COUNT(*) 
                    FROM RoomBilling 
                    WHERE occupancy_id = :occupancyId 
                      AND billing_start_date = :billingStartDate 
                      AND billing_end_date = :billingEndDate";

                using var command = new OracleCommand(sql, connection);
                command.Parameters.Add("occupancyId", OracleDbType.Int32).Value = occupancyId;
                command.Parameters.Add("billingStartDate", OracleDbType.Date).Value = billingStartDate;
                command.Parameters.Add("billingEndDate", OracleDbType.Date).Value = billingEndDate;

                var count = Convert.ToInt32(await command.ExecuteScalarAsync());
                return count > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"检查账单是否存在时发生错误");
                throw;
            }
        }

        /// <summary>
        /// 获取需要生成账单的入住记录
        /// </summary>
        public async Task<List<OccupancyData>> GetOccupancyRecordsForBillingAsync(DateTime billingStartDate, DateTime billingEndDate, int? elderlyId = null)
        {
            try
            {
                _logger.LogInformation($"💰 中文兼容服务获取需要生成账单的入住记录: {billingStartDate:yyyy-MM-dd} 到 {billingEndDate:yyyy-MM-dd}");

                using var connection = new OracleConnection(ConnectionString);
                await connection.OpenAsync();

                var whereClause = "WHERE ro.check_in_date <= :billingEndDate AND (ro.check_out_date IS NULL OR ro.check_out_date >= :billingStartDate)";
                var parameters = new List<OracleParameter>
                {
                    new OracleParameter("billingEndDate", OracleDbType.Date) { Value = billingEndDate },
                    new OracleParameter("billingStartDate", OracleDbType.Date) { Value = billingStartDate }
                };

                if (elderlyId.HasValue)
                {
                    whereClause += " AND ro.elderly_id = :elderlyId";
                    parameters.Add(new OracleParameter("elderlyId", OracleDbType.Int32) { Value = elderlyId.Value });
                }

                var sql = $@"
                    SELECT 
                        ro.occupancy_id, ro.room_id, ro.elderly_id, ro.check_in_date, 
                        ro.check_out_date, ro.status, ro.bed_number, ro.remarks,
                        ro.created_date, ro.updated_date,
                        rm.room_number, rm.rate as room_rate,
                        ei.name as elderly_name
                    FROM RoomOccupancy ro
                    LEFT JOIN RoomManagement rm ON ro.room_id = rm.room_id
                    LEFT JOIN ElderlyInfo ei ON ro.elderly_id = ei.elderly_id
                    {whereClause}
                    ORDER BY ro.created_date DESC";

                using var command = new OracleCommand(sql, connection);
                command.Parameters.AddRange(parameters.ToArray());

                var records = new List<OccupancyData>();
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var record = new OccupancyData
                    {
                        OccupancyId = reader.GetInt32("occupancy_id"),
                        RoomId = reader.GetInt32("room_id"),
                        ElderlyId = reader.GetInt32("elderly_id"),
                        CheckInDate = reader.GetDateTime("check_in_date"),
                        CheckOutDate = reader.IsDBNull("check_out_date") ? null : reader.GetDateTime("check_out_date"),
                        Status = reader.IsDBNull("status") ? "" : reader.GetString("status"),
                        BedNumber = reader.IsDBNull("bed_number") ? "" : reader.GetString("bed_number"),
                        Remarks = reader.IsDBNull("remarks") ? "" : reader.GetString("remarks"),
                        CreatedDate = reader.GetDateTime("created_date"),
                        UpdatedDate = reader.GetDateTime("updated_date"),
                        RoomNumber = reader.IsDBNull("room_number") ? "" : reader.GetString("room_number"),
                        RoomRate = reader.IsDBNull("room_rate") ? 0 : reader.GetDecimal("room_rate"),
                        ElderlyName = reader.IsDBNull("elderly_name") ? "" : reader.GetString("elderly_name")
                    };
                    records.Add(record);
                }

                _logger.LogInformation($"✅ 中文兼容服务成功获取到 {records.Count} 条需要生成账单的入住记录");
                return records;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ 中文兼容服务获取需要生成账单的入住记录失败");
                throw;
            }
        }

        /// <summary>
        /// 获取房间费率
        /// </summary>
        public async Task<decimal> GetRoomRateAsync(int roomId)
        {
            try
            {
                using var connection = new OracleConnection(ConnectionString);
                await connection.OpenAsync();

                var sql = "SELECT rate FROM RoomManagement WHERE room_id = :roomId";
                using var command = new OracleCommand(sql, connection);
                command.Parameters.Add("roomId", OracleDbType.Int32).Value = roomId;

                var result = await command.ExecuteScalarAsync();
                return result == DBNull.Value ? 100 : Convert.ToDecimal(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ 获取房间费率失败");
                return 100; // 默认费率
            }
        }

        #endregion

        #region 💰 支付管理功能

        /// <summary>
        /// 处理账单支付 - 支持全额支付和部分支付
        /// </summary>
        public async Task<object> ProcessBillingPaymentAsync(int billingId, decimal paymentAmount, 
            string paymentMethod, string? remarks = null)
        {
            try
            {
                _logger.LogInformation($"💰 处理账单支付: 账单ID={billingId}, 金额={paymentAmount}, 方式={paymentMethod}");

                using var connection = new OracleConnection(ConnectionString);
                await connection.OpenAsync();

                // 1. 先获取当前账单信息
                var getBillingSql = @"
                    SELECT billing_id, 
                           NVL(total_amount, 0) as total_amount, 
                           NVL(paid_amount, 0) as paid_amount, 
                           NVL(unpaid_amount, NVL(total_amount, 0)) as unpaid_amount, 
                           NVL(payment_status, '未支付') as payment_status
                    FROM RoomBilling 
                    WHERE billing_id = :billingId";

                using var getBillingCommand = new OracleCommand(getBillingSql, connection);
                getBillingCommand.Parameters.Add(":billingId", OracleDbType.Int32).Value = billingId;

                using var reader = await getBillingCommand.ExecuteReaderAsync();
                if (!await reader.ReadAsync())
                {
                    throw new ArgumentException($"未找到账单ID: {billingId}");
                }

                var totalAmount = reader.IsDBNull("total_amount") ? 0m : reader.GetDecimal("total_amount");
                var currentPaidAmount = reader.IsDBNull("paid_amount") ? 0m : reader.GetDecimal("paid_amount");
                var unpaidAmount = reader.IsDBNull("unpaid_amount") ? 0m : reader.GetDecimal("unpaid_amount");
                var currentStatus = reader.IsDBNull("payment_status") ? "未支付" : reader.GetString("payment_status");

                reader.Close();

                // 2. 验证支付金额
                if (paymentAmount <= 0)
                {
                    throw new ArgumentException("支付金额必须大于0");
                }

                if (paymentAmount > unpaidAmount)
                {
                    throw new ArgumentException($"支付金额({paymentAmount})不能超过未支付金额({unpaidAmount})");
                }

                // 3. 计算新的支付状态
                var newPaidAmount = currentPaidAmount + paymentAmount;
                var newUnpaidAmount = totalAmount - newPaidAmount;
                
                string newPaymentStatus;
                DateTime? paymentDate = null;

                if (newUnpaidAmount == 0)
                {
                    newPaymentStatus = "已支付";
                    paymentDate = DateTime.Now;
                }
                else if (newPaidAmount > 0)
                {
                    newPaymentStatus = "部分支付";
                }
                else
                {
                    newPaymentStatus = "未支付";
                }

                // 4. 更新账单支付信息
                var updateSql = @"
                    UPDATE RoomBilling 
                    SET paid_amount = :paidAmount,
                        unpaid_amount = :unpaidAmount,
                        payment_status = :paymentStatus,
                        payment_date = :paymentDate,
                        remarks = NVL(:newRemarks, remarks),
                        updated_date = SYSDATE
                    WHERE billing_id = :billingId";

                using var updateCommand = new OracleCommand(updateSql, connection);
                updateCommand.Parameters.Add(":paidAmount", OracleDbType.Decimal).Value = newPaidAmount;
                updateCommand.Parameters.Add(":unpaidAmount", OracleDbType.Decimal).Value = newUnpaidAmount;
                updateCommand.Parameters.Add(":paymentStatus", OracleDbType.NVarchar2).Value = newPaymentStatus;
                updateCommand.Parameters.Add(":paymentDate", OracleDbType.Date).Value = paymentDate ?? (object)DBNull.Value;
                updateCommand.Parameters.Add(":newRemarks", OracleDbType.NVarchar2).Value = remarks ?? (object)DBNull.Value;
                updateCommand.Parameters.Add(":billingId", OracleDbType.Int32).Value = billingId;

                var rowsAffected = await updateCommand.ExecuteNonQueryAsync();

                if (rowsAffected > 0)
                {
                    var result = new
                    {
                        BillingId = billingId,
                        PaymentAmount = paymentAmount,
                        PaymentMethod = paymentMethod,
                        TotalAmount = totalAmount,
                        PaidAmount = newPaidAmount,
                        UnpaidAmount = newUnpaidAmount,
                        PaymentStatus = newPaymentStatus,
                        PaymentDate = paymentDate,
                        Remarks = remarks
                    };

                    _logger.LogInformation($"✅ 账单支付成功: {newPaymentStatus}, 已付{newPaidAmount}/{totalAmount}");
                    return result;
                }

                throw new Exception("支付更新失败");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ 账单支付失败: 账单ID={billingId}");
                throw;
            }
        }

        /// <summary>
        /// 获取支付历史记录
        /// </summary>
        public async Task<List<object>> GetPaymentHistoryAsync(int billingId)
        {
            try
            {
                _logger.LogInformation($"📋 获取账单支付历史: {billingId}");

                using var connection = new OracleConnection(ConnectionString);
                await connection.OpenAsync();

                // 由于我们使用现有表结构，支付历史通过更新时间来模拟
                var sql = @"
                    SELECT 
                        billing_id,
                        paid_amount,
                        payment_date,
                        payment_status,
                        remarks,
                        updated_date,
                        created_date
                    FROM RoomBilling 
                    WHERE billing_id = :billingId
                    ORDER BY updated_date DESC";

                using var command = new OracleCommand(sql, connection);
                command.Parameters.Add(":billingId", OracleDbType.Int32).Value = billingId;

                var history = new List<object>();
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    history.Add(new
                    {
                        BillingId = reader.GetInt32("billing_id"),
                        PaidAmount = reader.GetDecimal("paid_amount"),
                        PaymentDate = reader.IsDBNull("payment_date") ? (DateTime?)null : reader.GetDateTime("payment_date"),
                        PaymentStatus = reader.GetString("payment_status"),
                        Remarks = reader.IsDBNull("remarks") ? null : reader.GetString("remarks"),
                        UpdatedDate = reader.GetDateTime("updated_date"),
                        CreatedDate = reader.GetDateTime("created_date")
                    });
                }

                _logger.LogInformation($"✅ 获取支付历史成功: {history.Count}条记录");
                return history;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ 获取支付历史失败: {billingId}");
                throw;
            }
        }

        /// <summary>
        /// 获取所有老人账务状态
        /// </summary>
        public async Task<List<object>> GetElderlyAccountStatusAsync()
        {
            try
            {
                _logger.LogInformation($"👴 获取所有老人账务状态");

                using var connection = new OracleConnection(ConnectionString);
                await connection.OpenAsync();

                var sql = @"
                    SELECT 
                        ei.elderly_id,
                        ei.name as elderly_name,
                        ro.room_id,
                        rm.room_number,
                        ro.check_in_date,
                        SUM(CASE WHEN rb.remarks NOT LIKE '%押金%' THEN rb.unpaid_amount ELSE 0 END) as total_owed,
                        SUM(CASE WHEN rb.remarks LIKE '%押金收取%' THEN rb.paid_amount 
                                 WHEN rb.remarks LIKE '%押金退还%' THEN -rb.paid_amount
                                 WHEN rb.remarks LIKE '%押金抵扣%' THEN -rb.paid_amount
                                 ELSE 0 END) as deposit_balance,
                        COUNT(CASE WHEN rb.payment_status IN ('未支付', '部分支付') AND rb.remarks NOT LIKE '%押金%' THEN 1 END) as unpaid_billing_count,
                        MAX(rb.payment_date) as last_payment_date
                    FROM ElderlyInfo ei
                    LEFT JOIN RoomOccupancy ro ON ei.elderly_id = ro.elderly_id AND ro.status = '入住中'
                    LEFT JOIN RoomManagement rm ON ro.room_id = rm.room_id
                    LEFT JOIN RoomBilling rb ON ei.elderly_id = rb.elderly_id
                    GROUP BY ei.elderly_id, ei.name, ro.room_id, rm.room_number, ro.check_in_date
                    ORDER BY total_owed DESC";

                using var command = new OracleCommand(sql, connection);
                var accountStatus = new List<object>();

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    accountStatus.Add(new
                    {
                        ElderlyId = reader.GetDecimal("elderly_id"),
                        ElderlyName = reader.GetString("elderly_name"),
                        RoomId = reader.IsDBNull("room_id") ? null : (int?)reader.GetInt32("room_id"),
                        RoomNumber = reader.IsDBNull("room_number") ? null : reader.GetString("room_number"),
                        CheckInDate = reader.IsDBNull("check_in_date") ? null : (DateTime?)reader.GetDateTime("check_in_date"),
                        TotalOwedAmount = reader.IsDBNull("total_owed") ? 0 : reader.GetDecimal("total_owed"),
                        DepositBalance = reader.IsDBNull("deposit_balance") ? 0 : reader.GetDecimal("deposit_balance"),
                        UnpaidBillingCount = reader.GetInt32("unpaid_billing_count"),
                        LastPaymentDate = reader.IsDBNull("last_payment_date") ? null : (DateTime?)reader.GetDateTime("last_payment_date")
                    });
                }

                _logger.LogInformation($"✅ 获取老人账务状态成功: {accountStatus.Count}条记录");
                return accountStatus;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ 获取老人账务状态失败");
                throw;
            }
        }

        #endregion
    }
}
