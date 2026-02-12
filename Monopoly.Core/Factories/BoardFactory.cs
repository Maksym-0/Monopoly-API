using Monopoly.Core.Models.Abstractions.Classes;
using Monopoly.Core.Models.Game;
using Monopoly.Core.Models.Game.Cells;

namespace Monopoly.Core.Factories
{
    internal static class BoardFactory
    {
        internal static GameBoard CreateBoard(Guid gameId)
        {
            Guid boardId = Guid.NewGuid();

            List<Cell> cells = CellFactory.CreateCells(boardId);
            List<Models.Game.Monopoly> monopolies = MonopolyFactory.CreateMonopolies(boardId);

            for (int i = 0; i < cells.Count; i++)
            {
                if (cells[i] is CompanyCell companyCell)
                {
                    int monopolyIndex = Constants.CellsMonopolyIndex[i];
                    if (monopolyIndex != 0)
                    {
                        Models.Game.Monopoly? monopoly = monopolies.FirstOrDefault(m => m.Index == monopolyIndex);
                        if (monopoly == null)
                        {
                            throw new Exception($"Помилка при ініціалізації гри: не знайдено монополію з індексом {monopolyIndex} для клітини {companyCell.Name}");
                        }
                        companyCell.SetMonopoly(monopoly);
                        monopoly.Cells.Add(companyCell);
                    }
                }
            }

            return new GameBoard(boardId, gameId, cells, monopolies);
        }
    }
}
