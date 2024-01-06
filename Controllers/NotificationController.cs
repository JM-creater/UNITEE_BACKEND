using Microsoft.AspNetCore.Mvc;
using UNITEE_BACKEND.Entities;
using UNITEE_BACKEND.Services;

namespace UNITEE_BACKEND.Controllers
{
    [ApiController, Route("[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService service;

        public NotificationController(INotificationService notificationService)
        {
            service = notificationService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateNotification([FromBody] Notification notification)
        {
            try
            {
                var result = await service.AddNotification(notification);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetNotification(int userId)
        {
            try
            {
                var notification = await service.GetNotificationById(userId);

                if (notification == null)
                    return NotFound(new { message = "Notification not found" });

                return Ok(notification);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        [HttpGet("unread/{userId}")]
        public async Task<IActionResult> GetUnreadNotifications(int userId)
        {
            var notifications = await service.GetUnreadNotifications(userId);

            return Ok(notifications);
        }

        [HttpPost("markRead/{userId}")]
        public async Task<IActionResult> MarkNotificationsAsRead(int userId)
        {
            await service.MarkNotificationsAsRead(userId);

            return Ok(); 
        }

        [HttpPost("markReadSupplier/{userId}")]
        public async Task<IActionResult> MarkNotificationsAsReadSupplier(int userId)
        {
            await service.MarkNotificationAsReadSupplier(userId);

            return Ok();
        }

        [HttpGet("supplierUnread/{userId}")]
        public async Task<ActionResult<IEnumerable<Notification>>> GetUnreadNotificationsSupplier(int userId)
        {
            try
            {
                var notifications = await service.GetSupplierUnreadNotifications(userId);

                return Ok(notifications);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        [HttpPut("supplierMarkAsRead/{userId}")]
        public async Task<IActionResult> MarkNotificationsSupplierAsRead(int userId)
        {
            try
            {
                await service.MarkSupplierNotificationsAsRead(userId);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }
    }
}
