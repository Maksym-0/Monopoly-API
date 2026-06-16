using Monopoly.Core.Models.Game;
using Monopoly.Core.Models.Game.OfferSystem;

namespace Monopoly.Core.Interfaces.Repositories
{
    public interface IGamesRepository
    {
        public Task<Game?> GetByIdAsync(Guid id);
        public Task AddAsync(Game game);
        public Task AddTradeOfferAsync(TradeOffer tradeOffer);
        public Task DeleteByIdAsync(Guid id);
    }
}
