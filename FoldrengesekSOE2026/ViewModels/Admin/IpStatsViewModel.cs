namespace FoldrengesekSOE2026.ViewModels.Admin
{
    public class IpStatsViewModel
    {
        public string IpAddress { get; set; } = string.Empty;
        public int TotalRequests { get; set; }
        public int FailedLogins { get; set; }
        public int UniqueUsers { get; set; }
        public DateTime LastActivity { get; set; }
    }
}
