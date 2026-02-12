using Monopoly.Core.Models.Abstractions.Classes;
using Monopoly.Core.Models.Game;
using Monopoly.Core.Models.Game.Cells;
using Monopoly.Core.Models.Room;

namespace Monopoly.Core.Factories
{
    public static class GameFactory
    {
        public static Game CreateGame(Room room)
        {
            Guid gameId = Guid.NewGuid();
            Guid boardId = Guid.NewGuid();

            List<Cell> cells = CellFactory.CreateCells(boardId);
            List<Models.Game.Monopoly> monopolies = MonopolyFactory.CreateMonopolies(boardId);

            for(int i = 0; i < cells.Count; i++)
            {
                if (cells[i] is CompanyCell companyCell)
                {
                    int monopolyIndex = Constants.CellsMonopolyIndex[i];
                    if(monopolyIndex != 0)
                    {
                        Models.Game.Monopoly? monopoly = monopolies.FirstOrDefault(m => m.Index == monopolyIndex);
                        if(monopoly == null)
                        {
                            throw new Exception($"Помилка при ініціалізації гри: не знайдено монополію з індексом {monopolyIndex} для клітини {companyCell.Name}");
                        }
                        companyCell.SetMonopoly(monopoly);
                        monopoly.Cells.Add(companyCell);
                    }
                }
            }

            GameBoard board = new GameBoard(boardId, gameId, cells, monopolies);
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
