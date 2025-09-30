using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CitizenEngagementPortal.Web.Models.Domain
{
    public class Notification
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string Message { get; set; } = string.Empty;

        public NotificationType Type { get; set; } = NotificationType.System;

        [Required]
        public string UserId { get; set; } = string.Empty;

        public string? IssueId { get; set; }

        public bool IsRead { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; } = null!;

        [ForeignKey(nameof(IssueId))]
        public virtual Issue? Issue { get; set; }
    }

    public enum NotificationType
    {
        IssueCreated,
        IssueAssigned,
        IssueUpdated,
        IssueResolved,
        CommentAdded,
        System
    }
}