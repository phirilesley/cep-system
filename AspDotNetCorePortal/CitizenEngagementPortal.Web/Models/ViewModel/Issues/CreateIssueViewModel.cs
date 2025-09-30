using System.ComponentModel.DataAnnotations;
using CitizenEngagementPortal.Web.Models.Domain;

namespace CitizenEngagementPortal.Web.Models.ViewModel.Issues
{
    public class CreateIssueViewModel
    {
        [Required]
        [StringLength(200)]
        [Display(Name = "Issue Title")]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(2000)]
        [Display(Name = "Description")]
        public string Description { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        [Display(Name = "Location")]
        public string Location { get; set; } = string.Empty;

        [Display(Name = "Latitude")]
        public double? Latitude { get; set; }

        [Display(Name = "Longitude")]
        public double? Longitude { get; set; }

        [Required]
        [Display(Name = "Category")]
        public string CategoryId { get; set; } = string.Empty;

        [Display(Name = "Priority")]
        public Priority Priority { get; set; } = Priority.Medium;

        public List<IFormFile>? MediaFiles { get; set; }

        public List<IssueCategory>? Categories { get; set; }
    }
}