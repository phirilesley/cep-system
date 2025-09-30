using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CitizenEngagementPortal.Web.Models;
using CitizenEngagementPortal.Web.Models.Domain;

namespace CitizenEngagementPortal.Web.Data
{
    public class ApplicationDbInitializer
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<ApplicationDbInitializer> _logger;

        public ApplicationDbInitializer(
            ApplicationDbContext context,
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<ApplicationDbInitializer> logger)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task InitializeAsync()
        {
            try
            {
                // Apply any pending migrations
                await _context.Database.MigrateAsync();

                // Create roles if they don't exist
                await CreateRolesAsync();

                // Create default users
                await CreateDefaultUsersAsync();

                _logger.LogInformation("Database initialization completed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while initializing the database.");
                throw;
            }
        }

        private async Task CreateRolesAsync()
        {
            var roles = new[] { "Admin", "Official", "Citizen" };

            foreach (var roleName in roles)
            {
                var roleExists = await _roleManager.RoleExistsAsync(roleName);
                if (!roleExists)
                {
                    var role = new IdentityRole(roleName);
                    var result = await _roleManager.CreateAsync(role);
                    
                    if (result.Succeeded)
                    {
                        _logger.LogInformation($"Role '{roleName}' created successfully.");
                    }
                    else
                    {
                        _logger.LogError($"Failed to create role '{roleName}'. Errors: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                }
            }
        }

        private async Task CreateDefaultUsersAsync()
        {
            // Create default admin user
            var adminUser = await _userManager.FindByEmailAsync("admin@cep.local");
            if (adminUser == null)
            {
                adminUser = new User
                {
                    UserName = "admin@cep.local",
                    Email = "admin@cep.local",
                    Name = "System Administrator",
                    EmailConfirmed = true,
                    Role = UserRole.Admin
                };

                var result = await _userManager.CreateAsync(adminUser, "Admin123!");
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(adminUser, "Admin");
                    _logger.LogInformation("Default admin user created successfully.");
                }
                else
                {
                    _logger.LogError($"Failed to create admin user. Errors: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }

            // Create default citizen user
            var citizenUser = await _userManager.FindByEmailAsync("citizen@example.com");
            if (citizenUser == null)
            {
                citizenUser = new User
                {
                    UserName = "citizen@example.com",
                    Email = "citizen@example.com",
                    Name = "John Citizen",
                    EmailConfirmed = true,
                    Role = UserRole.Citizen,
                    Phone = "+1234567891",
                    Address = "123 Main Street, City"
                };

                var result = await _userManager.CreateAsync(citizenUser, "Citizen123!");
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(citizenUser, "Citizen");
                    _logger.LogInformation("Default citizen user created successfully.");

                    // Create sample issues for the citizen user
                    await CreateSampleIssuesAsync(citizenUser);
                }
                else
                {
                    _logger.LogError($"Failed to create citizen user. Errors: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
        }

        private async Task CreateSampleIssuesAsync(User citizen)
        {
            var adminUser = await _userManager.FindByEmailAsync("admin@cep.local");
            var infrastructureCategory = await _context.IssueCategories.FirstOrDefaultAsync(c => c.Name == "Infrastructure");
            var sanitationCategory = await _context.IssueCategories.FirstOrDefaultAsync(c => c.Name == "Sanitation");

            if (infrastructureCategory != null)
            {
                var issue1 = new Issue
                {
                    Title = "Pothole on Main Street",
                    Description = "Large pothole causing traffic congestion and potential damage to vehicles. Located near the intersection with 1st Avenue.",
                    Location = "Main Street, between 1st and 2nd Avenue",
                    Latitude = 40.7128,
                    Longitude = -74.0060,
                    CategoryId = infrastructureCategory.Id,
                    Priority = Priority.High,
                    Status = IssueStatus.Pending,
                    CitizenId = citizen.Id
                };

                _context.Issues.Add(issue1);
            }

            if (sanitationCategory != null)
            {
                var issue2 = new Issue
                {
                    Title = "Overflowing garbage bins",
                    Description = "Public garbage bins are overflowing and not being emptied regularly. This is causing health concerns and attracting pests.",
                    Location = "Central Park, near the entrance",
                    Latitude = 40.7829,
                    Longitude = -73.9654,
                    CategoryId = sanitationCategory.Id,
                    Priority = Priority.Medium,
                    Status = IssueStatus.InProgress,
                    CitizenId = citizen.Id,
                    AssignedTo = adminUser?.Id,
                    AssignedAt = DateTime.UtcNow
                };

                _context.Issues.Add(issue2);
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation("Sample issues created successfully.");
        }
    }
}