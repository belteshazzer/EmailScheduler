namespace EmailScheduler.Models.Dtos
{
    public class ScheduledEmailDto
    {
        public Guid UserId { get; set; }
        public required string GoogleId { get; set; }
        public required string Subject { get; set; }
        public required string Body { get; set; }
        public required string RecipientEmail { get; set; }
        public DateTime ScheduledTime { get; set; }
        public bool IsSent { get; set; } = false;
        public bool IsRead { get; set; } = false;
        // public int AccountId { get; set; }
    }
}