using Monopoly.Core.Models.Game.OfferSystem;

namespace Monopoly.Core.DTO.Games
{
    public class TradeOfferDto
    {
        public Guid OffererAccountId { get; set; }
        public PropositionDto OffererProposition { get; set; }

        public Guid OffereeAccountId { get; set; }
        public PropositionDto OffereeProposition { get; set; }
    
        public TradeOfferDto(TradeOffer tradeOffer)
        {
            OffererAccountId = tradeOffer.Offerer.AccountId.Value;
            OffererProposition = new PropositionDto(tradeOffer.OffererProposition);
            OffereeAccountId = tradeOffer.Offeree.AccountId.Value;
            OffereeProposition = new PropositionDto(tradeOffer.OffereeProposition);
        }
        public TradeOfferDto() { }
    }
}
