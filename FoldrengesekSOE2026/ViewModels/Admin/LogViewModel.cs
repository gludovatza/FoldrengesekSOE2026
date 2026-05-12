namespace FoldrengesekSOE2026.ViewModels.Admin
{
    public class LogViewModel
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string? UserId { get; set; }
        public string? UserEmail { get; set; }
        public string HttpMethod { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public int StatusCode { get; set; }
        public string? Message { get; set; }
        public string LogLevel { get; set; } = string.Empty;
        public bool IsAuthFailure { get; set; }
        public string? IpAddress { get; set; }
        public string? EntityType { get; set; }
        public string? EntityId { get; set; }
        public string? Action { get; set; }
    }
}
