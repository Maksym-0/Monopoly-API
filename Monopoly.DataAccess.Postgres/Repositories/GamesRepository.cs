using Microsoft.EntityFrameworkCore;
using Monopoly.Core.Interfaces.Repositories;
using Monopoly.Core.Models.Game;

namespace Monopoly.DataAccess.Postgres.Repositories
{
    public class GamesRepository : IGamesRepository
    {
        private readonly MonopolyDbContext _context;

        public GamesRepository(MonopolyDbContext dbContext)
        {
            _context = dbContext;
        }

        public async Task<Game?> GetById(Guid id)
        {
            return await _context.Games
                .Include(g => g.TurnState)
                .Include(g => g.Players)
                    .ThenInclude(p => p.Account)
                .Include(g => g.Players)
                    .ThenInclude(p => p.Dices)
                .Include(g => g.Players)
                    .ThenInclude(p => p.OwnedCells)
                .Include(g => g.Board)
                    .ThenInclude(b => b.Cells)
                .Include(g => g.Board)
                    .ThenInclude(b => b.Monopolies)
                    .ThenInclude(m => m.Cells)
                .Include(g => g.CurrentTradeOffer)
                    .ThenInclude(t => t.OffererProposition)
                .Include(g => g.CurrentTradeOffer)
                    .ThenInclude(t => t.OffereeProposition)
                .Include(g => g.CurrentTradeOffer)
                    .ThenInclude(t => t.Offerer)
                .Include(g => g.CurrentTradeOffer)
                    .ThenInclude(t => t.Offeree)
                .FirstOrDefaultAsync(g => g.Id == id);
        }
        public async Task Add(Game game)
        {
            await _context.AddAsync(game);
        }
        public async Task DeleteById(Guid id)
        {
            var game = await _context.Games.FindAsync(id);
            
            if(game != null)
            {
                _context.Games.Remove(game);
            }
        }
    }
}
