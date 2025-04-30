using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using System.Text;

namespace EmailScheduler.Services
{
    public class GmailService
    {
        private readonly IConfiguration _configuration;

        public GmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<List<string>> GetInboxEmailsAsync(string userId, string accessToken)
        {
            return await GetEmailsAsync(userId, accessToken, "INBOX");
        }

        public async Task<List<string>> GetSentEmailsAsync(string userId, string accessToken)
        {
            return await GetEmailsAsync(userId, accessToken, "SENT");
        }

        private async Task<List<string>> GetEmailsAsync(string userId, string accessToken, string labelId)
        {
            try
            {
                // Initialize the Gmail API client
                var credential = GoogleCredential.FromAccessToken(accessToken);
                var service = new GmailService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "EmailScheduler"
                });

                // Fetch the list of messages with the specified label
                var request = service.Users.Messages.List(userId);
                request.LabelIds = labelId;
                request.MaxResults = 10; // Fetch up to 10 emails
                var response = await request.ExecuteAsync();

                var emailList = new List<string>();

                if (response.Messages != null && response.Messages.Count > 0)
                {
                    foreach (var message in response.Messages)
                    {
                        // Fetch the full message details
                        var messageRequest = service.Users.Messages.Get(userId, message.Id);
                        var messageDetails = await messageRequest.ExecuteAsync();

                        // Extract the subject from the email headers
                        var subjectHeader = messageDetails.Payload.Headers.FirstOrDefault(h => h.Name == "Subject");
                        var subject = subjectHeader?.Value ?? "No Subject";

                        emailList.Add(subject);
                    }
                }

                return emailList;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching emails: {ex.Message}");
                return new List<string>();
            }
        }
    }
}