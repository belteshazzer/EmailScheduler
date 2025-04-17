namespace EmailScheduler.Services.EmailService
{
    public interface IEmailSender
    {
        Task<bool> SendEmail(Guid userId, string fromEmail, string toEmail, string subject, string body);
    }
}