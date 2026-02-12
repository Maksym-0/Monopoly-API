using Monopoly.Core.Models.Abstractions.Classes;
using System;

namespace Monopoly.Core.Models.Game.Cells
{
    public class CasinoCell : Cell
    {
        public CasinoCell(Guid id, Guid boardId, int number, string name) : base(id, boardId, number, name)
        {
            Special = true;
        }
        public CasinoCell() { }

        private static Random random = Random.Shared;
        
        public override string ApplyEffect(Player player)
        {
            int win = random.Next(-3, 4) * 100;
            player.ApplyToBalance(win);

            return $"{player.Name} зайшов у Казино. Результат: {win}";
        }
    }
}
