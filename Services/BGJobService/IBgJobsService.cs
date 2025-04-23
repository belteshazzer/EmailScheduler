using EmailScheduler.Models.Entities;

namespace EmailScheduler.Services.BGJobService
{
    public interface IBgJobsService
    {
        Task<bool> SendScheduleEmailAsync(ScheduledEmails email);

    }
}