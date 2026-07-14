using TrelloClone.Business.Interfaces;
using TrelloClone.Core.Entities;
using TrelloClone.Core.Interfaces;
using TrelloClone.DataAccess.Context;

namespace TrelloClone.Business.Services
{
    public class AuditLogService : IAuditLogService
    {
        private readonly IRepository<AuditLog> _logRepository;
        private readonly TrelloCloneDbContext _context;

        public AuditLogService(IRepository<AuditLog> logRepository, TrelloCloneDbContext context)
        {
            _logRepository = logRepository;
            _context = context;
        }

        public async Task LogActionAsync(string action, string details, string username, int? cardId = null)
        {
            var log = new AuditLog
            {
                Action = action,
                Details = details,
                Username = username,
                CardId = cardId
            };

            await _logRepository.AddAsync(log);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetRecentLogsAsync(int count = 10)
        {
            var logs = await _logRepository.GetAllAsync();
            return logs.OrderByDescending(l => l.CreatedDate).Take(count);
        }

    }
}