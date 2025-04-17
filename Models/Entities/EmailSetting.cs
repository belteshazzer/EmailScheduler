namespace EmailScheduler.Models.Entities
{
    public class EmailSetting
    {
        public Guid UserId { get; set; }
        public string SmtpUser { get; set; }
        // public string SmtpPass { get; set; }
        public string SmtpHost { get; set; }
        public int SmtpPort { get; set; }
        public string GoogleId { get; set; }
        public bool EnableSsl { get; set; }
        public bool IsDefault { get; set; } = false;
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? CreatedAt { get; set; }

        public Users User { get; set; }

    }
}