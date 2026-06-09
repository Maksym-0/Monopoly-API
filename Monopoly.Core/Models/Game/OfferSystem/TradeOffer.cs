namespace Monopoly.Core.Models.Game.OfferSystem
{
    public class TradeOffer
    {
        public Guid Id { get; private set; }

        public Guid GameId { get; private set; }
        public Game Game { get; private set; }
        
        public Guid OffererId { get; private set; }
        public Player Offerer { get; private set; }
        public Guid OffererPropositionId { get; private set; }
        public Proposition OffererProposition { get; private set; }

        public Guid OffereeId { get; private set; }
        public Player Offeree { get; private set; }
        public Guid OffereePropositionId { get; private set; }
        public Proposition OffereeProposition { get; private set; }

        public TradeOffer(Guid id, Game game, Player offerer, Proposition offererProposition, Player offeree, Proposition offereeProposition)
        {
            Id = id;

            GameId = game.Id;
            Game = game;

            OffererId = offerer.Id;
            Offerer = offerer;
            OffererPropositionId = offererProposition.Id;
            OffererProposition = offererProposition;

            OffereeId = offeree.Id;
            Offeree = offeree;
            OffereePropositionId = offereeProposition.Id;
            OffereeProposition = offereeProposition;
        }
        public TradeOffer() { }
    }
}
