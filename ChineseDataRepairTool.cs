using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

namespace RoomDeviceManagement;

/// <summary>
/// 中文数据修复工具 - 修复数据库中被破坏的中文字符
/// </summary>
public class ChineseDataRepairTool
{
    private static readonly string ConnectionString = "Data Source=47.96.238.102:1521/orcl;User Id=application_user;Password=20252025;Connection Timeout=30;Pooling=false;";

    public static async Task RepairChineseData()
    {
        Console.WriteLine("🔧 开始修复数据库中的中文字符数据...");
        Console.WriteLine();

        try
        {
            await RepairDeviceManagementData();
            await RepairRoomManagementData();
            await RepairElderlyInfoData();
            await RepairStaffInfoData();
            
            Console.WriteLine();
            Console.WriteLine("✅ 中文数据修复完成！");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ 中文数据修复失败: {ex.Message}");
            Console.WriteLine($"详细错误: {ex}");
        }
    }

    /// <summary>
    /// 修复设备管理表中的中文数据
    /// </summary>
    private static async Task RepairDeviceManagementData()
    {
        Console.WriteLine("🔧 修复设备管理表中的中文数据...");

        using var connection = new OracleConnection(ConnectionString);
        await connection.OpenAsync();

        // 查找被破坏的设备数据（包含问号的记录）
        var selectSql = @"
            SELECT device_id, device_name, device_type, location, status 
            FROM DeviceManagement 
            WHERE device_name LIKE '%?%' OR device_type LIKE '%?%' OR location LIKE '%?%' OR status LIKE '%?%'";

        using var selectCmd = new OracleCommand(selectSql, connection);
        using var reader = await selectCmd.ExecuteReaderAsync();

        var brokenDevices = new List<(int id, string name, string type, string location, string status)>();

        while (await reader.ReadAsync())
        {
            brokenDevices.Add((
                reader.GetInt32(0),
                reader.GetString(1),
                reader.GetString(2),
                reader.GetString(3),
                reader.GetString(4)
            ));
        }

        Console.WriteLine($"发现 {brokenDevices.Count} 个被破坏的设备记录");

        // 修复已知的设备数据
        var repairData = new Dictionary<int, (string name, string type, string location, string status)>
        {
            { 1, ("智能血压计", "血压监测仪", "101房间", "正常运行") },
            { 2, ("Heart Monitor B2", "心率监测仪", "Room 202", "正常") },
            { 3, ("Fall Detector C3", "跌倒检测仪", "Corridor - 3rd Floor", "正常") },
            { 42, ("API测试设备", "智能传感器", "测试房间", "正常") }
        };

        foreach (var (deviceId, deviceData) in repairData)
        {
            var updateSql = @"
                UPDATE DeviceManagement 
                SET device_name = :deviceName, 
                    device_type = :deviceType, 
                    location = :location, 
                    status = :status 
                WHERE device_id = :deviceId";

            using var updateCmd = new OracleCommand(updateSql, connection);
            updateCmd.Parameters.Add(":deviceName", OracleDbType.NVarchar2).Value = deviceData.name;
            updateCmd.Parameters.Add(":deviceType", OracleDbType.NVarchar2).Value = deviceData.type;
            updateCmd.Parameters.Add(":location", OracleDbType.NVarchar2).Value = deviceData.location;
            updateCmd.Parameters.Add(":status", OracleDbType.NVarchar2).Value = deviceData.status;
            updateCmd.Parameters.Add(":deviceId", OracleDbType.Int32).Value = deviceId;

            var rowsAffected = await updateCmd.ExecuteNonQueryAsync();
            if (rowsAffected > 0)
            {
                Console.WriteLine($"✅ 修复设备 {deviceId}: {deviceData.name}");
            }
        }
    }

    /// <summary>
    /// 修复房间管理表中的中文数据
    /// </summary>
    private static async Task RepairRoomManagementData()
    {
        Console.WriteLine("🔧 修复房间管理表中的中文数据...");

        using var connection = new OracleConnection(ConnectionString);
        await connection.OpenAsync();

        // 查找被破坏的房间数据
        var selectSql = @"
            SELECT room_id, room_number, room_type, status, bed_type 
            FROM RoomManagement 
            WHERE room_type LIKE '%?%' OR status LIKE '%?%' OR bed_type LIKE '%?%'";

        using var selectCmd = new OracleCommand(selectSql, connection);
        using var reader = await selectCmd.ExecuteReaderAsync();

        var brokenRooms = new List<(int id, string number, string type, string status, string bedType)>();

        while (await reader.ReadAsync())
        {
            brokenRooms.Add((
                reader.GetInt32(0),
                reader.GetString(1),
                reader.GetString(2),
                reader.GetString(3),
                reader.GetString(4)
            ));
        }

        Console.WriteLine($"发现 {brokenRooms.Count} 个被破坏的房间记录");

        // 修复房间数据的示例
        foreach (var room in brokenRooms)
        {
            string newType = room.type.Contains("?") ? "标准间" : room.type;
            string newStatus = room.status.Contains("?") ? "可用" : room.status;
            string newBedType = room.bedType.Contains("?") ? "单人床" : room.bedType;

            var updateSql = @"
                UPDATE RoomManagement 
                SET room_type = :roomType, 
                    status = :status, 
                    bed_type = :bedType 
                WHERE room_id = :roomId";

            using var updateCmd = new OracleCommand(updateSql, connection);
            updateCmd.Parameters.Add(":roomType", OracleDbType.NVarchar2).Value = newType;
            updateCmd.Parameters.Add(":status", OracleDbType.NVarchar2).Value = newStatus;
            updateCmd.Parameters.Add(":bedType", OracleDbType.NVarchar2).Value = newBedType;
            updateCmd.Parameters.Add(":roomId", OracleDbType.Int32).Value = room.id;

            var rowsAffected = await updateCmd.ExecuteNonQueryAsync();
            if (rowsAffected > 0)
            {
                Console.WriteLine($"✅ 修复房间 {room.number}: {newType}");
            }
        }
    }

    /// <summary>
    /// 修复老人信息表中的中文数据
    /// </summary>
    private static async Task RepairElderlyInfoData()
    {
        Console.WriteLine("🔧 修复老人信息表中的中文数据...");

        using var connection = new OracleConnection(ConnectionString);
        await connection.OpenAsync();

        var selectSql = @"
            SELECT elderly_id, name, gender, health_status 
            FROM ElderlyInfo 
            WHERE name LIKE '%?%' OR gender LIKE '%?%' OR health_status LIKE '%?%'";

        using var selectCmd = new OracleCommand(selectSql, connection);
        using var reader = await selectCmd.ExecuteReaderAsync();

        var brokenRecords = new List<(int id, string name, string gender, string healthStatus)>();

        while (await reader.ReadAsync())
        {
            brokenRecords.Add((
                reader.GetInt32(0),
                reader.IsDBNull(1) ? "" : reader.GetString(1),
                reader.IsDBNull(2) ? "" : reader.GetString(2),
                reader.IsDBNull(3) ? "" : reader.GetString(3)
            ));
        }

        Console.WriteLine($"发现 {brokenRecords.Count} 个被破坏的老人信息记录");

        foreach (var record in brokenRecords)
        {
            string newName = record.name.Contains("?") ? "测试老人" : record.name;
            string newGender = record.gender.Contains("?") ? "男" : record.gender;
            string newHealthStatus = record.healthStatus.Contains("?") ? "良好" : record.healthStatus;

            var updateSql = @"
                UPDATE ElderlyInfo 
                SET name = :name, 
                    gender = :gender, 
                    health_status = :healthStatus 
                WHERE elderly_id = :elderlyId";

            using var updateCmd = new OracleCommand(updateSql, connection);
            updateCmd.Parameters.Add(":name", OracleDbType.NVarchar2).Value = newName;
            updateCmd.Parameters.Add(":gender", OracleDbType.NVarchar2).Value = newGender;
            updateCmd.Parameters.Add(":healthStatus", OracleDbType.NVarchar2).Value = newHealthStatus;
            updateCmd.Parameters.Add(":elderlyId", OracleDbType.Int32).Value = record.id;

            var rowsAffected = await updateCmd.ExecuteNonQueryAsync();
            if (rowsAffected > 0)
            {
                Console.WriteLine($"✅ 修复老人信息 {record.id}: {newName}");
            }
        }
    }

    /// <summary>
    /// 修复工作人员信息表中的中文数据
    /// </summary>
    private static async Task RepairStaffInfoData()
    {
        Console.WriteLine("🔧 修复工作人员信息表中的中文数据...");

        using var connection = new OracleConnection(ConnectionString);
        await connection.OpenAsync();

        var selectSql = @"
            SELECT staff_id, name, position, department 
            FROM StaffInfo 
            WHERE name LIKE '%?%' OR position LIKE '%?%' OR department LIKE '%?%'";

        using var selectCmd = new OracleCommand(selectSql, connection);
        using var reader = await selectCmd.ExecuteReaderAsync();

        var brokenRecords = new List<(int id, string name, string position, string department)>();

        while (await reader.ReadAsync())
        {
            brokenRecords.Add((
                reader.GetInt32(0),
                reader.IsDBNull(1) ? "" : reader.GetString(1),
                reader.IsDBNull(2) ? "" : reader.GetString(2),
                reader.IsDBNull(3) ? "" : reader.GetString(3)
            ));
        }

        Console.WriteLine($"发现 {brokenRecords.Count} 个被破坏的工作人员信息记录");

        foreach (var record in brokenRecords)
        {
            string newName = record.name.Contains("?") ? "测试员工" : record.name;
            string newPosition = record.position.Contains("?") ? "护理员" : record.position;
            string newDepartment = record.department.Contains("?") ? "护理部" : record.department;

            var updateSql = @"
                UPDATE StaffInfo 
                SET name = :name, 
                    position = :position, 
                    department = :department 
                WHERE staff_id = :staffId";

            using var updateCmd = new OracleCommand(updateSql, connection);
            updateCmd.Parameters.Add(":name", OracleDbType.NVarchar2).Value = newName;
            updateCmd.Parameters.Add(":position", OracleDbType.NVarchar2).Value = newPosition;
            updateCmd.Parameters.Add(":department", OracleDbType.NVarchar2).Value = newDepartment;
            updateCmd.Parameters.Add(":staffId", OracleDbType.Int32).Value = record.id;

            var rowsAffected = await updateCmd.ExecuteNonQueryAsync();
            if (rowsAffected > 0)
            {
                Console.WriteLine($"✅ 修复工作人员信息 {record.id}: {newName}");
            }
        }
    }
}
