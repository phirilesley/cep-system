using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CitizenEngagementPortal.Web.Models.Domain;

namespace CitizenEngagementPortal.Web.Models
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Issue> Issues { get; set; }
        public DbSet<IssueCategory> IssueCategories { get; set; }
        public DbSet<IssueMedia> IssueMedia { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure Issue entity
            builder.Entity<Issue>(entity =>
            {
                entity.HasOne(i => i.Citizen)
                    .WithMany(u => u.ReportedIssues)
                    .HasForeignKey(i => i.CitizenId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(i => i.Assignee)
                    .WithMany(u => u.AssignedIssues)
                    .HasForeignKey(i => i.AssignedTo)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Configure IssueMedia entity
            builder.Entity<IssueMedia>(entity =>
            {
                entity.HasOne(m => m.Issue)
                    .WithMany(i => i.Media)
                    .HasForeignKey(m => m.IssueId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Comment entity
            builder.Entity<Comment>(entity =>
            {
                entity.HasOne(c => c.Issue)
                    .WithMany(i => i.Comments)
                    .HasForeignKey(c => c.IssueId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(c => c.Author)
                    .WithMany(u => u.Comments)
                    .HasForeignKey(c => c.AuthorId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure Notification entity
            builder.Entity<Notification>(entity =>
            {
                entity.HasOne(n => n.User)
                    .WithMany(u => u.Notifications)
                    .HasForeignKey(n => n.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(n => n.Issue)
                    .WithMany(i => i.Notifications)
                    .HasForeignKey(n => n.IssueId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Seed data
            builder.Entity<IssueCategory>().HasData(
                new IssueCategory 
                { 
                    Id = "1", 
                    Name = "Infrastructure", 
                    Description = "Roads, bridges, public buildings, etc.", 
                    Icon = "Building", 
                    Color = "#3B82F6" 
                },
                new IssueCategory 
                { 
                    Id = "2", 
                    Name = "Sanitation", 
                    Description = "Waste management, cleanliness, etc.", 
                    Icon = "Trash2", 
                    Color = "#10B981" 
                },
                new IssueCategory 
                { 
                    Id = "3", 
                    Name = "Health", 
                    Description = "Healthcare facilities, public health issues", 
                    Icon = "Heart", 
                    Color = "#EF4444" 
                },
                new IssueCategory 
                { 
                    Id = "4", 
                    Name = "Safety", 
                    Description = "Security, emergency services", 
                    Icon = "Shield", 
                    Color = "#F59E0B" 
                },
                new IssueCategory 
                { 
                    Id = "5", 
                    Name = "Environment", 
                    Description = "Parks, pollution, environmental concerns", 
                    Icon = "TreePine", 
                    Color = "#22C55E" 
                },
                new IssueCategory 
                { 
                    Id = "6", 
                    Name = "Utilities", 
                    Description = "Water, electricity, gas services", 
                    Icon = "Zap", 
                    Color = "#8B5CF6" 
                }
            );
        }
    }
}