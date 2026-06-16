using Monopoly.Core.Models.Room;

namespace Monopoly.Core.Interfaces.Repositories
{
    public interface IRoomsRepository
    {
        public Task<List<Room>> GetRoomsAsync();
        public Task<Room?> GetRoomByIdAsync(Guid roomId);
        public Task AddRoomAsync(Room room);
        public Task DeleteByIdAsync(Guid roomId);

        public Task<List<PlayerInRoom>> GetPlayersByRoomIdAsync(Guid roomId);
        public Task<PlayerInRoom?> GetPlayerByIdAsync(Guid playerId);
        Task<PlayerInRoom?> GetPlayerByAccountIdAsync(Guid accountId);
        public Task AddPlayerAsync(PlayerInRoom player);
        public Task DeletePlayerByIdAsync(Guid roomId, Guid playerId);
    }
}
