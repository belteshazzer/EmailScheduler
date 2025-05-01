using AutoMapper;
using EmailScheduler.Models.Dtos;
using EmailScheduler.Models.Entities;
using EmailScheduler.Repositories;

namespace EmailScheduler.Services.EmailService
{
    public class ScheduleEmailsService : IScheduleEmailsService
    {
        private readonly IGenericRepository<Emails> _emailRepository;
        private readonly IGenericRepository<Users> _userRepository;
        private readonly IGenericRepository<EmailSetting> _emailSettingRepository;
        private readonly IMapper _mapper;
        
        public ScheduleEmailsService(IGenericRepository<Emails> emailRepository,
            IGenericRepository<Users> userRepository, IGenericRepository<EmailSetting> emailSettingRepository, IMapper mapper)
        {
            _emailRepository = emailRepository;
            _userRepository = userRepository;
            _emailSettingRepository = emailSettingRepository;
            _mapper = mapper;
        }

        public async Task<bool> ScheduleEmailAsync(EmailsDto emailDto)
        {
            var email = _mapper.Map<Emails>(emailDto);
            await _emailRepository.AddAsync(email);
            return true; 
        }

        public async Task<bool> UpdateEmailStatusAsync(Guid emailId, bool isSent, bool isRead)
        {
            var email = await _emailRepository.GetByIdAsync(emailId);
            if (email == null)
            {
                return false; // Email not found
            }
            email.IsSent = isSent;
            email.IsRead = isRead;
            await _emailRepository.UpdateAsync(email);
            return true;
        }

        public async Task<IEnumerable<Emails>> GetScheduledEmailsAsync(Guid userId)
        {
            var emails = await _emailRepository.FindAsync(e => e.UserId == userId);
            if (emails == null || !emails.Any())
            {
                return Enumerable.Empty<Emails>(); // No emails found for the user
            }
            return emails;
        }

        public async Task<Emails?> GetScheduledEmailByIdAsync(Guid emailId)
        {
            var email = await _emailRepository.GetByIdAsync(emailId);
            return email; // Returns null if not found
        }

        public async Task<bool> DeleteScheduledEmailAsync(Guid emailId)
        {
            var email = await _emailRepository.GetByIdAsync(emailId);
            if (email == null)
            {
                return false; // Email not found
            }
            await _emailRepository.Delete(email);
            return true;
        }
    }
}