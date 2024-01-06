using Microsoft.EntityFrameworkCore;
using System.Globalization;
using UNITEE_BACKEND.DatabaseContext;
using UNITEE_BACKEND.Entities;
using UNITEE_BACKEND.Enum;

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
                                         .Include(n => n.Order)
                                            .ThenInclude(n => n.Cart)
                                                .ThenInclude(n => n.Items)
                                                    .ThenInclude(n => n.Product)
                                                        .ThenInclude(n => n.Sizes)
                                         .Include(n => n.Order)
                                            .ThenInclude(n => n.User)
                                         .Include(n => n.Order)
                                            .ThenInclude(n => n.Cart)
                                                .ThenInclude(n => n.Supplier)
                                         .Include(n => n.Order)
                                            .ThenInclude(n => n.OrderItems)
                                         .Where(n => n.UserId == userId)
                                         .OrderByDescending(n => n.DateCreated)
                                         .ToListAsync();

                return notif;
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }
        }
        
        public async Task<IEnumerable<Notification>> GetUnreadNotifications(int userId)
            => await context.Notifications
                                .Include(n => n.Order)
                                .Where(n => n.UserId == userId && !n.IsRead && n.UserRole == UserRole.Customer)
                                .ToListAsync();

        public async Task MarkNotificationsAsRead(int userId)
        {
            var unreadNotifications = context.Notifications
                                             .Where(n => n.UserId == userId && !n.IsRead && n.UserRole == UserRole.Customer);

            foreach (var notification in unreadNotifications)
            {
                notification.IsRead = true;
            }

            await context.SaveChangesAsync();
        }

        public async Task MarkNotificationAsReadSupplier(int userId)
        {
            var unreadNotifications = context.Notifications
                                                   .Where(n => n.UserId == userId && !n.IsRead && n.UserRole == UserRole.Supplier);

            foreach (var notification in unreadNotifications)
            {
                notification.IsRead = true;
            }

            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Notification>> GetSupplierUnreadNotifications(int userId)
            => await context.Notifications
                                .Include(n => n.Order)
                                .Where(n => n.UserId == userId && !n.IsRead && n.UserRole == UserRole.Supplier)
                                .ToListAsync();

        public async Task MarkSupplierNotificationsAsRead(int userId)
        {
            var unreadNotifications = context.Notifications
                                             .Where(n => n.UserId == userId && !n.IsRead && n.UserRole == UserRole.Supplier);

            foreach (var notification in unreadNotifications)
            {
                notification.IsRead = true;
            }

            await context.SaveChangesAsync();
        }

    }
}
