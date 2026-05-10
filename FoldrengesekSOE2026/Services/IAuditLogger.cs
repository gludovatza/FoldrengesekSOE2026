namespace FoldrengesekSOE2026.Services
{
    public interface IAuditLogger
    {
        Task LogChangeAsync<T>(
            string userId,
            string userEmail,
            string entityType,
            string entityId,
            string action,
            T? oldValue,
            T? newValue);
    }
}
