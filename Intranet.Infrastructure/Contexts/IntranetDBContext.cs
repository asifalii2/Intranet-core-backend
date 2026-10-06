using Intranet.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Intranet.Infrastructure.Contexts
{
    public class IntranetDBContext : DbContext
    {
        public IntranetDBContext(DbContextOptions<IntranetDBContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<NotificationUser> NotificationUsers { get; set; }
        public DbSet<RecentActivityUser> RecentActivityUsers { get; set; }
        public DbSet<Domain.Entities.Activity> Activities { get; set; }

        // Intranet_HR legacy tables used by auth
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Login> Logins { get; set; }
        public DbSet<LoginHistory> LoginHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User -> Attachment relationship for Avatar to avoid ambiguity
            modelBuilder.Entity<User>()
                .HasOne(u => u.AvatarAttachment)
                .WithMany()
                .HasForeignKey(u => u.AvatarAttachmentId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Employee>()
                .Property(e => e.GlobalId)
                .HasColumnName("Global_ID");
        }
    }
}
