using CitizenEngagementPortal.Web.Models.Domain;

namespace CitizenEngagementPortal.Web.Models.ViewModel.Admin
{
    public class AdminDashboardViewModel
    {
        public int TotalIssues { get; set; }
        public int PendingIssues { get; set; }
        public int InProgressIssues { get; set; }
        public int ResolvedIssues { get; set; }
        public int TotalUsers { get; set; }
        public int TotalCitizens { get; set; }

        public List<CategoryStats> CategoryStats { get; set; } = new();
        public List<IssueListItem> RecentIssues { get; set; } = new();
        public List<UserListItem> RecentUsers { get; set; } = new();
    }

    public class CategoryStats
    {
        public string CategoryName { get; set; } = string.Empty;
        public string CategoryColor { get; set; } = string.Empty;
        public int Count { get; set; }
        public double Percentage { get; set; }
    }

    public class IssueListItem
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public IssueStatus Status { get; set; }
        public Priority Priority { get; set; }
        public string Location { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string ReporterName { get; set; } = string.Empty;
    }

    public class UserListItem
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}