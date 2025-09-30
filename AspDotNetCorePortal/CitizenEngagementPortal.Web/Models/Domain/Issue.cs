using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CitizenEngagementPortal.Web.Models.Domain
{
    public class Issue
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(2000)]
        public string Description { get; set; } = string.Empty;

        public IssueStatus Status { get; set; } = IssueStatus.Pending;
        public Priority Priority { get; set; } = Priority.Medium;

        [Required]
        [StringLength(500)]
        public string Location { get; set; } = string.Empty;

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        [Required]
        public string CitizenId { get; set; } = string.Empty;

        [Required]
        public string CategoryId { get; set; } = string.Empty;

        public string? AssignedTo { get; set; }
        public DateTime? AssignedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey(nameof(CitizenId))]
        public virtual User Citizen { get; set; } = null!;

        [ForeignKey(nameof(CategoryId))]
        public virtual IssueCategory Category { get; set; } = null!;

        [ForeignKey(nameof(AssignedTo))]
        public virtual User? Assignee { get; set; }

        public virtual ICollection<IssueMedia> Media { get; set; } = new List<IssueMedia>();
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }

    public enum IssueStatus
    {
        Pending,
        InProgress,
        Resolved,
        Rejected,
        Closed
    }

    public enum Priority
    {
        Low,
        Medium,
        High,
        Urgent
    }
}