using System.Linq.Expressions;
using TrelloClone.Core.Entities;

namespace TrelloClone.Core.Interfaces
{
    public interface IRepository<T> where T : BaseEntity
    {
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task AddAsync(T entity);
        
        // Bu ikisi kesinlikle void olmalı
        void Update(T entity);
        void Delete(T entity);
    }
}