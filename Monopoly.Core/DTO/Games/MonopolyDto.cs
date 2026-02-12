using Monopoly.Core.Models.Game;

namespace Monopoly.Core.DTO.Games
{
    public class MonopolyDto
    {
        public int Index { get; set; }
        public string Type { get; set; }

        public bool IsMonopoly { get; set; }

        public MonopolyDto(Models.Game.Monopoly monopoly)
        {
            Index = monopoly.Index;
            Type = monopoly.Type;
            IsMonopoly = monopoly.IsMonopoly;
        }
        public MonopolyDto() { }
    }
}
