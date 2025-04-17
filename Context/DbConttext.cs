using EmailScheduler.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmailScheduler.Context
{
    public class AppDbContext :DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<ScheduledEmails> ScheduledEmails { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<EmailSetting> EmailSettings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ScheduledEmails>(e =>
            {
                e.HasKey(e => e.Id);
                e.Property(e => e.Id).HasDefaultValueSql("NEWID()");
                e.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
                e.HasOne(e => e.EmailSetting)
                    .WithMany()
                    .HasForeignKey(e => e.GoogleId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Users>(e =>
            {
                e.HasKey(e => e.Id);
                e.Property(e => e.Id).HasDefaultValueSql("NEWID()");
                e.Property(e => e.Username).IsRequired();
                e.Property(e => e.PasswordHash).IsRequired();
            });

            modelBuilder.Entity<EmailSetting>(e =>
            {
                e.HasKey(e => e.GoogleId); // Define composite key

                e.HasOne(e => e.User) 
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                e.Property(e => e.SmtpUser).IsRequired();
                e.Property(e => e.SmtpHost).IsRequired();
                e.Property(e => e.SmtpPort).IsRequired();
                e.Property(e => e.GoogleId).IsRequired();
                e.Property(e => e.EnableSsl).IsRequired();
                e.Property(e => e.IsDefault).IsRequired();
                e.Property(e => e.AccessToken).IsRequired(false);
                e.Property(e => e.RefreshToken).IsRequired(false);
                e.Property(e => e.CreatedAt).IsRequired(false);
            });
        }
    }
}