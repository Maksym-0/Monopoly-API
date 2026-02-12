using Monopoly.Core.Models.Room;

namespace Monopoly.Core.DTO.Rooms
{
    public class RoomDto
    {
        public Guid RoomId { get; set; }

        public int MaxNumberOfPlayers { get; set; }
        public int NumberOfPlayers { get; set; }

        public List<PlayerInRoomDto> Players { get; set; } = new();

        public Guid? GameId { get; set; }

        public bool HavePassword { get; set; }
        public bool InGame { get; set; }

        public RoomDto(Room room)
        {
            RoomId = room.Id;

            MaxNumberOfPlayers = room.MaxNumberOfPlayers;
            NumberOfPlayers = room.CountOfPlayers;

            foreach (var player in room.Players)
                Players.Add(new PlayerInRoomDto(player));

            if(room.Password != null)
                HavePassword = true;
            else 
                HavePassword = false;

            if (room.Game != null)
                GameId = room.Game.Id;
            
            InGame = room.InGame;
        }
    }
}
