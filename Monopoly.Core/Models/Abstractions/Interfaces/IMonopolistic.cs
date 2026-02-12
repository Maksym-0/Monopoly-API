namespace Monopoly.Core.Models.Abstractions.Interfaces
{
    public interface IMonopolistic
    {
        public Guid MonopolyId { get; }
        public Models.Game.Monopoly? Monopoly { get; }
    }
}
