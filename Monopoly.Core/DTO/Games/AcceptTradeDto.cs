using Monopoly.Core.Models.Game.OfferSystem;

namespace Monopoly.Core.DTO.Games
{
    public class AcceptTradeDto
    {
        public Guid OffererId { get; set; }
        public string OffererName { get; set; }
        public int NewOffererBalance { get; set; }
        public List<int> NewOffererCells { get; set; } = new List<int>();

        public Guid OffereeId { get; set; }
        public string OffereeName { get; set; }
        public int NewOffereeBalance { get; set; }
        public List<int> NewOffereeCells { get; set; } = new List<int>();

        public AcceptTradeDto(TradeOffer tradeOffer, int newOffererBalance, int newOffereeBalance)
        {
            OffererId = tradeOffer.OffererId;
            OffererName = tradeOffer.Offerer.Name;
            NewOffererBalance = newOffererBalance;
            NewOffererCells = tradeOffer.OffereeProposition.CellNumbers;

            OffereeId = tradeOffer.OffereeId;
            OffereeName = tradeOffer.Offeree.Name;
            NewOffereeBalance = newOffereeBalance;
            NewOffereeCells = tradeOffer.OffererProposition.CellNumbers;
        }
        public AcceptTradeDto() { }
    }
}
