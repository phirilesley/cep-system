using CitizenEngagementPortal.Web.Models.Domain;

namespace CitizenEngagementPortal.Web.Models.ViewModel.Issues
{
    public class IssueListViewModel
    {
        public List<IssueListItem> Issues { get; set; } = new();
        public IssueStatus? CurrentFilter { get; set; }
        public string? CurrentSort { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
        public bool HasPrevious => CurrentPage > 1;
        public bool HasNext => CurrentPage < TotalPages;
    }

    public class IssueListItem
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public IssueStatus Status { get; set; }
        public Priority Priority { get; set; }
        public string Location { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        
        public IssueCategory Category { get; set; } = null!;
        public User Citizen { get; set; } = null!;
        public User? Assignee { get; set; }
    }
}