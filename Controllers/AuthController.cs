using EmailScheduler.Services.AccountServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

namespace EmailScheduler.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IAccountService _accountService;

        public AuthController(ILogger<AuthController> logger, IAccountService accountService)
        {
            _accountService = accountService;
            _logger = logger;
        }

        [HttpGet("signup-google")]
        public IActionResult SignUpWithGoogle()
        {
            // Redirect the user to Google's authentication page
            var redirectUrl = Url.Action("GoogleCallback", "Auth");
            var properties = new AuthenticationProperties
            {
                RedirectUri = redirectUrl
            };

            Console.WriteLine($"State sent to Google: {redirectUrl}");
            return Challenge(new AuthenticationProperties { RedirectUri = redirectUrl }, GoogleDefaults.AuthenticationScheme);
        }

    
        [HttpGet("google-callback")]
        public async Task<IActionResult> GoogleCallback([FromServices] IAccountService accountService)
        {
            try
            {
                var state = HttpContext.Request.Query["state"];
                Console.WriteLine($"State received in callback: {state}");


                // Authenticate the user
                var authenticateResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                if (!authenticateResult.Succeeded)
                {
                    var error = authenticateResult.Failure?.Message ?? "Unknown error";
                    Console.WriteLine($"Authentication failed: {error}");
                    return BadRequest(new { Error = $"Google authentication failed: {error}" });
                }

                // Process the user information
                var user = await accountService.SignUpWithGoogleAsync(HttpContext);
                return Ok(new { Message = "User signed up successfully", User = user });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpGet("blank-response")]
        public IActionResult BlankResponse()
        {
            // Return a blank response with status code 200 OK
            return Ok("successfully signed up with Google");
        }
    }
}