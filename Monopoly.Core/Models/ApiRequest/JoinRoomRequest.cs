namespace Monopoly.Core.Models.Request
{
    public class JoinRoomRequest
    {
        public Guid RoomId { get; set; }
        public string? Password { get; set; }
    }
}
