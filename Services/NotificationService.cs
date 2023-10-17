using Microsoft.EntityFrameworkCore;
using System.Globalization;
using UNITEE_BACKEND.DatabaseContext;
using UNITEE_BACKEND.Entities;

namespace UNITEE_BACKEND.Services
{
    public class NotificationService : INotificationService
    {
        private readonly AppDbContext context;

        public NotificationService(AppDbContext dbcontext)
        {
            this.context = dbcontext;
        }

        public async Task<Notification> AddNotification(Notification notification)
        {
            try
            {
                context.Notifications.Add(notification);
                await context.SaveChangesAsync();

                return notification;
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }
        }

        public async Task<List<Notification>> GetNotificationById(int userId)
        {
            try
            {
                var notif = await context.Notifications
                                         .Include(o => o.Order)
                                         .Where(n => n.UserId == userId)
                                         .ToListAsync();

                return notif;
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }
        }
    }
}
