using Monopoly.Core.Models.Game;

namespace Monopoly.Core.DTO.Games
{
    public class GameStateDto
    {
        public Guid GameId { get; set; }

        public Guid RoomId { get; set; }

        public List<PlayerDto> Players { get; set; } = new();
        public BoardDto Board { get; set; }
        public TurnStateDto TurnState { get; set; }
        public TradeOfferDto? CurrentTradeOffer { get; set; }

        public GameStateDto(Game game)
        {
            GameId = game.Id;
            RoomId = game.RoomId;
            foreach (var player in game.Players)
            {
                Players.Add(new PlayerDto(player));
            }
            Board = new BoardDto(game.Board);
            TurnState = new TurnStateDto(game.TurnState);
            if (game.CurrentTradeOffer != null)
            {
                CurrentTradeOffer = new TradeOfferDto(game.CurrentTradeOffer);
            }
        }
        public GameStateDto() { }
    }
}
