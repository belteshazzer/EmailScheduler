using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mail;
using AutoMapper;
using EmailScheduler.Models.Dtos;
using EmailScheduler.Models.Entities;
using EmailScheduler.Repositories;
using Newtonsoft.Json;

namespace EmailScheduler.Services.EmailService
{
    public class EmailSenderService : IEmailSenderService
    {
        private readonly IGenericRepository<EmailSetting> _emailSettingRepository;
        private readonly IMapper _mapper;

        public EmailSenderService(IGenericRepository<EmailSetting> emailSettingRepository, IMapper mapper)
        {
            _emailSettingRepository = emailSettingRepository;
            _mapper = mapper;
        }

        public async Task<bool> SendScheduledEmail(ScheduledEmailDto emailDto)
        {
            try
            {
                var email = _mapper.Map<ScheduledEmails>(emailDto);
                var fromEmail = email.EmailSetting.SmtpUser; // Sender's email address
                // Retrieve email settings for the user
                var emailSettings = (await _emailSettingRepository.FindAsync(x => x.UserId == email.UserId && x.SmtpUser == fromEmail)).FirstOrDefault();
                if (emailSettings == null)
                {
                    Console.WriteLine("Email settings not found for the user.");
                    return false;
                }

                if (string.IsNullOrEmpty(emailSettings.AccessToken))
                {
                    Console.WriteLine("Access token is missing. Cannot send email.");
                    return false;
                }

                if (await IsTokenExpired(emailSettings.AccessToken))
                {
                    Console.WriteLine("Access token expired. Attempting to refresh...");
                    var newAccessToken = await RefreshAccessTokenAsync(emailSettings.RefreshToken);
                    if (string.IsNullOrEmpty(newAccessToken))
                    {
                        Console.WriteLine("Failed to refresh access token.");
                        return false;
                    }

                    // Update the email settings with the new access token
                    emailSettings.AccessToken = newAccessToken;
                    await _emailSettingRepository.UpdateAsync(emailSettings);
                }

                // Configure the SMTP client
                using var smtpClient = new SmtpClient(emailSettings.SmtpHost, emailSettings.SmtpPort)
                {
                    EnableSsl = emailSettings.EnableSsl,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(emailSettings.SmtpUser, emailSettings.AccessToken) // Use Access Token
                };

                // Create the email message
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(emailSettings.SmtpUser),
                    Subject = email.Subject,
                    Body = email.Body,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(email.RecipientEmail); // Recipient's email address

                // Log email details
                Console.WriteLine($"Connecting to SMTP server: {emailSettings.SmtpHost}:{emailSettings.SmtpPort}");
                Console.WriteLine($"Using SSL: {emailSettings.EnableSsl}");
                Console.WriteLine($"Sending email from: {emailSettings.SmtpUser} to: {email.RecipientEmail}");

                // Send the email
                await smtpClient.SendMailAsync(mailMessage);

                return true; // Email sent successfully
            }
            catch (SmtpException smtpEx)
            {
                // Log detailed SMTP error information
                Console.WriteLine($"SMTP error: {smtpEx.Message}");
                if (smtpEx.StatusCode != SmtpStatusCode.GeneralFailure)
                {
                    Console.WriteLine($"SMTP status code: {smtpEx.StatusCode}");
                }
                if (smtpEx.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {smtpEx.InnerException.Message}");
                }
                return false;
            }
            catch (Exception ex)
            {
                // Log general exceptions
                Console.WriteLine($"Error sending email: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                return false;
            }
        }


        private async Task<bool> IsTokenExpired(string accessToken)
        {
            try
            {
                // Decode the JWT token
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(accessToken);

                // Extract the expiration time (exp claim)
                var expClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Exp);
                if (expClaim == null)
                {
                    Console.WriteLine("Expiration claim not found in the token.");
                    return true; // Assume expired if no expiration claim is found
                }

                // Convert the expiration time from Unix timestamp to DateTime
                var expUnixTime = long.Parse(expClaim.Value);
                var expirationTime = DateTimeOffset.FromUnixTimeSeconds(expUnixTime).UtcDateTime;

                // Check if the token is expired
                var currentTime = DateTime.UtcNow;
                if (currentTime >= expirationTime)
                {
                    Console.WriteLine("Access token is expired.");
                    return true;
                }

                Console.WriteLine("Access token is valid.");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error decoding access token: {ex.Message}");
                return true; // Assume expired if there's an error
            }
        }


        private async Task<string> RefreshAccessTokenAsync(string refreshToken)
        {
            try
            {
                using var httpClient = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, "https://oauth2.googleapis.com/token")
                {
                    Content = new FormUrlEncodedContent(new Dictionary<string, string>
                    {
                        { "client_id", "YOUR_GOOGLE_CLIENT_ID" }, // Replace with your Google Client ID
                        { "client_secret", "YOUR_GOOGLE_CLIENT_SECRET" }, // Replace with your Google Client Secret
                        { "refresh_token", refreshToken },
                        { "grant_type", "refresh_token" }
                    })
                };

                var response = await httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var tokenResponse = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseContent);
                    return tokenResponse["access_token"];
                }

                Console.WriteLine($"Failed to refresh access token. Status code: {response.StatusCode}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error refreshing access token: {ex.Message}");
                return null;
            }
        }
    }
}