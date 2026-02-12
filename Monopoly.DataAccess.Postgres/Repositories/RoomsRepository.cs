using Microsoft.EntityFrameworkCore;
using Monopoly.Core.Interfaces.Repositories;
using Monopoly.Core.Models.Room;

namespace Monopoly.DataAccess.Postgres.Repositories
{
    public class RoomsRepository : IRoomsRepository
    {
        private readonly MonopolyDbContext _context;

        public RoomsRepository(MonopolyDbContext context)
        {
            _context = context;
        }

        public async Task<List<Room>> GetRooms()
        {
            return await _context.Rooms
                .AsNoTracking()
                .Include(r => r.Players)
                    .ThenInclude(p => p.Account)
                .Include(r => r.Game)
                .ToListAsync();
        }
        public async Task<Room?> GetRoomById(Guid roomId)
        {
            return await _context.Rooms
                .Include(r => r.Players)
                    .ThenInclude(p => p.Account)
                .Include(r => r.Game)
                    .ThenInclude(g => g.Players)
                .FirstOrDefaultAsync(r => r.Id == roomId);
        }
        public async Task AddRoom(Room room)
        {
            await _context.AddAsync(room);
        }
        public async Task DeleteById(Guid roomId)
        {
            Room? room = await GetRoomById(roomId);

            if (room != null)
                _context.Rooms.Remove(room);
        }

        public async Task<List<PlayerInRoom>> GetPlayersByRoomId(Guid roomId)
        {
            return await _context.PlayersInRooms
                .Where(p => p.RoomId == roomId)
                .ToListAsync();
        }
        public async Task<PlayerInRoom?> GetPlayerById(Guid playerId)
        {
            return await _context.PlayersInRooms
                .FirstOrDefaultAsync(p => p.Id == playerId);
        }
        public async Task<PlayerInRoom?> GetPlayerByAccountId(Guid accountId)
        {
            return await _context.PlayersInRooms
                .Include(p => p.Room)
                    .ThenInclude(r => r.Game)
                .FirstOrDefaultAsync(p => p.AccountId == accountId);
        }
        public async Task AddPlayer(Guid roomId, PlayerInRoom player)
        {
            await _context.AddAsync(player);
        }
        public async Task DeletePlayerById(Guid roomId, Guid playerId)
        {
            PlayerInRoom? player = await GetPlayerById(playerId);

            if (player != null)
                _context.PlayersInRooms.Remove(player);
        }
    }
}
