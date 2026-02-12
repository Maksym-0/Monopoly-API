using Monopoly.Core.Models.Game;
using Monopoly.Core.Models.Room;

namespace Monopoly.Core.Factories
{
    public static class GameFactory
    {
        public static Game CreateGame(Room room)
        {
            Guid gameId = Guid.NewGuid();
            
            GameBoard board = BoardFactory.CreateBoard(gameId);
            List<Player> players = PlayerFactory.CreatePlayers(room.Players, gameId);
            TurnState turnState = new TurnState(Guid.NewGuid(), gameId);

            Game game = new Game(gameId, room.Id, room, board, players, turnState);

            game.Board.Game = game;
            game.TurnState.Game = game;
            foreach (var player in game.Players)
                player.Game = game;

            room.Game = game;

            return game;
        }
    }
}
