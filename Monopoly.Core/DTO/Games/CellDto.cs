using Monopoly.Core.Models.Abstractions.Classes;
using Monopoly.Core.Models.Game.Cells;

namespace Monopoly.Core.DTO.Games
{
    public class CellDto
    {
        public Guid Id { get; set; }
        public int Number {  get; set; }
        public string Name { get; set; }
        public string CellType { get; set; }
        public bool Special {  get; set; }

        public int? Price { get; set; }
        public int? Rent { get; set; }
        public int? Level { get; set; }
        public Guid? OwnerId { get; set; }
        public int? MonopolyIndex { get; set; }

        public CellDto(Cell cell)
        {
            Id = cell.Id;
            Number = cell.Number;
            Name = cell.Name;
            Special = cell.Special;
            CellType = cell.GetType().Name;

            if (cell is CompanyCell companyCell)
            {
                Price = companyCell.Price;
                Rent = companyCell.Rent;
                Level = companyCell.Level;
                OwnerId = companyCell.OwnerId;
                MonopolyIndex = companyCell.Monopoly.Index;
            }
        }
        public CellDto() { }
    }
}
