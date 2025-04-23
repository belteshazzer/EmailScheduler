using EmailScheduler.Models.Dtos;
using EmailScheduler.Models.Entities;

namespace EmailScheduler.Services.EmailService
{
    public interface IScheduleEmailsService
    {
        Task<bool> ScheduleEmailAsync(ScheduledEmailDto email);
        Task<bool> UpdateEmailStatusAsync(Guid emailId, bool isSent, bool isRead);
        Task<IEnumerable<ScheduledEmails>> GetScheduledEmailsAsync(Guid userId);
        Task<ScheduledEmails?> GetScheduledEmailByIdAsync(Guid emailId);
        Task<bool> DeleteScheduledEmailAsync(Guid emailId);
    }
}