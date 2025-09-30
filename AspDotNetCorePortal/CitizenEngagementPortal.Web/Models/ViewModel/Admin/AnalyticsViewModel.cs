namespace CitizenEngagementPortal.Web.Models.ViewModel.Admin
{
    public class AnalyticsViewModel
    {
        public List<IssuesByCategory> IssuesByCategory { get; set; } = new();
        public List<IssuesByStatus> IssuesByStatus { get; set; } = new();
        public List<IssuesByMonth> IssuesByMonth { get; set; } = new();
        public List<TopCategory> TopCategories { get; set; } = new();
        
        public ResolutionTimeStats ResolutionTime { get; set; } = new();
    }

    public class IssuesByCategory
    {
        public string Name { get; set; } = string.Empty;
        public int Count { get; set; }
        public string Color { get; set; } = string.Empty;
    }

    public class IssuesByStatus
    {
        public string Name { get; set; } = string.Empty;
        public int Count { get; set; }
        public string Color { get; set; } = string.Empty;
    }

    public class IssuesByMonth
    {
        public string Month { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class TopCategory
    {
        public string Name { get; set; } = string.Empty;
        public int Count { get; set; }
        public double Percentage { get; set; }
    }

    public class ResolutionTimeStats
    {
        public double Average { get; set; }
        public double Fastest { get; set; }
        public double Slowest { get; set; }
    }
}