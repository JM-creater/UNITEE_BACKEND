using UNITEE_BACKEND.Entities;

namespace UNITEE_BACKEND.Services
{
    public interface INotificationService
    {
        public Task<Notification> AddNotification(Notification notification);
        public Task<List<Notification>> GetNotificationById(int userId);
        public Task<IEnumerable<Notification>> GetUnreadNotifications(int userId);
        public Task MarkNotificationsAsRead(int userId);
    }
}
