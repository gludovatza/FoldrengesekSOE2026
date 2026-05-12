namespace FoldrengesekSOE2026.ViewModels.Admin
{
    public class AdminStatsViewModel
    {
        public int TotalUsers { get; set; }
        public int ActiveUsersToday { get; set; }
        public int TotalLogs { get; set; }
        public int FailedLoginAttemptsToday { get; set; }
        public List<EntityActionCountViewModel> TopActions { get; set; } = new();
    }

    public class EntityActionCountViewModel
    {
        public string EntityType { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}
