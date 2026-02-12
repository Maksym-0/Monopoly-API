using Monopoly.Core.Models.Abstractions.Classes;
using Monopoly.Core.Models.Abstractions.Interfaces;

namespace Monopoly.Core.Models.Game.Cells
{
    public class LosingMoneyCell : Cell, IMoneyEater
    {
        public LosingMoneyCell(Guid id, Guid boardId, int number, string name) : base(id, boardId, number, name)
        {
            Special = true;
        }
        public LosingMoneyCell() { }

        public int Rent { get; private set; }
        private static Random random = Random.Shared;

        public override string ApplyEffect(Player player)
        {
            int lose = random.Next(0, 4) * 100;
            Rent = lose;
            if (lose == 0)
                return $"{player.Name} потрапив на клітину Мінус гроші, але йому пощастило і сплачувати рахунки не потрібно!";
            
            Board.Game.TurnState.RequirePayment();
            return $"{player.Name} потрапив на клітину Мінус гроші. До сплати {lose}$";
        }
    }
}
