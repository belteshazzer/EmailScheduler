using EmailScheduler.Models.Dtos;
using EmailScheduler.Models.Entities;

namespace EmailScheduler.Services.EmailService
{
    public interface IEmailSenderService
    {
        Task<bool> SendEmail(EmailsDto emailDto);
    }
}