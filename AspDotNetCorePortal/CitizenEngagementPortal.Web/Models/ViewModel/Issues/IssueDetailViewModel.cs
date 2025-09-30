using CitizenEngagementPortal.Web.Models.Domain;

namespace CitizenEngagementPortal.Web.Models.ViewModel.Issues
{
    public class IssueDetailViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public IssueStatus Status { get; set; }
        public Priority Priority { get; set; }
        public string Location { get; set; } = string.Empty;
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? AssignedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }

        public IssueCategory Category { get; set; } = null!;
        public User Citizen { get; set; } = null!;
        public User? Assignee { get; set; }

        public List<IssueMedia> Media { get; set; } = new();
        public List<Comment> Comments { get; set; } = new();

        public bool CanEdit { get; set; }
        public bool CanAssign { get; set; }
        public bool CanResolve { get; set; }
    }
}