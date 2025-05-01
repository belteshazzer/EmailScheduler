using Hangfire;
using Microsoft.AspNetCore.Mvc;
using EmailScheduler.Models.Dtos;
using EmailScheduler.Services.EmailService;
using CsvHelper;
using System.Globalization;

namespace EmailScheduler.Controllers
{
    [ApiController]
    [Route("api/schedule")]
    public class ScheduleController : ControllerBase
    {
        private readonly IEmailSenderService _emailService;

        private readonly IScheduleEmailsService _scheduleEmailService;

        public ScheduleController(IEmailSenderService emailService, IScheduleEmailsService scheduleEmailService)
        {
            _emailService = emailService;
            _scheduleEmailService = scheduleEmailService;
        }

        [HttpPost("schedule-email")]
        public async Task<IActionResult> ScheduleEmailAsync(IFormFile file, [FromBody] EmailsDto emailDto)
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

                    emailDto.RecipientEmails = emailList;

                    await _scheduleEmailService.ScheduleEmailAsync(emailDto);

                    BackgroundJob.Schedule(() => _emailService.SendEmail(emailDto), delay);

                    return Ok("Emails scheduled successfully.");
                }
                catch (Exception ex)
                {
                    return BadRequest($"Error processing the file: {ex.Message}");
                }

            }
            else{
                // Calculate the delay until the scheduled time
                var delay = emailDto.ScheduledTime - DateTime.UtcNow;

                if (delay.TotalSeconds <= 0)
                {
                    return BadRequest("Scheduled time must be in the future.");
                }

                await _scheduleEmailService.ScheduleEmailAsync(emailDto);

                // Schedule the email to be sent at the specified time
                BackgroundJob.Schedule(() => _emailService.SendEmail(emailDto), delay);

                return Ok("Email scheduled successfully.");
            }
        }
    }
}