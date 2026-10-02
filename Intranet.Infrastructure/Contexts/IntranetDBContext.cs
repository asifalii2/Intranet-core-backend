using Intranet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User -> Attachment relationship for Avatar to avoid ambiguity
            modelBuilder.Entity<User>()
                .HasOne(u => u.AvatarAttachment)
                .WithMany()
                .HasForeignKey(u => u.AvatarAttachmentId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
