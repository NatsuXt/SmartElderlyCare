using System.Threading.Tasks;

namespace ElderlyCare.Services
{
    public interface INotificationService
    {
        Task SendNotificationAsync(int staffId, string title, string message);
    }

    public class NotificationService : INotificationService
    {
        public async Task SendNotificationAsync(int staffId, string title, string message)
        {
            // 实际项目中这里会集成推送通知系统
            // 例如：发送电子邮件、短信或应用内通知
            await Task.CompletedTask;
        }
    }
}