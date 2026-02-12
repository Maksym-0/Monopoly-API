using Monopoly.Core.Models.Game.Cells;

namespace Monopoly.Core.Models.Game
{
    public class Monopoly
    {
        public Guid Id { get; private set; }
        
        public int Index { get; internal set; }
        public string Type => Constants.CellsMonopolyTypes[Index-1];

        public bool IsMonopoly { get; internal set; } = false;

        public Guid BoardId {  get; internal set; }
        public GameBoard? Board { get; internal set; }

        public List<CompanyCell> Cells { get; internal set; } = new();

        public Monopoly(Guid id, Guid boardId, int index)
        {
            Id = id;
            BoardId = boardId;
            Index = index;
        }

        public Monopoly() { }

        public bool CheckAndApplyMonopoly() 
        {
            if(Cells.Count == 0)
            {
                IsMonopoly = false;
                return false;
            }
            
            var ownerId = Cells[0].OwnerId;

            if (ownerId == null)
            {
                IsMonopoly = false;
                return false;
            }

            bool result = Cells.All(c => c.OwnerId == ownerId);
            if(result)
            {
                IsMonopoly = true;
                UpdateCellsRent();
            }
            return result;
        }
        private void UpdateCellsRent()
        {
            foreach(var cell in Cells)
            {
                if (IsMonopoly)
                    cell.SetRent(Constants.CellBaseMonopolyRents[cell.Number]);
                else
                    cell.SetRent(Constants.CellStartRents[cell.Number]);
            }
        }

        public void ResetMonopoly()
        {
            foreach(var cell in Cells)
            {
                cell.ChangeLevel(0);
            }
            IsMonopoly = false;
        }
    }
}
