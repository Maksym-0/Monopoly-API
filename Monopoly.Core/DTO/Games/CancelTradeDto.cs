namespace Monopoly.Core.DTO.Games
{
    public class CancelTradeDto
    {
        public Guid CancelerId { get; set; }
        public string CancelerName { get; set; }

        public CancelTradeDto(Guid cancelerId, string cancelerName)
        {
            CancelerId = cancelerId;
            CancelerName = cancelerName;
        }
        public CancelTradeDto() { }
    }
}
