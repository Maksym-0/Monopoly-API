namespace Monopoly.Core.Models.ApiRequest
{
    public class CreateTradeOfferRequest
    {
        public int OffererMoneyProposition { get; set; }
        public List<int> OffererCellNumbers { get; set; }

        public Guid OffereePlayerId { get; set; }
        public int OffereeMoneyProposition { get; set; }
        public List<int> OffereeCellNumbers { get; set; }
    }
}
