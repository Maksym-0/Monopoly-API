namespace Monopoly.Core.Models.Game
{
    public class TurnState
    {
        public Guid Id { get; internal set; }

        public Guid GameId { get; internal set; }
        public Game Game { get; internal set; }

        public int CurrentPlayerIndex { get; internal set; } = 0;

        public bool NeedPay { get; internal set; } = false;
        public bool CanRollDices { get; internal set; } = false;
        public bool CanBuyCell { get; internal set; } = false;
        public bool CanLevelUpCell { get; internal set; } = false;

        public TurnState(Guid id, Guid gameId)
        {
            Id = id;
            GameId = gameId;
        }
        public TurnState() { }

        public Player NextTurn()
        {
            if (Game.Players.FirstOrDefault(p => p.InGame == true) == null)
                throw new Exception("В грі не лишилось жодного гравця, здатного розпочати хід");

            int nextIndex = CurrentPlayerIndex;
            Player nextPlayer;

            while (true)
            {
                nextIndex = (nextIndex % Game.Players.Count) + 1;
                nextPlayer = Game.Players[Game.Players.FindIndex(p => p.Index == nextIndex)];

                if (!nextPlayer.InGame)
                    continue;
                
                if (nextPlayer.IsPrisoner)
                {
                    if(nextPlayer.CantAction < 1)
                    {
                        nextPlayer.IsPrisoner = false;
                    }
                    else
                    {
                        nextPlayer.CantAction--;
                    }
                    CurrentPlayerIndex = nextIndex;
                    break;
                }
                else if(nextPlayer.CantAction > 0)
                {
                    nextPlayer.CantAction--;
                    continue;
                }
                else
                {
                    CurrentPlayerIndex = nextIndex;
                    break;
                }
            }

            StartTurn();
            
            return nextPlayer;
        }

        public void StartTurn()
        {
            Reset();
            CanRollDices = true;
            CanLevelUpCell = true;
        }
        public void EnableCanRollDices()
        {
            CanRollDices = true;
        }
        public void DisableCanRollDices()
        {
            CanRollDices = false;
        }
        public void EnableBuying()
        {
            CanBuyCell = true;
        }
        public void DisableBuying()
        {
            CanBuyCell = false;
        }
        public void AllowLevelUp()
        {
            CanLevelUpCell = true;
        }
        public void ForbidLevelUp()
        {
            CanLevelUpCell = false;
        }
        public void RequirePayment()
        {
            NeedPay = true;
        }
        public void CompletePayment()
        {
            NeedPay = false;
        }

        internal void SetCurrentPlayerIndex(int index)
        {
            if(index < 0)
                throw new Exception("Неможливо встановити від'ємний індекс");
            CurrentPlayerIndex = index;
        }

        private void Reset()
        {
            NeedPay = false;
            CanRollDices = false;
            CanBuyCell = false;
            CanLevelUpCell = false;
    }
    }
}
