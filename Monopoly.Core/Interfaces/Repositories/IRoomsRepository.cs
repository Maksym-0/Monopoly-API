using Monopoly.Core.Models.Room;

namespace Monopoly.Core.Interfaces.Repositories
{
    public interface IRoomsRepository
    {
        public Task<List<Room>> GetRooms();
        public Task<Room?> GetRoomById(Guid roomId);
        public Task AddRoom(Room room);
        public Task DeleteById(Guid roomId);

        public Task<List<PlayerInRoom>> GetPlayersByRoomId(Guid roomId);
        public Task<PlayerInRoom?> GetPlayerById(Guid playerId);
        Task<PlayerInRoom?> GetPlayerByAccountId(Guid accountId);
        public Task AddPlayer(Guid roomId, PlayerInRoom player);
        public Task DeletePlayerById(Guid roomId, Guid playerId);
    }
}
