using Monopoly.Core.Models.Abstractions.Classes;

namespace Monopoly.Core.Models.Game.Cells
{
    public class ReceiveMoneyCell : Cell
    {
        public ReceiveMoneyCell(Guid id, Guid boardId, int number, string name) : base(id, boardId, number, name)
        {
            Special = true;
        }
        public ReceiveMoneyCell() { }

        private static Random random = Random.Shared;

        public override string ApplyEffect(Player player)
        {
            int plus = random.Next(1, 4) * 100;
            player.ApplyToBalance(plus);

            return $"{player.Name} потрапив на клітину Плюс гроші. Нараховано {plus}$";
        }
    }
}
