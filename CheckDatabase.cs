using Oracle.ManagedDataAccess.Client;
using RoomDeviceManagement.Services;

public class CheckDatabase
{
    public static async Task Main()
    {
        var connectionString = "Data Source=47.96.238.102:1521/orcl;User Id=application_user;Password=oracle_password;";
        var dbService = new DatabaseService(connectionString);
        
        Console.WriteLine("检查RoomManagement表数据：");
        var rooms = await dbService.QueryAsync<dynamic>("SELECT * FROM RoomManagement");
        foreach (var room in rooms)
        {
            Console.WriteLine($"Room ID: {room.ROOM_ID}, Number: {room.ROOM_NUMBER}, Type: {room.ROOM_TYPE}");
        }
        
        Console.WriteLine("\n检查DeviceStatus表数据：");
        var devices = await dbService.QueryAsync<dynamic>("SELECT * FROM DeviceStatus");
        foreach (var device in devices)
        {
            Console.WriteLine($"Device ID: {device.DEVICE_ID}, Name: {device.DEVICE_NAME}, Status: {device.STATUS}");
        }
    }
}
