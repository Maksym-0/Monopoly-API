using Monopoly.Core.Models.Game;

namespace Monopoly.Core.DTO.Games
{
    public class PlayerDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public int Index { get; set; }

        public int Balance { get; set; } 
        public int Location { get; set; }
        public int CantAction { get; set; }
        public int ReverseMove { get; set; } 

        public bool InGame { get; set; }
        public bool IsPrisoner { get; set; }

        public DiceDto Dices { get; set; }

        public PlayerDto(Player player)
        {
            Id = player.Id;
            Name = player.Name;
            Index = player.Index;
            Balance = player.Balance;
            Location = player.Location;
            CantAction = player.CantAction;
            ReverseMove = player.ReverseMove;
            InGame = player.InGame;
            IsPrisoner = player.IsPrisoner;
            Dices = new DiceDto(player.Dices);
        }
        public PlayerDto() { }
    }
}
