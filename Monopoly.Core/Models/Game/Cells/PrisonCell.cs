using Monopoly.Core.Models.Abstractions.Classes;

namespace Monopoly.Core.Models.Game.Cells
{
    public class PrisonCell : Cell
    {
        public PrisonCell(Guid id, Guid boardId, int number, string name) : base(id, boardId, number, name)
        {
            Special = true;
        }
        public PrisonCell() { }

        public override string ApplyEffect(Player player)
        {
            if (!player.IsPrisoner)
            {
                player.SetLocation(30);

                return $"{player.Name} потрапив на клітину В'язниця і відправляється на відпочинок";
            }
            else
            {
                if (player.Dices.CountOfDubles == 3)
                    return $"{player.Name} ув'язнено за перевищення швидкості";
                else if (player.CantAction > 0)
                    return $"{player.Name} лишається у в'язниці ще {player.CantAction} ходів";
                
                return $"{player.Name} ув'язнено";
            }
        }
    }
}
