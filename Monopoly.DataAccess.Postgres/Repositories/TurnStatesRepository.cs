using Microsoft.EntityFrameworkCore;
using Monopoly.Core.Interfaces.Repositories;
using Monopoly.Core.Models.Game;

namespace Monopoly.DataAccess.Postgres.Repositories
{
    public class TurnStatesRepository : ITurnStatesRepository
    {
        private readonly MonopolyDbContext _context;

        public TurnStatesRepository(MonopolyDbContext context)
        {
            _context = context;
        }

        public async Task<TurnState?> GetByGameId(Guid gameId)
        {
            return await _context.TurnStates
                .Include(ts => ts.Game)
                    .ThenInclude(g => g.Players)
                .FirstOrDefaultAsync(t => t.GameId == gameId);
        }
    }
}
