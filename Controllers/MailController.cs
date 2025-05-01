using EmailScheduler.Models.Dtos;
using EmailScheduler.Services;
using EmailScheduler.Services.EmailService;
using Microsoft.AspNetCore.Mvc;

namespace EmailScheduler.Controllers
{
    [ApiController]
    [Route("api/gmail")]
    public class MailController : ControllerBase
    {
        private readonly GmailService _gmailService;
        private readonly IEmailSenderService _emailSenderService;

        public MailController(GmailService gmailService, IEmailSenderService emailSenderService)
        {
            _gmailService = gmailService;
            _emailSenderService = emailSenderService;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendEmail([FromBody] EmailsDto emailDto)
        {
            if (emailDto == null || emailDto.RecipientEmails == null || !emailDto.RecipientEmails.Any())
            {
                return BadRequest("Email data or recipient list is invalid.");
            }

            try
            {
                var result = await _emailSenderService.SendEmail(emailDto);
                if (result)
                {
                    return Ok("Email sent successfully.");
                }
                else
                {
                    return StatusCode(500, "Failed to send email.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error sending email: {ex.Message}");
            }
        }

        [HttpGet("inbox")]
        public async Task<IActionResult> GetInboxEmails([FromQuery] string accessToken)
        {
            if (string.IsNullOrEmpty(accessToken))
            {
                return BadRequest("Access token is required.");
            }

            var emails = await _gmailService.GetInboxEmailsAsync("me", accessToken);
            return Ok(emails);
        }

        [HttpGet("sent")]
        public async Task<IActionResult> GetSentEmails([FromQuery] string accessToken)
        {
            if (string.IsNullOrEmpty(accessToken))
            {
                return BadRequest("Access token is required.");
            }

            var emails = await _gmailService.GetSentEmailsAsync("me", accessToken);
            return Ok(emails);
        }
    }
}