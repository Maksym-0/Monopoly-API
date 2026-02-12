using Monopoly.Core.Models.Abstractions.Classes;

namespace Monopoly.Core.Models.Game.Cells
{
    public class RestCell : Cell
    {
        public RestCell(Guid id, Guid boardId, int number, string name) : base(id, boardId, number, name)
        {
            Special = true;
        }
        public RestCell() { }

        public override string ApplyEffect(Player player)
        {
            player.Imprison();

            return $"{player.Name} потрапив на клітину Відпочинок і йде до тюрми";
        }
    }
}
