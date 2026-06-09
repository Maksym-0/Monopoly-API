using Monopoly.Core.Models.Game.OfferSystem;

namespace Monopoly.Core.DTO.Games
{
    public class PropositionDto
    {
        public int Money { get; set; }
        public List<int> CellNumbers { get; set; }

        public PropositionDto(Proposition proposition)
        {
            Money = proposition.Money;
            CellNumbers = proposition.CellNumbers;
        }
        public PropositionDto() { }
    }
}
