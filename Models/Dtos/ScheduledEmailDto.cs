namespace EmailScheduler.Models.Dtos
{
    public class EmailsDto
    {
        public Guid UserId { get; set; }
        public required string GoogleId { get; set; }
        public required string Subject { get; set; }
        public required string Body { get; set; }
        public List<string> RecipientEmails { get; set; } = new List<string>(); // List of recipients
        public DateTime ScheduledTime { get; set; } = DateTime.UtcNow;
        public bool IsSent { get; set; } = false;
        public bool IsRead { get; set; } = false;
        // public int AccountId { get; set; }
    }
}