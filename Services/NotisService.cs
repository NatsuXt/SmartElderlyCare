// Services/NotificationService.cs
using System.Threading.Tasks;

namespace ElderlyCareManagement.Services
{
    public interface INotificationService
    {
        Task SendSosNotificationAsync(int staffId, int sosCallId);
    }

    public class NotificationService : INotificationService
    {
        public async Task SendSosNotificationAsync(int staffId, int sosCallId)
        {
            // 实际应用中这里应该集成短信、邮件或应用内通知系统
            // 这里只是模拟实现
            await Task.Delay(100); // 模拟网络延迟
        }
    }
}