using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CitizenEngagementPortal.Web.Models.Domain
{
    public class Comment
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [StringLength(2000)]
        public string Content { get; set; } = string.Empty;

        [Required]
        public string IssueId { get; set; } = string.Empty;

        [Required]
        public string AuthorId { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey(nameof(IssueId))]
        public virtual Issue Issue { get; set; } = null!;

        [ForeignKey(nameof(AuthorId))]
        public virtual User Author { get; set; } = null!;
    }
}