using Monopoly.Core.Models.Abstractions.Classes;
using Monopoly.Core.Models.Abstractions.Interfaces;

namespace Monopoly.Core.Models.Game.Cells
{
    public class CompanyCell : Cell, IOwnable, IUpgradable, IMoneyEater, IMonopolistic
    {
        public CompanyCell(Guid id, Guid boardId, int number, string name, int price, int rent) : base(id, boardId, number, name) 
        {
            Special = false;
            Name = name;
            Number = number;
            Price = price;
            Rent = rent;
            OwnerId = null;
            Level = 0;
        }
        public CompanyCell() { }

        public int Price { get; private set; }
        public int Rent { get; private set; }

        public int UpgradeCost => Constants.CellUpgradAndSellCost[Number];
        public int SellRefund => Constants.CellUpgradAndSellCost[Number];
        public int Level { get; private set; }

        public Guid MonopolyId { get; private set; }
        public Monopoly? Monopoly { get; private set; }

        public Guid? OwnerId { get; private set; }
        public Player? Owner { get; private set; }

        public override string ApplyEffect(Player player)
        {
            string message;

            if (OwnerId == null)
            {
                Board.Game.TurnState.CanBuyCell = true;
                message = $"{player.Name} завершив рух. Клітину можна купити";
            }
            else
            {
                if (OwnerId == player.Id)
                {
                    message = $"{player.Name} потрапив на свою клітину. Рента не стягується";
                    return message;
                }
                else
                {
                    Board.Game.TurnState.NeedPay = true;
                    message = $"{player.Name} потрапив на територію чужої компанії. До сплати {Rent}$";
                }
            }

            return message;
        }

        public void ChangeOwner(Player player)
        {
            OwnerId = player.Id;
            Owner = player;
        }
        public void ChangeLevel(int newLevel)
        {
            if (Special)
                throw new Exception("Неможливо змінювати рівень унікальної клітини");
            if (!Monopoly.IsMonopoly)
                throw new Exception("Неможливо змінювати рівень клітини, що немає монополії");

            switch (newLevel)
            {
                case 0:
                    if (Monopoly.IsMonopoly)
                        Rent = Constants.CellBaseMonopolyRents[Number];
                    else
                        Rent = Constants.CellStartRents[Number];
                    Level = 0;
                    break;
                case 1:
                    Rent = Constants.CellLevel1Rents[Number];
                    Level = 1;
                    break;
                case 2:
                    Rent = Constants.CellLevel2Rents[Number];
                    Level = 2;
                    break;
                case 3:
                    Rent = Constants.CellLevel3Rents[Number];
                    Level = 3;
                    break;
                case 4:
                    Level = 4;
                    Rent = Constants.CellLevel4Rents[Number];
                    break;
                case 5:
                    Level = 5;
                    Rent = Constants.CellLevel5Rents[Number];
                    break;
                default:
                    throw new Exception("Обраного рівня не існує. Оберіть рівень від 0 до 5");
            }
        }
        
        internal void SetMonopoly(Monopoly monopoly)
        {
            MonopolyId = monopoly.Id;
            Monopoly = monopoly;
        }
        internal void SetRent(int rent)
        {
            Rent = rent;
        }
        internal void ResetOwner()
        {
            OwnerId = null;
            Owner = null;
        }
    }
}
