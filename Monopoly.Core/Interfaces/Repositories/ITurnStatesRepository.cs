using Monopoly.Core.Models.Game;

namespace Monopoly.Core.Interfaces.Repositories
{
    public interface ITurnStatesRepository
    {
        public Task<TurnState?> GetByGameId(Guid gameId);
    }
}
