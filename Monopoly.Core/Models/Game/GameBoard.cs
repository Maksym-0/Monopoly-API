using Monopoly.Core.Models.Abstractions.Classes;
using Monopoly.Core.Models.Game.Cells;

namespace Monopoly.Core.Models.Game
{
    public class GameBoard
    {
        public Guid Id { get; private set; }
        
        public Guid GameId { get; private set; }
        public Game Game { get; internal set; }

        public List<Cell> Cells { get; private set; } = new();
        public List<Monopoly> Monopolies { get; private set; } = new();

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
