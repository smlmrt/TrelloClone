using TrelloClone.Business.Interfaces;
using TrelloClone.Core.Entities;
using TrelloClone.Core.Interfaces;
using TrelloClone.DataAccess.Context;

namespace TrelloClone.Business.Services
{
    public class ColumnService : IColumnService
    {
        private readonly IRepository<Column> _columnRepository;
        private readonly TrelloCloneDbContext _context;

        public ColumnService(IRepository<Column> columnRepository, TrelloCloneDbContext context)
        {
            _columnRepository = columnRepository;
            _context = context;
        }

        public async Task<IEnumerable<Column>> GetColumnsByProjectIdAsync(int projectId)
        {
            // Belirli bir projeye ait sütunları sırasına (Order) göre getiriyoruz
            return (await _columnRepository.FindAsync(c => c.ProjectId == projectId)).OrderBy(c => c.Order);
        }

        public async Task<Column?> GetColumnByIdAsync(int id)
        {
            return await _columnRepository.GetByIdAsync(id);
        }

        public async Task CreateColumnAsync(Column column)
        {
            await _columnRepository.AddAsync(column);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateColumnAsync(Column column)
        {
            _columnRepository.Update(column);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteColumnAsync(int id)
        {
            var column = await _columnRepository.GetByIdAsync(id);
            if (column != null)
            {
                _columnRepository.Delete(column);
                await _context.SaveChangesAsync();
            }
        }
    }
}