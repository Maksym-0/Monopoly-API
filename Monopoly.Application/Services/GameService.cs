using System.Net;
using Monopoly.Core;
using Monopoly.Core.Interfaces.Services;
using Monopoly.Core.Models.Game;
using Monopoly.Core.DTO.Games;
using Monopoly.Core.Models.Service;
using Monopoly.Core.Models.Abstractions.Interfaces;
using Monopoly.Core.Models.Abstractions.Classes;
using Monopoly.Core.Models.Room;
using Monopoly.Core.Interfaces.UnitsOfWork;

namespace Monopoly.Application.Services
{
    public class GameService : IGameService
    {
        private readonly IUnitOfWork _unitOfWork;
        public GameService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResponse<GameStateDto>> StatsOfGameAsync(Guid gameId)
        {
            Game? game = await _unitOfWork.Games.GetByIdAsync(gameId);
            if (game == null)
                return new ServiceResponse<GameStateDto>()
                {
                    Success = false,
                    Message = "Гру не знайдено",
                    StatusCode = HttpStatusCode.BadRequest,
                    Data = null
                };
            GameStateDto gameStatus = new GameStateDto(game);

            return new ServiceResponse<GameStateDto>()
            {
                Success = true,
                Message = "Гру знайдено",
                StatusCode = HttpStatusCode.OK,
                Data = gameStatus
            };
        }
        public async Task<ServiceResponse<MoveDto>> MoveAsync(Guid gameId, Guid accountId)
        {
            Game? game = await _unitOfWork.Games.GetByIdAsync(gameId);
            
            ServiceResponse<MoveDto> response = ValidateMove(game, accountId, 
                out Player? player);
            if (!response.Success)
                return response;

            bool startedAsPrisoner = player.IsPrisoner;

            player.RollDices();

            if (!player.IsPrisoner)
                player.Move(game.Board.Cells.Count);

            game.TurnState.DisableCanRollDices();

            DoubleResultHandler(player, game.TurnState);
            
            string cellMessage;
            Cell cell = game.Board.Cells.First(c => c.Number == player.Location);
            
            if (startedAsPrisoner && !player.IsPrisoner)
                cellMessage = $"{player.Name} кинув дубль та покинув в'язницю";
            else
                cellMessage = cell.ApplyEffect(player);

            await _unitOfWork.SaveChangesAsync();

            MoveDto dto = new MoveDto()
            {
                Player = new PlayerDto(player),
                Cell = new CellDto(cell),
                CellMessage = cellMessage
            };

            response.Message = "Рух завершено";
            response.Data = dto;

            return response;
        }
        public async Task<ServiceResponse<PayDto>> PayRentAsync(Guid gameId, Guid accountId)
        {
            Game? game = await _unitOfWork.Games.GetByIdAsync(gameId);

            ServiceResponse<PayDto> response = ValidatePayRent(game, accountId, 
                out Player? player, out IMoneyEater? moneyEater);
            if (!response.Success)
                return response;

            PayDto payResponse = new PayDto()
            {
                PlayerId = player.Id,
                PlayerName = player.Name
            };
            
            if (moneyEater is IOwnable ownableCell)
            {
                if (ownableCell.Owner == null)
                    throw new Exception("Клітина немає власника");

                payResponse.ReceiverId = ownableCell.OwnerId;
                payResponse.ReceiverName = ownableCell.Owner.Name;

                Player receiver = ownableCell.Owner;

                player.ApplyToBalance(-moneyEater.Rent);

                receiver.ApplyToBalance(moneyEater.Rent);
                    
                payResponse.Amount = moneyEater.Rent;

                payResponse.NewPlayerBalance = player.Balance;
                payResponse.NewReceiverBalance = receiver.Balance;
            }
            else
            {
                player.ApplyToBalance(-moneyEater.Rent);

                payResponse.Amount = moneyEater.Rent;
                payResponse.NewPlayerBalance = player.Balance;
            }

            game.TurnState.CompletePayment();

            await _unitOfWork.SaveChangesAsync();

            response.Message = "Рахунки сплачено";
            response.Data = payResponse;

            return response;
        }
        public async Task<ServiceResponse<PayDto>> PayToLeavePrisonAsync(Guid gameId, Guid accountId)
        {
            Game? game = await _unitOfWork.Games.GetByIdAsync(gameId);

            ServiceResponse<PayDto> response = ValidatePayToLeavePrison(game, accountId, 
                out Player? player, out TurnState? turnState);
            if (!response.Success)
                return response;

            bool result = player.PayToLeavePrison(Constants.LeavePrisonCost);

            if (!result)
                return new ServiceResponse<PayDto>(false, "Помилка при сплаті. Недостатньо грошей на рахунку", HttpStatusCode.BadRequest, null);
            
            player.LeavePrison();

            await _unitOfWork.SaveChangesAsync();

            PayDto payResponse = new PayDto()
            {
                PlayerId = player.Id,
                PlayerName = player.Name,

                Amount = Constants.LeavePrisonCost,

                IsPrisonPay = true,

                NewPlayerBalance = player.Balance,
            };
            
            response.Message = "Гравець успішно вийшов із в'язниці";
            response.Data = payResponse;

            return response;
        }
        public async Task<ServiceResponse<BuyDto>> BuyCellAsync(Guid gameId, Guid accountId)
        {
            Game? game = await _unitOfWork.Games.GetByIdAsync(gameId);

            ServiceResponse<BuyDto> response = ValidateBuy(game, accountId, 
                out Player? player, out Cell? cell, out IOwnable? ownableCell);
            if (!response.Success)
                return response;

            int oldBalance = player.Balance;
            player.ApplyToBalance(-ownableCell.Price);
            ownableCell.ChangeOwner(player);

            BuyDto buyDto = new BuyDto()
            {
                PlayerId = player.Id,
                PlayerName = player.Name,
                OldBalance = oldBalance,
                NewBalance = player.Balance,

                CellNumber = cell.Number,
                CellName = cell.Name,
                Price = ownableCell.Price
            };

            if (ownableCell is IMonopolistic monopolisticCell)
            {
                bool result = monopolisticCell.Monopoly.CheckAndApplyMonopoly();
                buyDto.CellMonopolyType = monopolisticCell.Monopoly.Type;
                buyDto.HasMonopoly = result;
            }

            await _unitOfWork.SaveChangesAsync();

            response.Message = "Клітину придбано";
            response.Data = buyDto;

            return response;
        }
        public async Task<ServiceResponse<LevelChangeDto>> LevelUpAsync(Guid gameId, Guid accountId, int cellNumber)
        {
            Game? game = await _unitOfWork.Games.GetByIdAsync(gameId);

            ServiceResponse<LevelChangeDto> response = ValidateLevelUp(game, accountId, cellNumber, 
                out Player? player, out TurnState? turnState, out Cell? cell, out IUpgradable? upgradableCell);
            if (!response.Success)
                return response;

            int oldBalance = player.Balance;
            player.LevelUpCell(upgradableCell);

            turnState.ForbidLevelUp();

            await _unitOfWork.SaveChangesAsync();

            LevelChangeDto levelChangeDto = new LevelChangeDto()
            {
                PlayerId = player.Id,
                PlayerName = player.Name,

                CellNumber = cell.Number,
                CellName = cell.Name,

                NewLevel = upgradableCell.Level,
                OldLevel = upgradableCell.Level - 1,

                OldPlayerBalance = oldBalance,
                NewPlayerBalance = player.Balance
            };

            response.Message = "Збільшення рівню клітини завершено";
            response.Data = levelChangeDto;

            return response;
        }
        public async Task<ServiceResponse<LevelChangeDto>> LevelDownAsync(Guid gameId, Guid accountId, int cellNumber)
        {
            Game? game = await _unitOfWork.Games.GetByIdAsync(gameId);

            ServiceResponse<LevelChangeDto> response = ValidateLevelDown(game, accountId, cellNumber,
                out Player? player, out TurnState? turnState, out Cell? cell, out IUpgradable? upgradableCell);
            if (!response.Success)
                return response;

            int oldBalance = player.Balance;
            player.LevelDownCell(upgradableCell);

            await _unitOfWork.SaveChangesAsync();

            LevelChangeDto levelChangeDto = new LevelChangeDto()
            {
                PlayerId = player.Id,
                PlayerName = player.Name,

                CellNumber = cell.Number,
                CellName = cell.Name,

                NewLevel = upgradableCell.Level,
                OldLevel = upgradableCell.Level + 1,

                OldPlayerBalance = oldBalance,
                NewPlayerBalance = player.Balance
            };

            response.Message = "Зменшення рівня завершено";
            response.Data = levelChangeDto;

            return response;
        }
        public async Task<ServiceResponse<NextActionDto>> EndActionAsync(Guid gameId, Guid accountId)
        {
            Game? game = await _unitOfWork.Games.GetByIdAsync(gameId);

            ServiceResponse<NextActionDto> response = ValidateEndAction(game, accountId, 
                out Player? player, out TurnState turnState);
            if (!response.Success)
                return response;

            Player nextPlayer = turnState.NextTurn();

            await _unitOfWork.SaveChangesAsync();

            NextActionDto nextActionDto = new NextActionDto()
            {
                PlayerId = player.Id,
                PlayerName = player.Name,

                NewPlayerId = nextPlayer.Id,
                NewPlayerName = nextPlayer.Name
            };

            response.Message = "Хід завершено";
            response.Data = nextActionDto;

            return response;
        }
        public async Task<ServiceResponse<LeaveGameDto>> LeaveGameAsync(Guid gameId, Guid accountId)
        {
            Game? game = await _unitOfWork.Games.GetByIdAsync(gameId);

            ServiceResponse<LeaveGameDto> response = ValidateLeave(game, accountId, 
                out Player? player, out TurnState? turnState);
            if (!response.Success)
                return response;

            Room? room = await _unitOfWork.Rooms.GetRoomByIdAsync(game.RoomId);
            if (room != null)
            {
                room.RemovePlayerByAccountId(accountId);
            }

            game.LeaveGame(player.Id);

            Player? winner = game.CheckWinner();

            LeaveGameDto leaveGameDto = new LeaveGameDto()
            {
                PlayerId = player.Id,
                PlayerName = player.Name,
                
                RemainingPlayers = room?.CountOfPlayers ?? 0,
                Winner = null,

                IsGameOver = winner != null
            };

            response.Data = leaveGameDto;

            if (winner != null)
            {
                response.Message = $"Гравець покинув гру. Гру завершено. Переможець: {winner.Name}";
                
                await _unitOfWork.Rooms.DeleteByIdAsync(game.RoomId);

                leaveGameDto.Winner = new PlayerDto(winner);
            }
            else
            {
                response.Message = "Гравець покинув гру";
            }

            await _unitOfWork.SaveChangesAsync();

            return response;
        }

        private ServiceResponse<MoveDto> ValidateMove(Game? game, Guid accountId, 
            out Player? player)
        {
            player = null;
            
            if (game == null)
                return new ServiceResponse<MoveDto>(false, "Гру не знайдено", HttpStatusCode.NotFound, null);

            Player? foundPlayer = game.Players.FirstOrDefault(p => p.AccountId == accountId);
            player = foundPlayer;

            if (foundPlayer == null)
                return new ServiceResponse<MoveDto>(false, "Гравця не знайдено", HttpStatusCode.NotFound, null);
            
            if (!foundPlayer.InGame)
                return new ServiceResponse<MoveDto>(false, "Гравець поза грою", HttpStatusCode.BadRequest, null);
            
            if (game.TurnState.CurrentPlayerIndex != foundPlayer.Index)
                return new ServiceResponse<MoveDto>(false, "Неможливо виконати дію не в свій хід", HttpStatusCode.BadRequest, null);
            
            if (!game.TurnState.CanRollDices)
                return new ServiceResponse<MoveDto>(false, "Неможливо кинути кубики. Гравець вже кидав кубики", HttpStatusCode.BadRequest, null);

            if (game.TurnState.NeedPay)
                return new ServiceResponse<MoveDto>(false, "Неможливо кинути кубики. Гравець повинен сплатити рахунки", HttpStatusCode.BadRequest, null);

            return new ServiceResponse<MoveDto>(true, "Валідація успішна", HttpStatusCode.OK, null); ;
            
        }
        private ServiceResponse<PayDto> ValidatePayRent(Game? game, Guid accountId, 
            out Player? player, out IMoneyEater? moneyEater)
        {
            player = null;
            moneyEater = null;

            if (game == null)
                return new ServiceResponse<PayDto>(false, "Гру не знайдено", HttpStatusCode.NotFound, null);

            Player? foundPlayer = game.Players.FirstOrDefault(p => p.AccountId == accountId);
            player = foundPlayer;

            if (foundPlayer == null)
                return new ServiceResponse<PayDto>(false, "Гравця не знайдено", HttpStatusCode.NotFound, null);

            if (!foundPlayer.InGame)
                return new ServiceResponse<PayDto>(false, "Гравець поза грою", HttpStatusCode.BadRequest, null);
            
            if (game.TurnState.CurrentPlayerIndex != foundPlayer.Index)
                return new ServiceResponse<PayDto>(false, "Неможливо виконати дію не в свій хід", HttpStatusCode.BadRequest, null);
            
            if (!game.TurnState.NeedPay)
                return new ServiceResponse<PayDto>(false, "Гравець не повинен платити", HttpStatusCode.BadRequest, null);

            Cell? foundCell = game.Board.Cells.FirstOrDefault(c => c.Number == foundPlayer.Location);

            if (foundCell == null)
                return new ServiceResponse<PayDto>(false, "Клітину не знайдено", HttpStatusCode.NotFound, null);
            
            if (foundCell is not IMoneyEater foundMoneyEater)
                return new ServiceResponse<PayDto>(false, "Клітина не потребує сплати рахунків", HttpStatusCode.BadRequest, null);

            moneyEater = foundMoneyEater;

            if (!foundPlayer.IsPrisoner && foundPlayer.Balance < moneyEater.Rent)
                return new ServiceResponse<PayDto>(false, "Недостатньо коштів для сплати ренти", HttpStatusCode.BadRequest, null);

            return new ServiceResponse<PayDto>(true, "Валідація успішна", HttpStatusCode.OK, null);
        }
        private ServiceResponse<PayDto> ValidatePayToLeavePrison(Game? game, Guid accountId, 
            out Player? player, out TurnState? turnState)
        {
            player = null;
            turnState = null;

            if (game == null)
                return new ServiceResponse<PayDto>(false, "Гру не знайдено", HttpStatusCode.NotFound, null);

            player = game.Players.FirstOrDefault(p => p.AccountId == accountId);
            
            if (player == null)
                return new ServiceResponse<PayDto>(false, "Гравця не знайдено", HttpStatusCode.NotFound, null);

            turnState = game.TurnState;

            if (turnState == null)
                return new ServiceResponse<PayDto>(false, "Статус ходу не знайдено", HttpStatusCode.NotFound, null);

            if (!player.InGame)
                return new ServiceResponse<PayDto>(false, "Гравець поза грою", HttpStatusCode.BadRequest, null);

            if (!player.IsPrisoner)
                return new ServiceResponse<PayDto>(false, "Гравець не перебуває у в'язниці", HttpStatusCode.BadRequest, null);

            if (turnState.CurrentPlayerIndex != player.Index)
                return new ServiceResponse<PayDto>(false, "Неможливо виконати дію не в свій хід", HttpStatusCode.BadRequest, null);

            if (player.IsPrisoner && player.Balance < Constants.LeavePrisonCost)
                return new ServiceResponse<PayDto>(false, "Недостатньо коштів для виходу з в'язниці", HttpStatusCode.BadRequest, null);

            return new ServiceResponse<PayDto>(true, "Валідація успішна", HttpStatusCode.OK, null);
        }
        private ServiceResponse<BuyDto> ValidateBuy(Game? game, Guid accountId, 
            out Player? player, out Cell? cell, out IOwnable? ownableCell)
        {
            player = null;
            cell = null;
            ownableCell = null;

            if (game == null)
                return new ServiceResponse<BuyDto>(false, "Гру не знайдено", HttpStatusCode.NotFound, null);

            Player? foundPlayer = game.Players.FirstOrDefault(p => p.AccountId == accountId);
            player = foundPlayer;

            if (foundPlayer == null)
                return new ServiceResponse<BuyDto>(false, "Гравця не знайдено", HttpStatusCode.NotFound, null);

            if (!foundPlayer.InGame)
                return new ServiceResponse<BuyDto>(false, "Гравець поза грою", HttpStatusCode.BadRequest, null);

            if (game.TurnState.CurrentPlayerIndex != foundPlayer.Index)
                return new ServiceResponse<BuyDto>(false, "Неможливо придбати клітину не в свій хід", HttpStatusCode.BadRequest, null);

            if (!game.TurnState.CanBuyCell)
                return new ServiceResponse<BuyDto>(false, "Гравець зараз не може придбати клітину", HttpStatusCode.BadRequest, null);

            Cell? foundCell = game.Board.Cells.FirstOrDefault(c => c.Number == foundPlayer.Location);
            cell = foundCell;

            if (foundCell == null)
                return new ServiceResponse<BuyDto>(false, "Клітину не знайдено", HttpStatusCode.NotFound, null);

            if (foundCell.Special)
                return new ServiceResponse<BuyDto>(false, "Неможливо придбати особливу клітину", HttpStatusCode.BadRequest, null);

            if (foundCell is not IOwnable foundOwnableCell)
                return new ServiceResponse<BuyDto>(false, "Клітина не продається", HttpStatusCode.BadRequest, null);

            ownableCell = foundOwnableCell;

            if (ownableCell.OwnerId != null)
                return new ServiceResponse<BuyDto>(false, "Неможливо придбати клітину, що належить іншому гравцю", HttpStatusCode.BadRequest, null);
            
            if (foundPlayer.Balance < ownableCell.Price)
                return new ServiceResponse<BuyDto>(false, "Неможливо придбати клітину. Недостатньо коштів", HttpStatusCode.BadRequest, null);
            
            return new ServiceResponse<BuyDto>(true, "Валідація успішна", HttpStatusCode.OK, null);
        }
        private ServiceResponse<LevelChangeDto> ValidateLevelUp(Game? game, Guid accountId, int cellNumber,
            out Player? player, out TurnState? turnState, out Cell? cell, out IUpgradable? upgradableCell)
        {
            player = null;
            turnState = null;
            cell = null;
            upgradableCell = null;

            if (game == null)
                return new ServiceResponse<LevelChangeDto>(false, "Гру не знайдено", HttpStatusCode.NotFound, null);

            Player? foundPlayer = game.Players.FirstOrDefault(p => p.AccountId == accountId);

            if (foundPlayer == null)
                return new ServiceResponse<LevelChangeDto>(false, "Гравця не знайдено", HttpStatusCode.NotFound, null);
            player = foundPlayer;

            Cell? foundCell = game.Board.Cells.FirstOrDefault(c => c.Number == cellNumber);
            if (foundCell == null)
                return new ServiceResponse<LevelChangeDto>(false, "Клітину не знайдено", HttpStatusCode.NotFound, null);
            cell = foundCell;

            TurnState? foundTurnState = game.TurnState;

            if (foundTurnState == null)
                return new ServiceResponse<LevelChangeDto>(false, "Статус гри не знайдено", HttpStatusCode.NotFound, null);
            turnState = foundTurnState;

            if (turnState.CurrentPlayerIndex != player.Index)
                return new ServiceResponse<LevelChangeDto>(false, "Неможливо виконати дію не в свій хід", HttpStatusCode.BadRequest, null);

            if (!turnState.CanLevelUpCell)
                return new ServiceResponse<LevelChangeDto>(false, "Гравець більше не може збільшувати рівень клітин", HttpStatusCode.BadRequest, null);

            if (foundCell is not IOwnable ownableCell)
                return new ServiceResponse<LevelChangeDto>(false, "Клітина нікому не належить", HttpStatusCode.BadRequest, null);

            if (ownableCell.OwnerId != player.Id)
                return new ServiceResponse<LevelChangeDto>(false, "Клітина не належить даному гравцю", HttpStatusCode.BadRequest, null);

            if (foundCell is not IUpgradable foundUpgradableCell)
                return new ServiceResponse<LevelChangeDto>(false, "Неможливо змінювати рівень даної клітини", HttpStatusCode.BadRequest, null);
            upgradableCell = foundUpgradableCell;

            if (foundUpgradableCell.Level == 5)
                return new ServiceResponse<LevelChangeDto>(false, "Досягнуто максимального рівня клітини", HttpStatusCode.BadRequest, null);

            if (foundUpgradableCell.UpgradeCost > player.Balance)
                return new ServiceResponse<LevelChangeDto>(false, "Неможливо збільшити рівень клітини. Недостатньо грошей", HttpStatusCode.BadRequest, null);

            return new ServiceResponse<LevelChangeDto>(true, "Валідація успішна", HttpStatusCode.OK, null);
        }
        private ServiceResponse<LevelChangeDto> ValidateLevelDown(Game? game, Guid accountId, int cellNumber,
            out Player? player, out TurnState? turnState, out Cell? cell, out IUpgradable? upgradableCell)
        {
            player = null;
            turnState = null;
            cell = null;
            upgradableCell = null;

            if (game == null)
                return new ServiceResponse<LevelChangeDto>(false, "Гру не знайдено", HttpStatusCode.NotFound, null);

            Player? foundPlayer = game.Players.FirstOrDefault(p => p.AccountId == accountId);

            if (foundPlayer == null)
                return new ServiceResponse<LevelChangeDto>(false, "Гравця не знайдено", HttpStatusCode.NotFound, null);
            player = foundPlayer;
            
            Cell? foundCell = game.Board.Cells.FirstOrDefault(c => c.Number == cellNumber);

            if (foundCell == null)
                return new ServiceResponse<LevelChangeDto>(false, "Клітину не знайдено", HttpStatusCode.NotFound, null);
            cell = foundCell;

            TurnState? foundTurnState = game.TurnState;

            if (foundTurnState == null)
                return new ServiceResponse<LevelChangeDto>(false, "Статус гри не знайдено", HttpStatusCode.NotFound, null);
            turnState = foundTurnState;

            if (turnState.CurrentPlayerIndex != player.Index)
                return new ServiceResponse<LevelChangeDto>(false, "Неможливо виконати дію не в свій хід", HttpStatusCode.BadRequest, null);

            if (foundCell is not IOwnable ownableCell)
                return new ServiceResponse<LevelChangeDto>(false, "Клітина нікому не належить", HttpStatusCode.BadRequest, null);

            if (ownableCell.OwnerId != player.Id)
                return new ServiceResponse<LevelChangeDto>(false, "Клітина не належить даному гравцю", HttpStatusCode.BadRequest, null);

            if (foundCell is not IUpgradable foundUpgradableCell)
                return new ServiceResponse<LevelChangeDto>(false, "Неможливо змінювати рівень даної клітини", HttpStatusCode.BadRequest, null);
            upgradableCell = foundUpgradableCell;

            if (foundUpgradableCell.Level == 0)
                return new ServiceResponse<LevelChangeDto>(false, "Досягнуто мінімального рівня клітини", HttpStatusCode.BadRequest, null);

            return new ServiceResponse<LevelChangeDto>(true, "Валідація успішна", HttpStatusCode.OK, null);
        }
        private ServiceResponse<NextActionDto> ValidateEndAction(Game? game, Guid accountId,
            out Player? player, out TurnState? turnState)
        {
            player = null;
            turnState = null;

            if (game == null)
                return new ServiceResponse<NextActionDto>(false, "Гру не знайдено", HttpStatusCode.NotFound, null);

            player = game.Players.FirstOrDefault(p => p.AccountId == accountId);

            if (player == null)
                return new ServiceResponse<NextActionDto>(false, "Гравця не знайдено", HttpStatusCode.NotFound, null);

            turnState = game.TurnState;

            if (turnState == null)
                return new ServiceResponse<NextActionDto>(false, "Статус ходу не знайдено", HttpStatusCode.NotFound, null);

            if (!player.InGame)
                return new ServiceResponse<NextActionDto>(false, "Гравець поза грою", HttpStatusCode.BadRequest, null);

            if (turnState.CurrentPlayerIndex != player.Index)
                return new ServiceResponse<NextActionDto>(false, "Неможливо завершити свій хід до його початку", HttpStatusCode.BadRequest, null);

            if (turnState.CanRollDices)
                return new ServiceResponse<NextActionDto>(false, "Гравець не може завершити хід, не кинувши кубики", HttpStatusCode.BadRequest, null);

            if (turnState.NeedPay)
                return new ServiceResponse<NextActionDto>(false, "Гравець не може завершити хід, не оплатившив рахунки", HttpStatusCode.BadRequest, null);

            if (turnState.Game.CurrentTradeOffer != null)
                return new ServiceResponse<NextActionDto>(false, "Гравець не може завершити хід, поки існує активна торгова пропозиція", HttpStatusCode.BadRequest, null);

            return new ServiceResponse<NextActionDto>(true, "Валідація успішна", HttpStatusCode.OK, null);
        }
        private ServiceResponse<LeaveGameDto> ValidateLeave(Game? game, Guid accountId, 
            out Player? player, out TurnState? turnState)
        {
            player = null;
            turnState = null;

            if (game == null)
                return new ServiceResponse<LeaveGameDto>(false, "Гру не знайдено", HttpStatusCode.NotFound, null);

            Player? foundPlayer = game.Players.FirstOrDefault(p => p.AccountId == accountId);

            if (foundPlayer == null)
                return new ServiceResponse<LeaveGameDto>(false, "Гравця не знайдено", HttpStatusCode.NotFound, null);

            player = foundPlayer;

            if (game.TurnState == null)
                return new ServiceResponse<LeaveGameDto>(false, "Статус ходу не знайдено", HttpStatusCode.NotFound, null);

            turnState = game.TurnState;

            if (!player.InGame)
                return new ServiceResponse<LeaveGameDto>(false, "Гравець вже поза грою", HttpStatusCode.BadRequest, null);
            
            return new ServiceResponse<LeaveGameDto>(true, "Валідація успішна", HttpStatusCode.OK, null);
        }

        private void DoubleResultHandler(Player player, TurnState turnState)
        {
            if (!player.Dices.Double)
                return;
            
            if (player.IsPrisoner)
            {
                player.LeavePrison();
                return;
            }
            
            if (player.Dices.CountOfDubles == 3)
            {
                player.Imprison();
                turnState.NextTurn();
                return;
            }

            turnState.EnableCanRollDices();
        }
    }
}
