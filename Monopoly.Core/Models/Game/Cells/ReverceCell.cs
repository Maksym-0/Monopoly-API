using Monopoly.Core.Models.Abstractions.Classes;

namespace Monopoly.Core.Models.Game.Cells
{
    public class ReverceCell : Cell
    {
        public ReverceCell(Guid id, Guid boardId, int number, string name) : base(id, boardId, number, name)
        {
            Special = true;
        }
        public ReverceCell() { }

        public override string ApplyEffect(Player player)
        {
            player.SetReverseMove(1);

            return $"{player.Name} потрапив на клітину Зворотній хід. Наступний хід відбудеться у зворотньому напрямку";
        }
    }
}
