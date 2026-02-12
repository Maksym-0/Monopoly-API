using Monopoly.Core.Models.Abstractions.Classes;
using Monopoly.Core.Models.Game.Cells;

namespace Monopoly.Core.Models.Game
{
    public class GameBoard
    {
        public Guid Id { get; set; }
        
        public Guid GameId { get; set; }
        public Game Game { get; set; }

        public List<Cell> Cells { get; set; } = new();
        public List<Monopoly> Monopolies { get; set; } = new();

        public GameBoard(Guid id, Guid gameId, List<Cell> cells, List<Monopoly> monopolies)
        {
            Id = id;
            GameId = gameId;
            Cells = cells;
            Monopolies = monopolies;
        }
        public GameBoard() { }

        public void ResetOwnerCells(Guid ownerId)
        {
            foreach(var cell in Cells)
            {
                if(cell is CompanyCell companyCell)
                {
                    if (companyCell.OwnerId == ownerId)
                    {
                        companyCell.ResetOwner();
                        if (companyCell.Monopoly.IsMonopoly)
                        {
                            companyCell.Monopoly.ResetMonopoly();
                        }
                    }
                }
            }
        }
    }
}
