using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using CitizenEngagementPortal.Web.Services;

namespace CitizenEngagementPortal.Web.Services
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly IConfiguration _configuration;

        public EmailService(ILogger<EmailService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<bool> SendEmailAsync(string to, string subject, string htmlContent)
        {
            try
            {
                var apiKey = _configuration["SendGrid:ApiKey"];
                if (string.IsNullOrEmpty(apiKey))
                {
                    _logger.LogWarning("SendGrid API key not configured. Skipping email send.");
                    return false;
                }

                var client = new SendGridClient(apiKey);
                var from = new EmailAddress(_configuration["SendGrid:FromEmail"] ?? "noreply@cep.local", "Citizen Engagement Portal");
                var toEmail = new EmailAddress(to);
                var msg = MailHelper.CreateSingleEmail(from, toEmail, subject, null, htmlContent);

                var response = await client.SendEmailAsync(msg);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"Email sent successfully to {to}");
                    return true;
                }
                else
                {
                    _logger.LogError($"Failed to send email to {to}. Status code: {response.StatusCode}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending email to {to}");
                return false;
            }
        }

        public async Task<bool> SendIssueCreatedNotificationAsync(string to, string issueTitle, string reporterName, string issueId)
        {
            var subject = "New Issue Reported";
            var htmlContent = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                    <h2 style='color: #3B82F6;'>New Issue Reported</h2>
                    <p>A new issue has been reported by <strong>{reporterName}</strong>.</p>
                    <div style='background-color: #f3f4f6; padding: 16px; border-radius: 8px; margin: 16px 0;'>
                        <h3 style='margin: 0 0 8px 0;'>{issueTitle}</h3>
                        <p style='margin: 0; color: #6b7280;'>Issue ID: {issueId}</p>
                    </div>
                    <p>Please log in to the admin dashboard to review and assign this issue.</p>
                    <a href='{_configuration["App:Url"]}/Admin/Issues' style='background-color: #3B82F6; color: white; padding: 12px 24px; text-decoration: none; border-radius: 6px; display: inline-block; margin-top: 16px;'>
                        View in Dashboard
                    </a>
                </div>";

            return await SendEmailAsync(to, subject, htmlContent);
        }

        public async Task<bool> SendIssueAssignedNotificationAsync(string to, string issueTitle, string assigneeName, string issueId)
        {
            var subject = "Issue Assigned to You";
            var htmlContent = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                    <h2 style='color: #3B82F6;'>Issue Assigned to You</h2>
                    <p>Hello <strong>{assigneeName}</strong>,</p>
                    <p>A new issue has been assigned to you for resolution.</p>
                    <div style='background-color: #f3f4f6; padding: 16px; border-radius: 8px; margin: 16px 0;'>
                        <h3 style='margin: 0 0 8px 0;'>{issueTitle}</h3>
                        <p style='margin: 0; color: #6b7280;'>Issue ID: {issueId}</p>
                    </div>
                    <p>Please review the issue details and start working on it.</p>
                    <a href='{_configuration["App:Url"]}/Issues/Details/{issueId}' style='background-color: #3B82F6; color: white; padding: 12px 24px; text-decoration: none; border-radius: 6px; display: inline-block; margin-top: 16px;'>
                        View Issue Details
                    </a>
                </div>";

            return await SendEmailAsync(to, subject, htmlContent);
        }

        public async Task<bool> SendIssueUpdatedNotificationAsync(string to, string issueTitle, string newStatus, string issueId)
        {
            var subject = "Issue Status Update";
            var htmlContent = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                    <h2 style='color: #3B82F6;'>Issue Status Update</h2>
                    <p>The status of your reported issue has been updated.</p>
                    <div style='background-color: #f3f4f6; padding: 16px; border-radius: 8px; margin: 16px 0;'>
                        <h3 style='margin: 0 0 8px 0;'>{issueTitle}</h3>
                        <p style='margin: 0; color: #6b7280;'>New Status: <strong>{newStatus}</strong></p>
                        <p style='margin: 0; color: #6b7280;'>Issue ID: {issueId}</p>
                    </div>
                    <p>You can view the issue details and track its progress.</p>
                    <a href='{_configuration["App:Url"]}/Issues/Details/{issueId}' style='background-color: #3B82F6; color: white; padding: 12px 24px; text-decoration: none; border-radius: 6px; display: inline-block; margin-top: 16px;'>
                        View Issue Details
                    </a>
                </div>";

            return await SendEmailAsync(to, subject, htmlContent);
        }

        public async Task<bool> SendIssueResolvedNotificationAsync(string to, string issueTitle, string resolutionTime, string issueId)
        {
            var subject = "Issue Resolved";
            var htmlContent = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                    <h2 style='color: #10B981;'>Issue Resolved</h2>
                    <p>Good news! The issue you reported has been resolved.</p>
                    <div style='background-color: #f0fdf4; padding: 16px; border-radius: 8px; margin: 16px 0; border-left: 4px solid #10B981;'>
                        <h3 style='margin: 0 0 8px 0;'>{issueTitle}</h3>
                        <p style='margin: 0; color: #6b7280;'>Resolution Time: {resolutionTime}</p>
                        <p style='margin: 0; color: #6b7280;'>Issue ID: {issueId}</p>
                    </div>
                    <p>Thank you for helping improve our community. If you have any feedback about the resolution, please add a comment to the issue.</p>
                    <a href='{_configuration["App:Url"]}/Issues/Details/{issueId}' style='background-color: #10B981; color: white; padding: 12px 24px; text-decoration: none; border-radius: 6px; display: inline-block; margin-top: 16px;'>
                        View Resolved Issue
                    </a>
                </div>";

            return await SendEmailAsync(to, subject, htmlContent);
        }

        public async Task<bool> SendCommentAddedNotificationAsync(string to, string issueTitle, string commenterName, string commentPreview, string issueId)
        {
            var subject = "New Comment on Issue";
            var htmlContent = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                    <h2 style='color: #3B82F6;'>New Comment on Issue</h2>
                    <p><strong>{commenterName}</strong> has added a comment to an issue you're following.</p>
                    <div style='background-color: #f3f4f6; padding: 16px; border-radius: 8px; margin: 16px 0;'>
                        <h3 style='margin: 0 0 8px 0;'>{issueTitle}</h3>
                        <p style='margin: 0; color: #6b7280; font-style: italic;'>""{commentPreview}""</p>
                    </div>
                    <p>Click below to view the full comment and respond if needed.</p>
                    <a href='{_configuration["App:Url"]}/Issues/Details/{issueId}' style='background-color: #3B82F6; color: white; padding: 12px 24px; text-decoration: none; border-radius: 6px; display: inline-block; margin-top: 16px;'>
                        View Comment
                    </a>
                </div>";

            return await SendEmailAsync(to, subject, htmlContent);
        }
    }
}