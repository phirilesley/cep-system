using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CitizenEngagementPortal.Web.Models;
using CitizenEngagementPortal.Web.Models.Domain;

namespace CitizenEngagementPortal.Web.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(
            ApplicationDbContext context,
            UserManager<User> userManager,
            ILogger<DashboardController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        // GET: Dashboard
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var userIssues = await _context.Issues
                .Include(i => i.Category)
                .Include(i => i.Assignee)
                .Where(i => i.CitizenId == user.Id)
                .ToListAsync();

            var viewModel = new DashboardViewModel
            {
                TotalIssues = userIssues.Count,
                PendingIssues = userIssues.Count(i => i.Status == IssueStatus.Pending),
                InProgressIssues = userIssues.Count(i => i.Status == IssueStatus.InProgress),
                ResolvedIssues = userIssues.Count(i => i.Status == IssueStatus.Resolved),
                RecentIssues = userIssues
                    .OrderByDescending(i => i.CreatedAt)
                    .Take(5)
                    .ToList(),
                UserName = user.Name ?? user.Email,
                UserRole = user.Role.ToString()
            };

            return View(viewModel);
        }
    }

    public class DashboardViewModel
    {
        public int TotalIssues { get; set; }
        public int PendingIssues { get; set; }
        public int InProgressIssues { get; set; }
        public int ResolvedIssues { get; set; }
        public List<Issue> RecentIssues { get; set; } = new();
        public string UserName { get; set; } = string.Empty;
        public string UserRole { get; set; } = string.Empty;
    }
}