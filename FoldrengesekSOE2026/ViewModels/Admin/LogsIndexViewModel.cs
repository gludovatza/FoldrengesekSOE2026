namespace FoldrengesekSOE2026.ViewModels.Admin
{
    public class LogsIndexViewModel
    {
        public List<LogViewModel> Logs { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public string? UserEmail { get; set; }
        public string? EntityType { get; set; }
        public bool? IsAuthFailure { get; set; }
    }
}
