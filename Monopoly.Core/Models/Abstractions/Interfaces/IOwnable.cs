using Monopoly.Core.Models.Game;

namespace Monopoly.Core.Models.Abstractions.Interfaces
{
    public interface IOwnable
    {
        public int Price { get; }

        public Guid? OwnerId { get; }
        public Player? Owner { get; }

        public void ChangeOwner(Player player);
    }
}
