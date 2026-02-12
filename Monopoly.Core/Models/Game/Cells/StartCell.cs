using Monopoly.Core.Models.Abstractions.Classes;

namespace Monopoly.Core.Models.Game.Cells
{
    public class StartCell : Cell
    {
        public StartCell(Guid id, Guid boardId, int number, string name) : base(id, boardId, number, name) 
        {
            Special = true;
        }
        public StartCell() { }

        public override string ApplyEffect(Player player)
        {
            player.ApplyToBalance(300);
            
            return $"{player.Name} потрапив на клітину Старт і отримав 300$";
        }
    }
}
