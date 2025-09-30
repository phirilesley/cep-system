using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CitizenEngagementPortal.Web.Models.Domain
{
    public class IssueMedia
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [StringLength(1000)]
        public string Url { get; set; } = string.Empty;

        public MediaType Type { get; set; } = MediaType.Image;

        [Required]
        public string IssueId { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey(nameof(IssueId))]
        public virtual Issue Issue { get; set; } = null!;
    }

    public enum MediaType
    {
        Image,
        Video
    }
}