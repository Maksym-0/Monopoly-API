namespace Monopoly.Core.DTO.Rooms
{
    public class JoinRoomDto
    {
        public bool Success { get; set; }
        public bool IsGameStarted { get; set; }

        public RoomDto Room { get; set; }
    }
}
