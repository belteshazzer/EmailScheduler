using Hangfire;
using Microsoft.AspNetCore.Mvc;
using EmailScheduler.Models.Dtos;
using EmailScheduler.Services.EmailService;

namespace EmailScheduler.Controllers
{
    [ApiController]
    [Route("api/schedule")]
    public class ScheduleController : ControllerBase
    {
        private readonly IEmailSenderService _emailService;

        public ScheduleController(IEmailSenderService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost("email")]
        public IActionResult ScheduleEmail([FromBody] ScheduledEmailDto emailDto)
        {
            // Calculate the delay until the scheduled time
            var delay = emailDto.ScheduledTime - DateTime.UtcNow;

            if (delay.TotalSeconds <= 0)
            {
                return BadRequest("Scheduled time must be in the future.");
            }

            // Schedule the email to be sent at the specified time
            BackgroundJob.Schedule(() => _emailService.SendScheduledEmail(emailDto), delay);

            return Ok("Email scheduled successfully.");
        }
    }
}