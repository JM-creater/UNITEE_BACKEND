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
            this.service = notificationService;
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
    }
}
