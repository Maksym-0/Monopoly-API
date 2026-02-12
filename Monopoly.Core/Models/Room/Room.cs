using Monopoly.Core.Models.Game;

namespace Monopoly.Core.Models.Room
{
    public class Room
    {
        public Guid Id { get; private set; }
        
        public Game.Game? Game { get; internal set; }

        public int MaxNumberOfPlayers { get; private set; }
        public int CountOfPlayers => Players.Count;
        public string? Password { get; private set; }
        
        public bool InGame { get; private set; }

        public List<PlayerInRoom> Players { get; private set; } = new();

        public Room(Guid roomId, int maxNumberOfPlayers, string? password)
        {
            Id = roomId;
            MaxNumberOfPlayers = maxNumberOfPlayers;
            Password = password;
            InGame = false;
        }
        public Room() { }

        public void AddPlayer(PlayerInRoom playerInRoom, string? password)
        {
            if (InGame)
                throw new Exception("Неможливо приєднатись. Гру вже розпочато");
            if (CountOfPlayers >= MaxNumberOfPlayers)
                throw new Exception("Неможливо приєднатись. Кімната заповнена");
            if (Password != password)
                throw new Exception("Неможливо приєднатись. Введено неправильний пароль");

            Players.Add(playerInRoom);
        }
        public void RemovePlayerById(Guid playerId)
        {
            var playerToRemove = Players.FirstOrDefault(p => p.Id == playerId);
            if (playerToRemove == null)
                throw new Exception("Гравець не може покинути кімнату, в якій не перебуває");
            
            foreach (var player in Players)
            {
                if (player.Index > playerToRemove.Index)
                {
                    player.ChangeIndex(player.Index - 1);
                }
            }

            Players.Remove(playerToRemove);
        }
        public void RemovePlayerByAccountId(Guid accountId)
        {
            var player = Players.FirstOrDefault(p => p.AccountId == accountId);
            if (player == null)
                throw new Exception("Гравець не може покинути кімнату, в якій не перебуває");
            
            Players.Remove(player);
        }

        public void StartGame()
        {
            if (InGame)
                throw new Exception("Помилка початку гри. Гру вже розпочато");
            else if (CountOfPlayers < 2)
                throw new Exception("Неможливо розпочати гру. В кімнаті менше двох гравців");
            
            InGame = true;
        }
    }
}
