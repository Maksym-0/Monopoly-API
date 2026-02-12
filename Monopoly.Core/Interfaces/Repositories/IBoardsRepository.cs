using Monopoly.Core.Models.Abstractions.Classes;
using Monopoly.Core.Models.Game.Cells;
using Monopoly.Core.Models.Game;

namespace Monopoly.Core.Interfaces.Repositories
{
    public interface IBoardsRepository
    {
        public Task<GameBoard?> GetByGameId(Guid gameId);

        public Task<List<Cell>> GetCellsByGameId(Guid gameId);
        public Task<Cell?> GetCellByNumber(Guid gameId, int number);

        public Task<List<Core.Models.Game.Monopoly>> GetMonopoliesByGameId(Guid gameId);
        public Task<Core.Models.Game.Monopoly?> GetMonopolyById(Guid gameId, Guid monopolyId);
    }
}
