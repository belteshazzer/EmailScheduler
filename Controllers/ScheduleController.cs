using Hangfire;
using Microsoft.AspNetCore.Mvc;
using EmailScheduler.Models.Dtos;
using EmailScheduler.Services.EmailService;
using CsvHelper;

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
        public IActionResult ScheduleEmail(IFormFile file, [FromBody] ScheduledEmailDto emailDto)
        {
            if (file != null && file.Length > 0)
            {

                // Calculate the delay until the scheduled time
                var delay = emailDto.ScheduledTime - DateTime.UtcNow;

                if (delay.TotalSeconds <= 0)
                {
                    return BadRequest("Scheduled time must be in the future.");
                }


                var emailList = new List<string>();

                try
                {
                    using (var stream = file.OpenReadStream())
                    using (var reader = new StreamReader(stream))
                    using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                    {
                        // Assuming the CSV has a column named "Email"
                        var records = csv.GetRecords<dynamic>();
                        foreach (var record in records)
                        {
                            if (record.Email != null)
                            {
                                emailList.Add(record.Email.ToString());
                            }
                        }
                    }

                    // Send emails to the extracted addresses
                    foreach (var email in emailList)
                    {
                        emailDto.RecipientEmail = email; 

                        BackgroundJob.Schedule(() => _emailService.SendScheduledEmail(emailDto), delay);
                    }
                    return Ok("Emails scheduled successfully.");
                }
                else(
                    // Calculate the delay until the scheduled time
                    var delay = emailDto.ScheduledTime - DateTime.UtcNow;

                    if (delay.TotalSeconds <= 0)
                    {
                        return BadRequest("Scheduled time must be in the future.");
                    }

                    // Schedule the email to be sent at the specified time
                    BackgroundJob.Schedule(() => _emailService.SendScheduledEmail(emailDto), delay);

                    return Ok("Email scheduled successfully.");
                )
            }
        }
    }
}