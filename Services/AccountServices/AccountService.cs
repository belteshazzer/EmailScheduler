using EmailScheduler.Context;
using EmailScheduler.Models.Entities;
using EmailScheduler.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;

namespace EmailScheduler.Services.AccountServices
{
    public class AccountService : IAccountService
    {
        private readonly IGenericRepository<Users> _userRepository;
        private readonly IGenericRepository<EmailSetting> _emailSettingRepository;

        public AccountService(IGenericRepository<Users> userRepository, IGenericRepository<EmailSetting> emailSettingRepository)
        {
            _userRepository = userRepository;
            _emailSettingRepository = emailSettingRepository;
        }
 
        public async Task<Users> SignUpWithGoogleAsync(HttpContext httpContext)
        {
            // Authenticate the user
            var authenticateResult = await httpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (!authenticateResult.Succeeded)
                throw new Exception("Google authentication failed");

            // Extract user information from claims
            var claims = authenticateResult.Principal?.Identities.FirstOrDefault()?.Claims;
            var email = claims?.FirstOrDefault(c => c.Type == "email")?.Value;
            var name = claims?.FirstOrDefault(c => c.Type == "name")?.Value;
            var googleId = claims?.FirstOrDefault(c => c.Type == "sub")?.Value;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(googleId))
                throw new Exception("Invalid Google user information");

            var accessToken = authenticateResult.Properties.GetTokenValue("access_token");
            if (string.IsNullOrEmpty(accessToken))
                throw new Exception("Failed to retrieve access token");

            var refreshToken = authenticateResult.Properties.GetTokenValue("refresh_token");
            if (string.IsNullOrEmpty(refreshToken))
            {
                throw new Exception("Failed to retrieve refresh token");
            }
            
            // Check if the user already exists
            var existingUser = (await _userRepository.FindAsync(x => x.Username == email)).FirstOrDefault();
            if (existingUser != null)
            {
                throw new Exception("User already exists");
            }

            // Create a new user
            var newUser = new Users
            {
                Username = email,
                FullName = name,
                CreatedAt = DateTime.UtcNow
            };

            // Save the user to the database
            await _userRepository.AddAsync(newUser);

            var userId = (await _userRepository.FindAsync(x => x.Username == email)).FirstOrDefault()?.Id;
            if (userId == null)
                throw new Exception("Failed to retrieve user ID after creation");

            var emailSetting = new EmailSetting
            {
                UserId = userId.Value,
                SmtpHost = "smtp.gmail.com",
                SmtpPort = 587,
                SmtpUser = email,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                EnableSsl = true,
                CreatedAt = DateTime.UtcNow
            };

            return newUser;
        }
    }
}