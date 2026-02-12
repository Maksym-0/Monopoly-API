using Monopoly.Core.Models.Game;

namespace Monopoly.Core.DTO.Games
{
    public class TurnStateDto
    {
        public int CurrentPlayerIndex { get; set; }

        public bool NeedPay { get; set; }
        public bool CanRollDices { get; set; }
        public bool CanBuyCell { get; set; } 
        public bool CanLevelUpCell { get; set; }

        public TurnStateDto(TurnState state)
        {
            CurrentPlayerIndex = state.CurrentPlayerIndex;
            NeedPay = state.NeedPay;
            CanRollDices = state.CanRollDices;
            CanBuyCell = state.CanBuyCell;
            CanLevelUpCell = state.CanLevelUpCell;
        }
        public TurnStateDto() { }
    }
}
