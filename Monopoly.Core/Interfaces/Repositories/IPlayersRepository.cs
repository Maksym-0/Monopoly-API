using Monopoly.Core.Models.Game;

namespace Monopoly.Core.Interfaces.Repositories
{
    public interface IPlayersRepository 
    {
        Task<List<Player>> GetListByGameId(Guid gameId);
        Task<Player?> GetById(Guid gameId, Guid playerId);
        Task<Player?> GetByAccountId(Guid accountId);
    }
}
