using FoldrengesekSOE2026.Data;
using FoldrengesekSOE2026.Models;
using System.Security.Claims;

namespace FoldrengesekSOE2026.Middleware
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LoggingMiddleware> _logger;

        private static readonly string[] IgnoredExtensions =
        [
            ".css", ".js", ".png", ".jpg", ".jpeg", ".gif", ".svg",
            ".ico", ".woff", ".woff2", ".ttf", ".map"
        ];

        public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, FoldrengesContext dbContext)
        {
            var path = context.Request.Path.Value ?? "";

            var method = context.Request.Method;
            var timestamp = DateTime.UtcNow;

            string? userId = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            string? userEmail = context.User?.FindFirst(ClaimTypes.Email)?.Value;

            var ipAddress = context.Connection.RemoteIpAddress?.ToString();
            var userAgent = context.Request.Headers["User-Agent"].ToString();

            var (entityType, entityId, action) = ParsePathInfo(method, path);

            if (IgnoredExtensions.Any(ext => path.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
            {
                await _next(context);
                return;
            }

            if (path.StartsWith("/lib", StringComparison.OrdinalIgnoreCase) ||
                path.StartsWith("/css", StringComparison.OrdinalIgnoreCase) ||
                path.StartsWith("/js", StringComparison.OrdinalIgnoreCase) ||
                path.StartsWith("/images", StringComparison.OrdinalIgnoreCase))
            {
                await _next(context);
                return;
            }

            await _next(context);

            var statusCode = context.Response.StatusCode;

            // Redirect válaszok kihagyása
            if (statusCode >= 300 && statusCode < 400)
            {
                return;
            }

            var (logLevel, message, isAuthFailure) = DetermineLogDetails(method, path, statusCode, userEmail);

            _logger.Log(GetLogLevel(logLevel), "{Method} {Path} - {StatusCode} - User: {UserEmail} - IP: {IpAddress}",
                method, path, statusCode, userEmail ?? "Anonymous", ipAddress);

            var logEntry = new Log
            {
                Timestamp = timestamp,
                UserId = userId,
                UserEmail = userEmail,
                HttpMethod = method,
                Path = path,
                StatusCode = statusCode,
                Message = message,
                LogLevel = logLevel,
                IsAuthFailure = isAuthFailure,
                IpAddress = ipAddress,
                UserAgent = userAgent,
                EntityType = entityType,
                EntityId = entityId,
                Action = action
            };

            dbContext.Logs.Add(logEntry);
            await dbContext.SaveChangesAsync();
        }

        private (string logLevel, string message, bool isAuthFailure) DetermineLogDetails(
            string method, string path, int statusCode, string? userEmail)
        {
            var user = userEmail ?? "Anonymous";

            return statusCode switch
            {
                401 => ("Warning", $"Unauthorized access attempt by {user} to {method} {path}", true),
                403 => ("Warning", $"Forbidden access attempt by {user} to {method} {path}", true),
                >= 400 and < 500 => ("Warning", $"{user} - {method} {path} failed with {statusCode}", false),
                >= 500 => ("Error", $"Server error: {method} {path} - {statusCode}", false),
                _ => ("Information", $"{user} - {method} {path} - {statusCode}", false)
            };
        }

        private (string? entityType, string? entityId, string? action) ParsePathInfo(string method, string path)
        {
            var parts = path.Split('/', StringSplitOptions.RemoveEmptyEntries);

            string? entityType = null;
            string? entityId = null;
            string? action = null;

            if (parts.Length >= 1)
            {
                entityType = parts[0] switch
                {
                    "telepulesek" => "Telepules",
                    "naplok" => "Naplo",
                    "auth" => "Auth",
                    "feladatok" => "Feladat",
                    _ => parts[0]
                };

                if (parts.Length >= 2)
                {
                    if (int.TryParse(parts[1], out _) || parts[1].Length <= 20)
                    {
                        entityId = parts[1];
                    }
                    else
                    {
                        action = parts[1];
                    }
                }

                action ??= method switch
                {
                    "GET" => entityId != null ? "View" : "List",
                    "POST" => entityType == "Auth" ? "Login/Register" : "Create",
                    "PUT" => "Update",
                    "DELETE" => "Delete",
                    _ => method
                };
            }

            return (entityType, entityId, action);
        }

        private LogLevel GetLogLevel(string level) => level switch
        {
            "Error" => LogLevel.Error,
            "Warning" => LogLevel.Warning,
            _ => LogLevel.Information
        };
    }
}
