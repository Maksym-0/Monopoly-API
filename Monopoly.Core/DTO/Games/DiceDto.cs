using Monopoly.Core.Models.Game;

namespace Monopoly.Core.DTO.Games
{
    public class DiceDto
    {
        public int Dice1 { get; set; }
        public int Dice2 { get; set; }
        public int DiceSum {  get; set; }

        public bool Dubl {  get; set; }
        public int CountOfDubles { get; set; }

        public DiceDto(Dice dices)
        {
            Dice1 = dices.Dice1;
            Dice2 = dices.Dice2;
            DiceSum = dices.DiceSum;
            Dubl = dices.Double;
            CountOfDubles = dices.CountOfDubles;
        }
        public DiceDto() { }
    }
}
