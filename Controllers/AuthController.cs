using EmailScheduler.Services.AccountServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

namespace EmailScheduler.Controllers
{
    [ApiController]
    [Route("api/auth")]
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
                // Log all cookies
                foreach (var cookie in HttpContext.Request.Cookies)
                {
                    _logger.LogInformation($"Cookie: {cookie.Key} = {cookie.Value}");
                }

                // Log the state parameter
                var state = HttpContext.Request.Query["state"];
                _logger.LogInformation($"State received in callback: {state}");

                // Log the correlation cookie
                var correlationCookie = HttpContext.Request.Cookies.Keys
                    .FirstOrDefault(k => k.StartsWith(".AspNetCore.Correlation."));
                _logger.LogInformation($"Correlation cookie: {correlationCookie}");

                // Authenticate the user
                var authenticateResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                if (!authenticateResult.Succeeded)
                {
                    var error = authenticateResult.Failure?.Message ?? "Unknown error";
                    _logger.LogError($"Authentication failed: {error}");
                    return BadRequest(new { Error = $"Google authentication failed: {error}" });
                }

                // Process the user information
                var user = await accountService.SignUpWithGoogleAsync(HttpContext);
                var frontEndUrl = "http://localhost:3000/inbox"; // Replace with your front-end URL

                return Redirect($"{frontEndUrl}/?access_token={authenticateResult.Properties.GetTokenValue("access_token")}&refresh_token={authenticateResult.Properties.GetTokenValue("refresh_token")}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception occurred in GoogleCallback: {ex.Message}");

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