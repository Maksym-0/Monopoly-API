using Microsoft.EntityFrameworkCore;
using Monopoly.Core.Models.Abstractions.Classes;
using Monopoly.Core.Models.Game;
using Monopoly.Core.Interfaces.Repositories;

namespace Monopoly.DataAccess.Postgres.Repositories
{
    public class BoardsRepository : IBoardsRepository
    {
        private readonly MonopolyDbContext _context;

        public BoardsRepository(MonopolyDbContext context)
        {
            _context = context;
        }

        public async Task<GameBoard?> GetByGameId(Guid gameId)
        {
            return await _context.Boards
                .Include(b => b.Cells)
                .Include(b => b.Monopolies)
                    .ThenInclude(m => m.Cells)
                .FirstOrDefaultAsync(b => b.GameId == gameId);
        }

        public async Task<List<Cell>> GetCellsByGameId(Guid gameId)
        {
            return await _context.Cells
                .Where(c => c.Board.GameId == gameId)
                .ToListAsync();
        }
        public async Task<Cell?> GetCellByNumber(Guid gameId, int number)
        {
            return await _context.Cells
                .FirstOrDefaultAsync(c => c.Board.GameId == gameId && c.Number == number);
        }

        public async Task<List<Core.Models.Game.Monopoly>> GetMonopoliesByGameId(Guid gameId)
        {
            return await _context.Monopolies
                .Where(m => m.Board.GameId == gameId)
                .ToListAsync();
        }
        public async Task<Core.Models.Game.Monopoly?> GetMonopolyById(Guid gameId, Guid monopolyId)
        {
            return await _context.Monopolies
                .FirstOrDefaultAsync(m => m.Board.GameId == gameId && m.Id == monopolyId);
        }
    }
}
