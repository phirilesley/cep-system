namespace CitizenEngagementPortal.Web.Services
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string to, string subject, string htmlContent);
        Task<bool> SendIssueCreatedNotificationAsync(string to, string issueTitle, string reporterName, string issueId);
        Task<bool> SendIssueAssignedNotificationAsync(string to, string issueTitle, string assigneeName, string issueId);
        Task<bool> SendIssueUpdatedNotificationAsync(string to, string issueTitle, string newStatus, string issueId);
        Task<bool> SendIssueResolvedNotificationAsync(string to, string issueTitle, string resolutionTime, string issueId);
        Task<bool> SendCommentAddedNotificationAsync(string to, string issueTitle, string commenterName, string commentPreview, string issueId);
    }
}