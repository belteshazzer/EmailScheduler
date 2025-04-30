using EmailScheduler.Services;
using Microsoft.AspNetCore.Mvc;

namespace EmailScheduler.Controllers
{
    [ApiController]
    [Route("api/gmail")]
    public class GmailController : ControllerBase
    {
        private readonly GmailService _gmailService;

        public GmailController(GmailService gmailService)
        {
            _gmailService = gmailService;
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