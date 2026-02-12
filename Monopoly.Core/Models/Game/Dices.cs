namespace Monopoly.Core.Models.Game
{
    public class Dice
    {
        public Guid Id { get; private set; }

        public int Dice1 { get; private set; }
        public int Dice2 {  get; private set; }
        public int DiceSum => Dice1 + Dice2;

        public bool Double => Dice1 == Dice2;
        public int CountOfDubles { get; private set; }

        public Guid PlayerId { get; private set; }
        public Player Player { get; private set; }

        private static readonly Random random = Random.Shared;

        public Dice(int dice1, int dice2, int countOfDubles)
        {
            Dice1 = dice1;
            Dice2 = dice2;
            CountOfDubles = countOfDubles;
        }
        public Dice() { }

        internal void Roll()
        {
            Dice1 = random.Next(1, 7);
            Dice2 = random.Next(1, 7);
            if (Double)
                CountOfDubles++;
            else
                CountOfDubles = 0;
        }

        internal void SetCountOfDubles(int countOfDubles)
        {
            if (countOfDubles < 0)
                throw new Exception("Вкажіть невід'ємне число");
            CountOfDubles = countOfDubles;
        }
    }
}
