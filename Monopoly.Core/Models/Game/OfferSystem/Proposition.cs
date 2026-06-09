namespace Monopoly.Core.Models.Game.OfferSystem
{
    public class Proposition
    {
        public Guid Id { get; private set; }

        public int Money { get; private set; }
        public List<int> CellNumbers { get; private set; }

        public Proposition(Guid id, int money, List<int> cellNumbers)
        {
            Id = id;
            Money = money;
            CellNumbers = cellNumbers;
        }
        public Proposition() { }

        public void SetMoney(int money)
        {
            if (money < 0)
                throw new Exception("Сума грошей в пропозиції не може бути від'ємною");
            Money = money;
        }
        public void SetCellNumbers(List<int> cellNumbers)
        {
            if (cellNumbers.Count > 0 && cellNumbers.Any(cellNumber => cellNumber < 0))
                throw new Exception("Номера клітин в пропозиції не можуть бути менше 0");
            CellNumbers = cellNumbers;
        }
    }
}