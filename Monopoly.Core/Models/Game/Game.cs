using Monopoly.Core.Models.Room;

namespace Monopoly.Core.Models.Game
{
    public class Game
    {
        public Guid Id { get; private set; }

        public Guid RoomId { get; private set; }
        public Room.Room? Room { get; private set; }

        public GameBoard? Board { get; private set; }
        public List<Player> Players { get; private set; } = new();
        public TurnState? TurnState { get; private set; }

        public Game(Guid gameId, Guid roomId, Room.Room room, GameBoard board, List<Player> players, TurnState turnState)
        {
            Id = gameId;

            RoomId = roomId;
            Room = room;

            Board = board;
            Players = players;

            TurnState = turnState;
        }
        public Game() { }

        public void StartGame()
        {
            if (Players.Count < 2)
                throw new Exception("Недостатньо гравців для початку гри");

            TurnState.SetCurrentPlayerIndex(1);
            TurnState.StartTurn();
        }

        public Player? CheckWinner()
        {
            int playersInGame = 0;
            Player? winner = null;

            foreach (var player in Players)
            {
                if (player.InGame)
                {
                    playersInGame++;
                    winner = player;
                }
            }

            if (playersInGame == 1)
            {
                return winner;
            }
            else
                return null;
        }
        public void LeaveByPlayerId(Guid playerId)
        {
            Player? player = Players.FirstOrDefault(p => p.Id == playerId);
            if (player == null)
                throw new Exception("Граввець із цим Id не перебуває в грі");
            else if (!player.InGame)
                throw new Exception("Гравець вже покинув гру");

            if (player.Index == TurnState.CurrentPlayerIndex)
            {
                TurnState.NextTurn();
            }
            Board.ResetOwnerCells(player.Id);
            player.LeaveGame();
        }
        public void LeaveByAccountId(Guid accountId)
        {
            Player? player = Players.FirstOrDefault(p => p.AccountId == accountId);
            if (player == null)
                throw new Exception("Гравець із цим AccountId не перебуває в грі");
            else if (!player.InGame)
                throw new Exception("Гравець вже покинув гру");
            if (player.Index == TurnState.CurrentPlayerIndex)
            {
                TurnState.NextTurn();
            }
            Board.ResetOwnerCells(player.Id);
            player.LeaveGame();
        }
    }
}
