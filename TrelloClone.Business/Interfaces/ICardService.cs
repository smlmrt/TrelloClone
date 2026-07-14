using TrelloClone.Core.Entities;

namespace TrelloClone.Business.Interfaces
{
    public interface ICardService
    {
        Task<IEnumerable<Card>> GetCardsByColumnIdAsync(int columnId);
        Task<Card?> GetCardByIdAsync(int id);
        Task CreateCardAsync(Card card);
        Task UpdateCardAsync(Card card);
        Task DeleteCardAsync(int id);
    }
}