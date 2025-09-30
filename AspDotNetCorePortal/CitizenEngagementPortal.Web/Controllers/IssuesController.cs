using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CitizenEngagementPortal.Web.Models;
using CitizenEngagementPortal.Web.Models.ViewModel.Issues;
using CitizenEngagementPortal.Web.Models.Domain;

namespace CitizenEngagementPortal.Web.Controllers
{
    [Authorize]
    public class IssuesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<IssuesController> _logger;

        public IssuesController(
            ApplicationDbContext context,
            UserManager<User> userManager,
            ILogger<IssuesController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        // GET: Issues
        public async Task<IActionResult> Index(IssueStatus? status = null, string sortOrder = null, int page = 1)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var query = _context.Issues
                .Include(i => i.Category)
                .Include(i => i.Citizen)
                .Include(i => i.Assignee)
                .Where(i => i.CitizenId == user.Id);

            if (status.HasValue)
            {
                query = query.Where(i => i.Status == status.Value);
            }

            // Apply sorting
            query = sortOrder switch
            {
                "title" => query.OrderBy(i => i.Title),
                "title_desc" => query.OrderByDescending(i => i.Title),
                "date" => query.OrderBy(i => i.CreatedAt),
                "date_desc" => query.OrderByDescending(i => i.CreatedAt),
                _ => query.OrderByDescending(i => i.CreatedAt)
            };

            var pageSize = 10;
            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var issues = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var viewModel = new IssueListViewModel
            {
                Issues = issues.Select(i => new IssueListItem
                {
                    Id = i.Id,
                    Title = i.Title,
                    Description = i.Description.Length > 100 ? i.Description.Substring(0, 100) + "..." : i.Description,
                    Status = i.Status,
                    Priority = i.Priority,
                    Location = i.Location,
                    CreatedAt = i.CreatedAt,
                    Category = i.Category,
                    Citizen = i.Citizen,
                    Assignee = i.Assignee
                }).ToList(),
                CurrentFilter = status,
                CurrentSort = sortOrder,
                CurrentPage = page,
                TotalPages = totalPages
            };

            return View(viewModel);
        }

        // GET: Issues/Create
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var categories = await _context.IssueCategories.ToListAsync();
            
            var viewModel = new CreateIssueViewModel
            {
                Categories = categories
            };

            return View(viewModel);
        }

        // POST: Issues/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateIssueViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            if (ModelState.IsValid)
            {
                var issue = new Issue
                {
                    Title = model.Title,
                    Description = model.Description,
                    Location = model.Location,
                    Latitude = model.Latitude,
                    Longitude = model.Longitude,
                    CategoryId = model.CategoryId,
                    Priority = model.Priority,
                    CitizenId = user.Id,
                    Status = IssueStatus.Pending
                };

                _context.Issues.Add(issue);
                await _context.SaveChangesAsync();

                // Handle file uploads
                if (model.MediaFiles != null && model.MediaFiles.Count > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    foreach (var file in model.MediaFiles)
                    {
                        if (file.Length > 0)
                        {
                            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                            var filePath = Path.Combine(uploadsFolder, fileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }

                            var media = new IssueMedia
                            {
                                Url = $"/uploads/{fileName}",
                                Type = file.ContentType.StartsWith("image/") ? MediaType.Image : MediaType.Video,
                                IssueId = issue.Id
                            };

                            _context.IssueMedia.Add(media);
                        }
                    }

                    await _context.SaveChangesAsync();
                }

                _logger.LogInformation($"User {user.Email} created new issue: {issue.Title}");
                return RedirectToAction(nameof(Index));
            }

            // If we got this far, something failed; redisplay form
            model.Categories = await _context.IssueCategories.ToListAsync();
            return View(model);
        }

        // GET: Issues/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var issue = await _context.Issues
                .Include(i => i.Category)
                .Include(i => i.Citizen)
                .Include(i => i.Assignee)
                .Include(i => i.Media)
                .Include(i => i.Comments)
                    .ThenInclude(c => c.Author)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (issue == null)
            {
                return NotFound();
            }

            var viewModel = new IssueDetailViewModel
            {
                Id = issue.Id,
                Title = issue.Title,
                Description = issue.Description,
                Status = issue.Status,
                Priority = issue.Priority,
                Location = issue.Location,
                Latitude = issue.Latitude,
                Longitude = issue.Longitude,
                CreatedAt = issue.CreatedAt,
                UpdatedAt = issue.UpdatedAt,
                AssignedAt = issue.AssignedAt,
                ResolvedAt = issue.ResolvedAt,
                Category = issue.Category,
                Citizen = issue.Citizen,
                Assignee = issue.Assignee,
                Media = issue.Media.ToList(),
                Comments = issue.Comments.OrderBy(c => c.CreatedAt).ToList(),
                CanEdit = user.Id == issue.CitizenId || user.Role == UserRole.Admin,
                CanAssign = user.Role == UserRole.Admin || user.Role == UserRole.Official,
                CanResolve = user.Role == UserRole.Admin || user.Role == UserRole.Official || user.Id == issue.Assignee.Id
            };

            return View(viewModel);
        }

        // POST: Issues/AddComment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(string issueId, string content)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            if (string.IsNullOrWhiteSpace(content))
            {
                return BadRequest("Comment content is required.");
            }

            var issue = await _context.Issues.FindAsync(issueId);
            if (issue == null)
            {
                return NotFound();
            }

            var comment = new Comment
            {
                Content = content.Trim(),
                IssueId = issueId,
                AuthorId = user.Id
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"User {user.Email} added comment to issue {issue.Title}");

            return RedirectToAction(nameof(Details), new { id = issueId });
        }
    }
}