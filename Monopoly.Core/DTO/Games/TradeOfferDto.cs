using Monopoly.Core.Models.Game.OfferSystem;

namespace Monopoly.Core.DTO.Games
{
    public class TradeOfferDto
    {
        public Guid OffererAccountId { get; set; }
        public string OffererName { get; set; }
        public PropositionDto OffererProposition { get; set; }

        public Guid OffereeAccountId { get; set; }
        public string OffereeName { get; set; }
        public PropositionDto OffereeProposition { get; set; }
    
        public TradeOfferDto(TradeOffer tradeOffer)
        {
            OffererAccountId = tradeOffer.Offerer.AccountId.Value;
            OffererName = tradeOffer.Offerer.Name;
            OffererProposition = new PropositionDto(tradeOffer.OffererProposition);
            OffereeAccountId = tradeOffer.Offeree.AccountId.Value;
            OffereeName = tradeOffer.Offeree.Name;
            OffereeProposition = new PropositionDto(tradeOffer.OffereeProposition);
        }
        public TradeOfferDto() { }
    }
}
