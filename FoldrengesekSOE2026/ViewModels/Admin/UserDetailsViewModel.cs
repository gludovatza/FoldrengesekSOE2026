namespace FoldrengesekSOE2026.ViewModels.Admin
{
    public class UserDetailsViewModel
    {
        public UserInfoViewModel User { get; set; } = new();
        public UserStatsViewModel Stats { get; set; } = new();
        public List<LogViewModel> RecentLogs { get; set; } = new();
    }

    public class UserInfoViewModel
    {
        public string UserId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new();
    }

    public class UserStatsViewModel
    {
        public int TotalActions { get; set; }
        public int FailedLogins { get; set; }
        public DateTime? LastActivity { get; set; }
    }
}
