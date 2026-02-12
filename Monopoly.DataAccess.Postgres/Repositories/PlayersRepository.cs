using Microsoft.EntityFrameworkCore;
using Monopoly.Core.Interfaces.Repositories;
using Monopoly.Core.Models.Game;

namespace Monopoly.DataAccess.Postgres.Repositories
{
    public class PlayersRepository : IPlayersRepository
    {
        private readonly MonopolyDbContext _context;

        public PlayersRepository(MonopolyDbContext context)
        {
            _context = context;
        }

        public async Task<List<Player>> GetListByGameId(Guid gameId)
        {
            return await _context.Players
                .Where(p => p.GameId == gameId)
                .ToListAsync();
        }
        public async Task<Player?> GetById(Guid gameId, Guid playerId)
        {
            return await _context.Players
                .Include(p => p.Account)
                .Include(p => p.OwnedCells)
                .FirstOrDefaultAsync(p => p.GameId == gameId && p.Id == playerId);
        }
        public async Task<Player?> GetByAccountId(Guid accountId)
        {
            return await _context.Players
                .Include(p => p.Account)
                .Include(p => p.OwnedCells)
                .FirstOrDefaultAsync(p => p.AccountId == accountId);
        }
    }
}
