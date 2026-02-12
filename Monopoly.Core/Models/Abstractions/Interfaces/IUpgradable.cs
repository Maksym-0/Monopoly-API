namespace Monopoly.Core.Models.Abstractions.Interfaces
{
    public interface IUpgradable
    {
        public int UpgradeCost { get; }
        public int SellRefund { get; }

        public int Level { get; }

        public void ChangeLevel(int newLevel);
    }
}
