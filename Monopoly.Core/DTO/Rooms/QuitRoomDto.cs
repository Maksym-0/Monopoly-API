using Monopoly.Core.DTO.Games;

namespace Monopoly.Core.DTO.Rooms
{
    public class QuitRoomDto
    {
        public bool IsRoomDeleted { get; set; }
       
        public string? PlayerName { get; set; }
        public PlayerDto? Winner { get; set; }
        
        public int? RemainingPlayers { get; set; }

        public RoomDto? RoomDto { get; set; }
    }
}
