using EmailScheduler.Models.Dtos;
using EmailScheduler.Models.Entities;

namespace EmailScheduler.Services.EmailService
{
    public interface IScheduleEmailsService
    {
        Task<bool> ScheduleEmailAsync(EmailsDto email);
        Task<bool> UpdateEmailStatusAsync(Guid emailId, bool isSent, bool isRead);
        Task<IEnumerable<Emails>> GetScheduledEmailsAsync(Guid userId);
        Task<Emails?> GetScheduledEmailByIdAsync(Guid emailId);
        Task<bool> DeleteScheduledEmailAsync(Guid emailId);
    }
}