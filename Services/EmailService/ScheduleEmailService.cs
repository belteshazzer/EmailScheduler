using AutoMapper;
using EmailScheduler.Models.Dtos;
using EmailScheduler.Models.Entities;
using EmailScheduler.Repositories;

namespace EmailScheduler.Services.EmailService
{
    public class ScheduleEmailsService : IScheduleEmailsService
    {
        private readonly IGenericRepository<ScheduledEmails> _scheduledEmailRepository;
        private readonly IGenericRepository<Users> _userRepository;
        private readonly IGenericRepository<EmailSetting> _emailSettingRepository;
        private readonly IMapper _mapper;
        
        public ScheduleEmailsService(IGenericRepository<ScheduledEmails> scheduledEmailRepository,
            IGenericRepository<Users> userRepository, IGenericRepository<EmailSetting> emailSettingRepository, IMapper mapper)
        {
            _scheduledEmailRepository = scheduledEmailRepository;
            _userRepository = userRepository;
            _emailSettingRepository = emailSettingRepository;
            _mapper = mapper;
        }

        public async Task<bool> ScheduleEmailAsync(ScheduledEmailDto emailDto)
        {
            var email = _mapper.Map<ScheduledEmails>(emailDto);
            await _scheduledEmailRepository.AddAsync(email);
        }

        public async Task<bool> UpdateEmailStatusAsync(Guid emailId, bool isSent, bool isRead)
        {
            var email = await _scheduledEmailRepository.GetByIdAsync(emailId);
            if (email == null)
            {
                return false; // Email not found
            }
            email.IsSent = isSent;
            email.IsRead = isRead;
            await _scheduledEmailRepository.UpdateAsync(email);
            return true;
        }

        public async Task<IEnumerable<ScheduledEmails>> GetScheduledEmailsAsync(Guid userId)
        {
            var emails = await _scheduledEmailRepository.FindAsync(e => e.UserId == userId);
            if (emails == null || !emails.Any())
            {
                return Enumerable.Empty<ScheduledEmails>(); // No emails found for the user
            }
            return emails;
        }

        public async Task<ScheduledEmails?> GetScheduledEmailByIdAsync(Guid emailId)
        {
            var email = await _scheduledEmailRepository.GetByIdAsync(emailId);
            return email; // Returns null if not found
        }

        public async Task<bool> DeleteScheduledEmailAsync(Guid emailId)
        {
            var email = await _scheduledEmailRepository.GetByIdAsync(emailId);
            if (email == null)
            {
                return false; // Email not found
            }
            await _scheduledEmailRepository.Delete(email);
            return true;
        }
    }
}