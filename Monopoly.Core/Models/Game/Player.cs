using Monopoly.Core.Models.Room;
using Monopoly.Core.Models.Abstractions.Interfaces;
using Monopoly.Core.Models.Game.Cells;
using Monopoly.Core.Models.Account;

namespace Monopoly.Core.Models.Game
{
    public class Player
    {
        public Guid Id { get; private set; }
        public Guid? AccountId { get; private set; }
        public Account.Account? Account { get; private set; }

        public string Name { get; private set; }
        public int Index { get; private set; }

        public int Balance { get; private set; } = 3000;
        public int Location { get; private set; } = 0;
        public int CantAction { get; internal set; } = 0;
        public int ReverseMove { get; internal set; } = 0;
        
        public bool InGame { get; private set; } = true;
        public bool IsPrisoner { get; internal set; } = false;

        public List<CompanyCell> OwnedCells { get; private set; } = new();

        public Dice? Dices { get; private set; } = new();
        
        public Guid GameId { get; private set; }
        public Game? Game { get; set; }

        public Player(Guid playerId, Guid accountId, Guid gameId, Account.Account account, string name, int index)
        {
            Id = playerId;
            AccountId = accountId;
            Account = account;
            GameId = gameId;
            Name = name;
            Index = index;
        }
        public Player() { }

        public void Move(int boardSize)
        {
            if (ReverseMove > 0)
            {
                Location = (Location - Dices.DiceSum + boardSize) % boardSize;
                ReverseMove--;
            }
            else
            {
                int oldLocation = Location;
                Location = (Location + Dices.DiceSum) % boardSize;
                if (Location < oldLocation)
                    Balance += Constants.BonusForLap;
            }
        }
        
        public void RollDices()
        {
            if (Dices == null)
                Dices = new Dice();
            
            Dices.Roll();
        }

        public void BuyCell(IOwnable ownableCell)
        {
            if (ownableCell.OwnerId != null)
                throw new Exception("Клітина вже має власника");

            bool result = ApplyToBalance(-ownableCell.Price);
            if (!result)
                throw new Exception("Недостатньо грошей для придбання клітини");

            ownableCell.ChangeOwner(this);

            if (ownableCell is CompanyCell companyCell)
            {
                OwnedCells.Add(companyCell);

            if (companyCell.Monopoly != null)
                companyCell.Monopoly.CheckAndApplyMonopoly();
            }
        }
        public void LevelUpCell(IUpgradable upgradableCell)
        {
            ApplyToBalance(-upgradableCell.UpgradeCost);
            upgradableCell.ChangeLevel(upgradableCell.Level + 1);
        }
        public void LevelDownCell(IUpgradable upgradableCell)
        {
            upgradableCell.ChangeLevel(upgradableCell.Level - 1);
            ApplyToBalance(upgradableCell.SellRefund);
        }

        public void Imprison()
        {
            Location = 11;
            IsPrisoner = true;
            CantAction = 3;
            Dices.SetCountOfDubles(0);
        }
        public void LeavePrison()
        {
            if (!IsPrisoner)
                throw new Exception("Гравець не є ув'язненим");

            IsPrisoner = false;
            CantAction = 0;
        }

        public bool ApplyToBalance(int amount)
        {
            if (Balance + amount >= 0)
            {
                Balance += amount;
                return true;
            }
            else
                return false;
        }
        public bool PayToLeavePrison(int leavePrisonCost)
        {
            if (!IsPrisoner)
                throw new Exception("Гравець не є ув'язненим");

            bool result = ApplyToBalance(-leavePrisonCost);

            if (result)
                return true;
            else 
                return false;
        }

        internal void SetReverseMove(int reverseMove)
        {
            if (reverseMove < 0)
                throw new Exception("Вкажіть невід'ємне значення");
            ReverseMove = reverseMove;
        }
        internal void SetCantAction(int cantAction)
        {
            if (cantAction < 0)
                throw new Exception("Вкажіть невід'ємне значення");
            CantAction = cantAction;
        }
        internal void SetLocation(int location)
        {
            if (location < 0 || location > Game.Board.Cells.Count)
                throw new Exception("Вкажіть існуюче значення");
            Location = location;
        }
        internal void LeaveGame()
        {
            InGame = false;
        }
    }
}
