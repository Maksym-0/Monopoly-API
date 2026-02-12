using Monopoly.Core.DTO.Rooms;
using Monopoly.Core.Models.Service;

namespace Monopoly.Core.Interfaces.Services
{
    public interface IRoomService
    {
        Task<ServiceResponse<List<RoomDto>>> GetAllRoomsAsync();
        Task<ServiceResponse<RoomDto>> CreateRoomAsync(int maxNumberOfPlayers, string? password, Guid accountId);
        Task<ServiceResponse<JoinRoomDto>> JoinRoomAsync(Guid roomId, string? password, Guid accountId);
        Task<ServiceResponse<QuitRoomDto>> QuitRoomAsync(Guid accountId);
    }
}
