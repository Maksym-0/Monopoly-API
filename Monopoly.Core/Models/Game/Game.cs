using Monopoly.Core.Models.Abstractions.Classes;
using Monopoly.Core.Models.Abstractions.Interfaces;
using Monopoly.Core.Models.Game.Cells;
using Monopoly.Core.Models.Game.OfferSystem;
using Monopoly.Core.Models.Room;

namespace Monopoly.Core.Models.Game
{
    public class Game
    {
        public Guid Id { get; private set; }

        public Guid RoomId { get; private set; }
        public Room.Room? Room { get; private set; }

        public GameBoard? Board { get; private set; }
        public List<Player> Players { get; private set; } = new();
        public TurnState? TurnState { get; private set; }
        public TradeOffer? CurrentTradeOffer { get; private set; }

        public Game(Guid gameId, Guid roomId, Room.Room room, GameBoard board, List<Player> players, TurnState turnState)
        {
            Id = gameId;

            RoomId = roomId;
            Room = room;

            Board = board;
            Players = players;

            TurnState = turnState;
        }
        public Game() { }

        public void StartGame()
        {
            if (Players.Count < 2)
                throw new Exception("Недостатньо гравців для початку гри");

            TurnState.SetCurrentPlayerIndex(1);
            TurnState.StartTurn();
        }

        public void CreateTradeOffer(TradeOffer tradeOffer)
        {
            if (CurrentTradeOffer != null)
                throw new Exception("Вже є активна пропозиція торгівлі. Прийміть або скасуйте її перед створенням нової пропозиції");

            bool hasInvalidOffererCellNumbers = tradeOffer.OffererProposition.CellNumbers.Any(cellNumber => cellNumber < 0 || cellNumber >= Board.Cells.Count);
            bool hasInvalidOffereeCellNumbers = tradeOffer.OffereeProposition.CellNumbers.Any(cellNumber => cellNumber < 0 || cellNumber >= Board.Cells.Count);

            if (hasInvalidOffererCellNumbers || hasInvalidOffereeCellNumbers)
                throw new Exception("В пропозиції є номери клітин, що не існують на дошці");

            bool hasInvalidOffererCellOwners = tradeOffer.OffererProposition.CellNumbers.Any(cellNumber => Board.Cells.FirstOrDefault(c => c.Number == cellNumber) is IOwnable ownableCell && ownableCell.OwnerId != tradeOffer.OffererId);
            bool hasInvalidOffereeCellOwners = tradeOffer.OffereeProposition.CellNumbers.Any(cellNumber => Board.Cells.FirstOrDefault(c => c.Number == cellNumber) is IOwnable ownableCell && ownableCell.OwnerId != tradeOffer.OffereeId);

            if (hasInvalidOffererCellOwners || hasInvalidOffereeCellOwners)
                throw new Exception("В пропозиції є номери клітин, які не належать відповідним гравцям");

            bool hasDuplicateOffererCellNumbers = tradeOffer.OffererProposition.CellNumbers.GroupBy(cellNumber => cellNumber).Any(group => group.Count() > 1);
            bool hasDuplicateOffereeCellNumbers = tradeOffer.OffereeProposition.CellNumbers.GroupBy(cellNumber => cellNumber).Any(group => group.Count() > 1);

            if (hasDuplicateOffererCellNumbers || hasDuplicateOffereeCellNumbers)
                throw new Exception("В пропозиції є повторювані номери клітин в пропозиції гравця, що створює пропозицію");

            List<int> allTradedCellNumberss = tradeOffer.OffererProposition.CellNumbers
                .Concat(tradeOffer.OffereeProposition.CellNumbers)
                .ToList();

            List<Cell> tradedCells = allTradedCellNumberss
                .Select(cellNumber => Board.Cells.FirstOrDefault(c => c.Number == cellNumber))
                .OfType<Cell>()
                .ToList();

            foreach (Cell cell in tradedCells)
            {
                if (cell is not IOwnable)
                    throw new Exception("В пропозиції є номери клітин, які не є власністю жодного з гравців");

                if (cell is IOwnable ownableCell && ownableCell.OwnerId != tradeOffer.OffererId && ownableCell.OwnerId != tradeOffer.OffereeId)
                {
                    throw new Exception("В пропозиції є номери клітин, які не належать відповідним гравцям");
                }
                
                if (cell is IMonopolistic monopolisticCell && cell is IUpgradable upgradableCell)
                {
                    if (monopolisticCell.Monopoly != null && monopolisticCell.Monopoly.Cells.Any(mCell => mCell is IUpgradable mUpgradableCell && mUpgradableCell.Level > 0))
                    {
                        throw new Exception("В пропозиції є номери клітин, які є частиною монополії з клітинами з піднятим рівнем. Монополія не може бути розірвана в рамках однієї пропозиції торгівлі, якщо її клітини мають збільшений рівень. Зменшіть рівень усіх клітин монополії перед торгівлею");
                    }
                }
            }

            if (tradeOffer.OffererProposition.Money > tradeOffer.Offerer.Balance)
                throw new Exception("У Вас недостатньо грошей для цієї пропозиції");

            if (tradeOffer.OffereeProposition.Money > tradeOffer.Offeree.Balance)
                throw new Exception("У гравця, з яким Ви торгуєтесь, недостатньо грошей");

            if (tradeOffer.Offerer.Index != TurnState.CurrentPlayerIndex)
                throw new Exception("Неможливо створити пропозицію торгівлі не в свій хід");

            CurrentTradeOffer = tradeOffer;
        }
        public void AcceptTradeOffer(Guid playerId)
        {
            if (CurrentTradeOffer == null)
                throw new Exception("Немає активної пропозиції торгівлі");

            if (CurrentTradeOffer.OffereeId != playerId)
                throw new Exception("Тільки гравець, якому створена пропозиція, може її прийняти");

            Player offerer = CurrentTradeOffer.Offerer;
            Player offeree = CurrentTradeOffer.Offeree;

            if (offerer.Balance < CurrentTradeOffer.OffererProposition.Money || offeree.Balance < CurrentTradeOffer.OffereeProposition.Money)
            {
                CurrentTradeOffer = null;
                throw new Exception("У одного з гравців недостатньо грошей для цієї пропозиціїю Угоду скасовано");
            }

            offerer.ApplyToBalance(-CurrentTradeOffer.OffererProposition.Money);
            offeree.ApplyToBalance(CurrentTradeOffer.OffererProposition.Money);

            offeree.ApplyToBalance(-CurrentTradeOffer.OffereeProposition.Money);
            offerer.ApplyToBalance(CurrentTradeOffer.OffereeProposition.Money);

            List<IOwnable> offererCellsToTransfer = CurrentTradeOffer.OffererProposition.CellNumbers.Select(cellNumber => Board.Cells.FirstOrDefault(c => c.Number == cellNumber)).OfType<IOwnable>().ToList();
            foreach(var cell in offererCellsToTransfer)
            {
                cell.ChangeOwner(offeree);
                CompanyCell companyCell = (CompanyCell)cell;
                offerer.OwnedCells.Remove(companyCell);
                offeree.OwnedCells.Add(companyCell);
                if (cell is IMonopolistic monopolisticCell)
                    monopolisticCell.Monopoly.CheckAndApplyMonopoly();
            }

            List<IOwnable> offereCellsToTransfer = CurrentTradeOffer.OffereeProposition.CellNumbers.Select(cellNumber => Board.Cells.FirstOrDefault(c => c.Number == cellNumber)).OfType<IOwnable>().ToList();
            foreach(var cell in offereCellsToTransfer)
            {
                cell.ChangeOwner(offerer);
                CompanyCell companyCell = (CompanyCell)cell;
                offeree.OwnedCells.Remove(companyCell);
                offerer.OwnedCells.Add(companyCell);
                if (cell is IMonopolistic monopolisticCell)
                    monopolisticCell.Monopoly.CheckAndApplyMonopoly();
            }

            CurrentTradeOffer = null;
        }
        public void CancelTradeOffer(Guid playerId)
        {
            if (CurrentTradeOffer == null)
                throw new Exception("Немає активної пропозиції торгівлі");
            
            if (CurrentTradeOffer.OffererId != playerId && CurrentTradeOffer.OffereeId != playerId)
                throw new Exception("Неможливо скасувати пропозицію торгівлі, створену іншим гравцем");

            CurrentTradeOffer = null;
        }

        public Player? CheckWinner()
        {
            int playersInGame = 0;
            Player? winner = null;

            foreach (var player in Players)
            {
                if (player.InGame)
                {
                    playersInGame++;
                    winner = player;
                }
            }

            if (playersInGame == 1)
            {
                return winner;
            }
            else
                return null;
        }
        public void LeaveGame(Guid playerId)
        {
            Player? player = Players.FirstOrDefault(p => p.Id == playerId);
            if (player == null)
                throw new Exception("Граввець із цим Id не перебуває в грі");
            else if (!player.InGame)
                throw new Exception("Гравець вже покинув гру");

            if (player.Index == TurnState.CurrentPlayerIndex)
            {
                TurnState.NextTurn();
            }

            if (CurrentTradeOffer != null && (CurrentTradeOffer.OffererId == playerId || CurrentTradeOffer.OffereeId == playerId))
            {
                CurrentTradeOffer = null;
            }

            Board.ResetOwnerCells(player.Id);
            player.LeaveGame();
        }
    }
}
