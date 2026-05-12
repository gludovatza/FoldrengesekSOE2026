using FoldrengesekSOE2026.Data;
using FoldrengesekSOE2026.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoldrengesekSOE2026.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly FoldrengesContext _context;

        public AdminController(
            UserManager<IdentityUser> userManager,
            FoldrengesContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var today = DateTime.UtcNow.Date;

            var model = new AdminStatsViewModel
            {
                TotalUsers = await _userManager.Users.CountAsync(),

                ActiveUsersToday = await _context.Logs
                    .Where(l => l.Timestamp >= today && l.UserId != null)
                    .Select(l => l.UserId)
                    .Distinct()
                    .CountAsync(),

                TotalLogs = await _context.Logs.CountAsync(),

                FailedLoginAttemptsToday = await _context.Logs
                    .CountAsync(l => l.IsAuthFailure && l.Timestamp >= today),

                TopActions = await _context.Logs
                    .Where(l => l.EntityType != null && l.Action != null)
                    .GroupBy(l => new { l.EntityType, l.Action })
                    .Select(g => new EntityActionCountViewModel
                    {
                        EntityType = g.Key.EntityType ?? "",
                        Action = g.Key.Action ?? "",
                        Count = g.Count()
                    })
                    .OrderByDescending(x => x.Count)
                    .Take(10)
                    .ToListAsync()
            };

            return View(model);
        }

        public async Task<IActionResult> Users()
        {
            var users = await _userManager.Users
                .OrderBy(u => u.Email)
                .ToListAsync();

            var model = new List<UserActivityViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                var lastLog = await _context.Logs
                    .Where(l => l.UserId == user.Id)
                    .OrderByDescending(l => l.Timestamp)
                    .FirstOrDefaultAsync();

                var totalActions = await _context.Logs
                    .CountAsync(l => l.UserId == user.Id);

                var failedLogins = await _context.Logs
                    .CountAsync(l => l.UserId == user.Id && l.IsAuthFailure);

                model.Add(new UserActivityViewModel
                {
                    UserId = user.Id,
                    Email = user.Email ?? "",
                    UserName = user.UserName ?? "",
                    Roles = roles.ToList(),
                    LastActivity = lastLog?.Timestamp,
                    LastActivityDescription = lastLog != null
                        ? $"{lastLog.HttpMethod} {lastLog.Path}"
                        : "Még nincs aktivitás",
                    TotalActions = totalActions,
                    FailedLoginAttempts = failedLogins
                });
            }

            model = model
                .OrderByDescending(u => u.LastActivity)
                .ToList();

            return View(model);
        }

        public async Task<IActionResult> UserDetails(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction(nameof(Users));
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);

            var logs = await _context.Logs
                .Where(l => l.UserId == id)
                .OrderByDescending(l => l.Timestamp)
                .Take(100)
                .Select(l => new LogViewModel
                {
                    Id = l.Id,
                    Timestamp = l.Timestamp,
                    UserId = l.UserId,
                    UserEmail = l.UserEmail,
                    HttpMethod = l.HttpMethod,
                    Path = l.Path,
                    StatusCode = l.StatusCode,
                    Message = l.Message,
                    LogLevel = l.LogLevel,
                    IsAuthFailure = l.IsAuthFailure,
                    IpAddress = l.IpAddress,
                    EntityType = l.EntityType,
                    EntityId = l.EntityId,
                    Action = l.Action
                })
                .ToListAsync();

            var totalActions = await _context.Logs
                .CountAsync(l => l.UserId == id);

            var failedLogins = await _context.Logs
                .CountAsync(l => l.UserId == id && l.IsAuthFailure);

            var model = new UserDetailsViewModel
            {
                User = new UserInfoViewModel
                {
                    UserId = user.Id,
                    Email = user.Email ?? "",
                    UserName = user.UserName ?? "",
                    Roles = roles.ToList()
                },
                Stats = new UserStatsViewModel
                {
                    TotalActions = totalActions,
                    FailedLogins = failedLogins,
                    LastActivity = logs.FirstOrDefault()?.Timestamp
                },
                RecentLogs = logs
            };

            return View(model);
        }

        public async Task<IActionResult> Logs(
            string? userEmail = null,
            string? entityType = null,
            bool? isAuthFailure = null,
            int page = 1,
            int pageSize = 50)
        {
            if (page < 1)
            {
                page = 1;
            }

            if (pageSize < 1)
            {
                pageSize = 50;
            }

            var query = _context.Logs.AsQueryable();

            if (!string.IsNullOrWhiteSpace(userEmail))
            {
                query = query.Where(l =>
                    l.UserEmail != null &&
                    l.UserEmail.Contains(userEmail));
            }

            if (!string.IsNullOrWhiteSpace(entityType))
            {
                query = query.Where(l => l.EntityType == entityType);
            }

            if (isAuthFailure.HasValue)
            {
                query = query.Where(l => l.IsAuthFailure == isAuthFailure.Value);
            }

            var totalCount = await query.CountAsync();

            var logs = await query
                .OrderByDescending(l => l.Timestamp)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(l => new LogViewModel
                {
                    Id = l.Id,
                    Timestamp = l.Timestamp,
                    UserId = l.UserId,
                    UserEmail = l.UserEmail,
                    HttpMethod = l.HttpMethod,
                    Path = l.Path,
                    StatusCode = l.StatusCode,
                    Message = l.Message,
                    LogLevel = l.LogLevel,
                    IsAuthFailure = l.IsAuthFailure,
                    IpAddress = l.IpAddress,
                    EntityType = l.EntityType,
                    EntityId = l.EntityId,
                    Action = l.Action
                })
                .ToListAsync();

            var model = new LogsIndexViewModel
            {
                Logs = logs,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                UserEmail = userEmail,
                EntityType = entityType,
                IsAuthFailure = isAuthFailure
            };

            return View(model);
        }

        public async Task<IActionResult> FailedLogins(int days = 7)
        {
            if (days < 1)
            {
                days = 7;
            }

            var since = DateTime.UtcNow.AddDays(-days);

            var logs = await _context.Logs
                .Where(l => l.IsAuthFailure && l.Timestamp >= since)
                .OrderByDescending(l => l.Timestamp)
                .Select(l => new LogViewModel
                {
                    Id = l.Id,
                    Timestamp = l.Timestamp,
                    UserId = l.UserId,
                    UserEmail = l.UserEmail,
                    HttpMethod = l.HttpMethod,
                    Path = l.Path,
                    StatusCode = l.StatusCode,
                    Message = l.Message,
                    LogLevel = l.LogLevel,
                    IsAuthFailure = l.IsAuthFailure,
                    IpAddress = l.IpAddress,
                    EntityType = l.EntityType,
                    EntityId = l.EntityId,
                    Action = l.Action
                })
                .ToListAsync();

            ViewData["Days"] = days;

            return View(logs);
        }

        public async Task<IActionResult> IpStats(int days = 7)
        {
            if (days < 1)
            {
                days = 7;
            }

            var since = DateTime.UtcNow.AddDays(-days);

            var model = await _context.Logs
                .Where(l => l.IpAddress != null && l.Timestamp >= since)
                .GroupBy(l => l.IpAddress)
                .Select(g => new IpStatsViewModel
                {
                    IpAddress = g.Key ?? "",
                    TotalRequests = g.Count(),
                    FailedLogins = g.Count(l => l.IsAuthFailure),
                    UniqueUsers = g
                        .Where(l => l.UserEmail != null)
                        .Select(l => l.UserEmail)
                        .Distinct()
                        .Count(),
                    LastActivity = g.Max(l => l.Timestamp)
                })
                .OrderByDescending(x => x.TotalRequests)
                .ToListAsync();

            ViewData["Days"] = days;

            return View(model);
        }
    }
}
