namespace FoldrengesekSOE2026.ViewModels.Admin
{
    public class UserActivityViewModel
    {
        public string UserId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new();
        public DateTime? LastActivity { get; set; }
        public string? LastActivityDescription { get; set; }
        public int TotalActions { get; set; }
        public int FailedLoginAttempts { get; set; }
    }
}
