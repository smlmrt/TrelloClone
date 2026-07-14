using TrelloClone.Business.Interfaces;
using TrelloClone.Core.Entities;
using TrelloClone.Core.Interfaces;
using TrelloClone.DataAccess.Context;

namespace TrelloClone.Business.Services
{
    public class CardService : ICardService
    {
        private readonly IRepository<Card> _cardRepository;
        private readonly TrelloCloneDbContext _context;

        public CardService(IRepository<Card> cardRepository, TrelloCloneDbContext context)
        {
            _cardRepository = cardRepository;
            _context = context;
        }

        public async Task<IEnumerable<Card>> GetCardsByColumnIdAsync(int columnId)
        {
            // Belirli bir sütuna ait kartları sırasına (Order) göre getiriyoruz
            return (await _cardRepository.FindAsync(c => c.ColumnId == columnId)).OrderBy(c => c.Order);
        }

        public async Task<Card?> GetCardByIdAsync(int id)
        {
            return await _cardRepository.GetByIdAsync(id);
        }

        public async Task CreateCardAsync(Card card)
        {
            await _cardRepository.AddAsync(card);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCardAsync(Card card)
        {
            _cardRepository.Update(card);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCardAsync(int id)
        {
            var card = await _cardRepository.GetByIdAsync(id);
            if (card != null)
            {
                _cardRepository.Delete(card);
                await _context.SaveChangesAsync();
            }
        }
    }
}