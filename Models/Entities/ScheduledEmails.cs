namespace EmailScheduler.Models.Entities
{
    public class ScheduledEmails
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public required string GoogleId { get; set; }
        public required string Subject { get; set; }
        public required string Body { get; set; }
        public required string RecipientEmail { get; set; }
        public DateTime ScheduledTime { get; set; }
        public bool IsSent { get; set; } = false;
        public bool IsRead { get; set; } = false;
        public int AccountId { get; set; }
        
        public virtual Users User { get; set; }
        public virtual EmailSetting EmailSetting { get; set; }
    }
}