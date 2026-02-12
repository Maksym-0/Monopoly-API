using Monopoly.Core.Models.Abstractions.Classes;
using Monopoly.Core.Models.Game.Cells;

namespace Monopoly.Core.Factories
{
    internal static class CellFactory
    {
        internal static List<Cell> CreateCells(Guid boardId)
        {
            List<Cell> cells = new();

            int price;
            int rent;

            for (int i = 0; i < Constants.CellNames.Count; i++)
            {
                string name = Constants.CellNames[i];
                if (Constants.SpecialCellNames.Contains(name))
                {
                    Cell cell;

                    switch (name)
                    {
                        case "Старт":
                            cell = new StartCell(Guid.NewGuid(), boardId, i, name);
                            break;
                        case "Швидка допомога":
                            cell = new AmbulanceCell(Guid.NewGuid(), boardId, i, name);
                            break;
                        case "Казино":
                            cell = new CasinoCell(Guid.NewGuid(), boardId, i, name);
                            break;
                        case "Відпочинок":
                            cell = new RestCell(Guid.NewGuid(), boardId, i, name);
                            break;
                        case "Мінус гроші":
                            cell = new LosingMoneyCell(Guid.NewGuid(), boardId, i, name);
                            break;
                        case "Плюс гроші":
                            cell = new ReceiveMoneyCell(Guid.NewGuid(), boardId, i, name);
                            break;
                        case "В'язниця":
                            cell = new PrisonCell(Guid.NewGuid(), boardId, i, name);
                            break;
                        case "Зворотній хід":
                            cell = new ReverceCell(Guid.NewGuid(), boardId, i, name);
                            break;
                        default:
                            throw new ArgumentException("Невідома особлика клітина при ініціалізації");
                    }

                    cells.Add(cell);
                }
                else
                {
                    price = Constants.CellPrices[i];
                    rent = Constants.CellStartRents[i];

                    CompanyCell companyCell = new CompanyCell(Guid.NewGuid(), boardId, i, name, price, rent);

                    cells.Add(companyCell);
                }
            }

            return cells;
        }
    }
}
