using Monopoly.Core.Models.Abstractions.Classes;

namespace Monopoly.Core.Models.Game.Cells
{
    public class AmbulanceCell : Cell
    {
        public AmbulanceCell(Guid id, Guid boardId, int number, string name) : base(id, boardId, number, name)
        {
            Special = true;
        }
        public AmbulanceCell() { }

        public override string ApplyEffect(Player player)
        {
            player.SetCantAction(1);
            player.Dices.SetCountOfDubles(0);

            return $"{player.Name} потрапив на клітину Швидка допомога і пропускає один хід";
        }
    }
}
