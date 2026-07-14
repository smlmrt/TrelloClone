using TrelloClone.Core.Entities;

namespace TrelloClone.Business.Interfaces
{
    public interface IAuditLogService
    {
        Task LogActionAsync(string action, string details, string username, int? cardId = null);
        Task<IEnumerable<AuditLog>> GetRecentLogsAsync(int count = 10);
    }
}