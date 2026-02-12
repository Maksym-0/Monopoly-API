using Monopoly.Core.Models.Account;
using Monopoly.Core.Models.Room;

namespace Monopoly.Core.DTO.Rooms
{
    public class PlayerInRoomDto
    {
        public Guid Id { get; set; }

        public Guid AccountId { get; set; }

        public string Name { get; set; }
        public int Index { get; set; }

        public Guid RoomId { get; set; }

        public PlayerInRoomDto(PlayerInRoom player)
        {
            Id = player.Id;
            AccountId = player.AccountId;
            Name = player.Name;
            Index = player.Index;
            RoomId = player.RoomId;
        }
        public PlayerInRoomDto() { }
    }
}
