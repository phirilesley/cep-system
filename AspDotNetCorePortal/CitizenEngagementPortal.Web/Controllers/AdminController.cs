using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CitizenEngagementPortal.Web.Models;
using CitizenEngagementPortal.Web.Models.ViewModel.Admin;
using CitizenEngagementPortal.Web.Models.Domain;

namespace CitizenEngagementPortal.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<AdminController> _logger;

        public AdminController(
            ApplicationDbContext context,
            UserManager<User> userManager,
            ILogger<AdminController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        // GET: Admin
        public async Task<IActionResult> Index()
        {
            var totalIssues = await _context.Issues.CountAsync();
            var pendingIssues = await _context.Issues.CountAsync(i => i.Status == IssueStatus.Pending);
            var inProgressIssues = await _context.Issues.CountAsync(i => i.Status == IssueStatus.InProgress);
            var resolvedIssues = await _context.Issues.CountAsync(i => i.Status == IssueStatus.Resolved);
            var totalUsers = await _userManager.Users.CountAsync();
            var totalCitizens = await _userManager.Users.CountAsync(u => u.Role == UserRole.Citizen);

            var categoryStats = await _context.Issues
                .GroupBy(i => i.CategoryId)
                .Select(g => new
                {
                    CategoryId = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            var categories = await _context.IssueCategories.ToListAsync();
            var categoryStatsList = categories.Select(cat => new CategoryStats
            {
                CategoryName = cat.Name,
                CategoryColor = cat.Color ?? "#3B82F6",
                Count = categoryStats.FirstOrDefault(cs => cs.CategoryId == cat.Id)?.Count ?? 0,
                Percentage = totalIssues > 0 ? (double)(categoryStats.FirstOrDefault(cs => cs.CategoryId == cat.Id)?.Count ?? 0) / totalIssues * 100 : 0
            }).ToList();

            var recentIssues = await _context.Issues
                .Include(i => i.Category)
                .Include(i => i.Citizen)
                .OrderByDescending(i => i.CreatedAt)
                .Take(10)
                .Select(i => new IssueListItem
                {
                    Id = i.Id,
                    Title = i.Title,
                    Status = i.Status,
                    Priority = i.Priority,
                    Location = i.Location,
                    CreatedAt = i.CreatedAt,
                    CategoryName = i.Category.Name,
                    ReporterName = i.Citizen.Name ?? i.Citizen.Email
                })
                .ToListAsync();

            var recentUsers = await _userManager.Users
                .OrderByDescending(u => u.CreatedAt)
                .Take(10)
                .Select(u => new UserListItem
                {
                    Id = u.Id,
                    Name = u.Name ?? u.Email,
                    Email = u.Email,
                    Role = u.Role,
                    CreatedAt = u.CreatedAt
                })
                .ToListAsync();

            var viewModel = new AdminDashboardViewModel
            {
                TotalIssues = totalIssues,
                PendingIssues = pendingIssues,
                InProgressIssues = inProgressIssues,
                ResolvedIssues = resolvedIssues,
                TotalUsers = totalUsers,
                TotalCitizens = totalCitizens,
                CategoryStats = categoryStatsList,
                RecentIssues = recentIssues,
                RecentUsers = recentUsers
            };

            return View(viewModel);
        }

        // GET: Admin/Issues
        public async Task<IActionResult> Issues(IssueStatus? status = null, int page = 1)
        {
            var query = _context.Issues
                .Include(i => i.Category)
                .Include(i => i.Citizen)
                .Include(i => i.Assignee)
                .AsQueryable();

            if (status.HasValue)
            {
                query = query.Where(i => i.Status == status.Value);
            }

            var pageSize = 20;
            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var issues = await query
                .OrderByDescending(i => i.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentFilter = status;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(issues);
        }

        // GET: Admin/Analytics
        public async Task<IActionResult> Analytics()
        {
            var issuesByCategory = await _context.Issues
                .GroupBy(i => i.Category.Name)
                .Select(g => new IssuesByCategory
                {
                    Name = g.Key,
                    Count = g.Count(),
                    Color = g.First().Category.Color ?? "#3B82F6"
                })
                .ToListAsync();

            var issuesByStatus = await _context.Issues
                .GroupBy(i => i.Status)
                .Select(g => new IssuesByStatus
                {
                    Name = g.Key.ToString(),
                    Count = g.Count(),
                    Color = g.Key == IssueStatus.Pending ? "#F59E0B" :
        g.Key == IssueStatus.InProgress ? "#3B82F6" :
        g.Key == IssueStatus.Resolved ? "#10B981" :
        g.Key == IssueStatus.Rejected ? "#EF4444" :
        g.Key == IssueStatus.Closed ? "#6B7280" :
        "#6B7280"

                })
                .ToListAsync();

            var sixMonthsAgo = DateTime.UtcNow.AddMonths(-6);
            var issuesByMonth = await _context.Issues
                .Where(i => i.CreatedAt >= sixMonthsAgo)
                .GroupBy(i => new { i.CreatedAt.Year, i.CreatedAt.Month })
                .Select(g => new IssuesByMonth
                {
                    Month = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMM yyyy"),
                    Count = g.Count()
                })
                .OrderBy(g => g.Month)
                .ToListAsync();

            var topCategories = issuesByCategory
                .OrderByDescending(c => c.Count)
                .Take(5)
                .Select(c => new TopCategory
                {
                    Name = c.Name,
                    Count = c.Count,
                    Percentage = issuesByCategory.Sum(ic => ic.Count) > 0 ? (double)c.Count / issuesByCategory.Sum(ic => ic.Count) * 100 : 0
                })
                .ToList();

            var resolvedIssues = await _context.Issues
                .Where(i => i.Status == IssueStatus.Resolved && i.ResolvedAt.HasValue)
                .Select(i => new
                {
                    ResolutionTime = (i.ResolvedAt.Value - i.CreatedAt).TotalDays
                })
                .ToListAsync();

            var resolutionTimeStats = new ResolutionTimeStats
            {
                Average = resolvedIssues.Any() ? resolvedIssues.Average(r => r.ResolutionTime) : 0,
                Fastest = resolvedIssues.Any() ? resolvedIssues.Min(r => r.ResolutionTime) : 0,
                Slowest = resolvedIssues.Any() ? resolvedIssues.Max(r => r.ResolutionTime) : 0
            };

            var viewModel = new AnalyticsViewModel
            {
                IssuesByCategory = issuesByCategory,
                IssuesByStatus = issuesByStatus,
                IssuesByMonth = issuesByMonth,
                TopCategories = topCategories,
                ResolutionTime = resolutionTimeStats
            };

            return View(viewModel);
        }

        // GET: Admin/AssignIssue/5
        public async Task<IActionResult> AssignIssue(string id)
        {
            var issue = await _context.Issues
                .Include(i => i.Category)
                .Include(i => i.Citizen)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (issue == null)
            {
                return NotFound();
            }

            var officials = await _userManager.GetUsersInRoleAsync("Official");
            var admins = await _userManager.GetUsersInRoleAsync("Admin");
            var assignableUsers = officials.Concat(admins).ToList();

            ViewBag.AssignableUsers = assignableUsers;
            ViewBag.Issue = issue;

            return View();
        }

        // POST: Admin/AssignIssue/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignIssue(string id, string assignedTo)
        {
            var issue = await _context.Issues.FindAsync(id);
            if (issue == null)
            {
                return NotFound();
            }

            issue.AssignedTo = assignedTo;
            issue.AssignedAt = DateTime.UtcNow;
            issue.Status = IssueStatus.InProgress;

            _context.Update(issue);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Issue {issue.Title} assigned to user {assignedTo}");

            return RedirectToAction(nameof(Issues));
        }

        // POST: Admin/UpdateIssueStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateIssueStatus(string id, IssueStatus status)
        {
            var issue = await _context.Issues.FindAsync(id);
            if (issue == null)
            {
                return NotFound();
            }

            issue.Status = status;
            issue.UpdatedAt = DateTime.UtcNow;

            if (status == IssueStatus.Resolved)
            {
                issue.ResolvedAt = DateTime.UtcNow;
            }

            _context.Update(issue);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Issue {issue.Title} status updated to {status}");

            return RedirectToAction(nameof(Issues));
        }
    }
}