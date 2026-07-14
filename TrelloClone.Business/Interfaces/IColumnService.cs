using TrelloClone.Core.Entities;

namespace TrelloClone.Business.Interfaces
{
    public interface IColumnService
    {
        Task<IEnumerable<Column>> GetColumnsByProjectIdAsync(int projectId);
        Task<Column?> GetColumnByIdAsync(int id);
        Task CreateColumnAsync(Column column);
        Task UpdateColumnAsync(Column column);
        Task DeleteColumnAsync(int id);
    }
}