using Monopoly.Core.DTO.Games;
using Monopoly.Core.Interfaces.Services;
using Monopoly.Core.Interfaces.UnitsOfWork;
using Monopoly.Core.Models.Abstractions.Classes;
using Monopoly.Core.Models.ApiRequest;
using Monopoly.Core.Models.Game;
using Monopoly.Core.Models.Game.OfferSystem;
using Monopoly.Core.Models.Service;
using System.Net;

namespace Monopoly.Application.Services
{
    public class TradingService : ITradingService
    {
        IUnitOfWork _unitOfWork { get; set; }

        public TradingService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResponse<TradeOfferDto>> GetCurrentTradeOfferAsync(Guid gameId)
        {
            Game? game = await _unitOfWork.Games.GetByIdAsync(gameId);

            ServiceResponse<TradeOfferDto> response = ValidateGetCurrentTradeOffer(game);
            if (!response.Success)
                return response;

            response.Message = "Поточну пропозицію обміну знайдено";
            response.Data = new TradeOfferDto(game.CurrentTradeOffer);

            return response;
        }
        public async Task<ServiceResponse<TradeOfferDto>> CreateTradeOfferAsync(Guid gameId, Guid accountId, CreateTradeOfferRequest request)
        {
            Game? game = await _unitOfWork.Games.GetByIdAsync(gameId);

            ServiceResponse<TradeOfferDto> response = ValidateCreateTradeOffer(game, request, accountId,
                out Player? offerer, out Player? offeree);
            if (!response.Success)
                return response;

            Proposition offererProposition = new Proposition(Guid.NewGuid(), request.OffererMoneyProposition, request.OffererCellNumbers);
            Proposition offereeProposition = new Proposition(Guid.NewGuid(), request.OffereeMoneyProposition, request.OffereeCellNumbers);

            TradeOffer tradeOffer = new TradeOffer(Guid.NewGuid(), game, offerer, offererProposition, offeree, offereeProposition);

            game.CreateTradeOffer(tradeOffer);

            response.Message = "Пропозицію обміну успішно створено";
            response.Data = new TradeOfferDto(tradeOffer);

            await _unitOfWork.Games.AddTradeOfferAsync(tradeOffer);

            await _unitOfWork.SaveChangesAsync();

            return response;
        }
        public async Task<ServiceResponse<AcceptTradeDto>> AcceptTradeOfferAsync(Guid gameId, Guid accountId)
        {
            Game? game = await _unitOfWork.Games.GetByIdAsync(gameId);

            ServiceResponse<AcceptTradeDto> response = ValidateAcceptTradeOffer(game, accountId,
                out Player? offerer, out Player? offeree);
            if (!response.Success)
                return response;

            TradeOffer tradeOffer = game.CurrentTradeOffer;

            game.AcceptTradeOffer(offeree.Id);

            response.Message = "Пропозицію обміну прийнято";
            response.Data = new AcceptTradeDto(tradeOffer, offerer.Balance, offeree.Balance);

            await _unitOfWork.SaveChangesAsync();

            return response;
        }
        public async Task<ServiceResponse<CancelTradeDto>> CancelTradeOfferAsync(Guid gameId, Guid accountId)
        {
            Game? game = await _unitOfWork.Games.GetByIdAsync(gameId);

            ServiceResponse<CancelTradeDto> response = ValidateCancelTradeOffer(game, accountId, out Player? canceler);
            if (!response.Success)
                return response;

            game.CancelTradeOffer(canceler.Id);

            response.Message = "Пропозицію обміну скасовано";
            response.Data = new CancelTradeDto(canceler.Id, canceler.Name);

            await _unitOfWork.SaveChangesAsync();

            return response;
        }

        private ServiceResponse<TradeOfferDto> ValidateGetCurrentTradeOffer(Game? game)
        {
            if (game == null)
                return new ServiceResponse<TradeOfferDto>(false, "Гру не знайдено", HttpStatusCode.NotFound, null);

            if (game.CurrentTradeOffer == null)
                return new ServiceResponse<TradeOfferDto>(false, "Немає активної пропозиції обміну", HttpStatusCode.NotFound, null);

            return new ServiceResponse<TradeOfferDto>(true, "Валідація успішна", HttpStatusCode.OK, null);
        }
        private ServiceResponse<TradeOfferDto> ValidateCreateTradeOffer(Game? game, CreateTradeOfferRequest request, Guid offererAccountId, 
            out Player? offerer, out Player? offeree)
        {
            offerer = null;
            offeree = null;

            if (game == null)
                return new ServiceResponse<TradeOfferDto>(false, "Гру не знайдено", HttpStatusCode.NotFound, null);

            if (game.CurrentTradeOffer != null)
                return new ServiceResponse<TradeOfferDto>(false, "Неможливо створити другу пропозицію обміну, поки поточну не прийнято чи скасовано", HttpStatusCode.NotFound, null);

            offerer = game.Players.FirstOrDefault(p => p.AccountId == offererAccountId);

            if (offerer == null)
                return new ServiceResponse<TradeOfferDto>(false, "Гравця, який робить пропозицію обміну, не знайдено", HttpStatusCode.NotFound, null);

            if (game.TurnState.CurrentPlayerIndex != offerer.Index)
                return new ServiceResponse<TradeOfferDto>(false, "Неможливо створити пропозицію торгівлі поза своїм ходом", HttpStatusCode.BadRequest, null);

            offeree = game.Players.FirstOrDefault(p => p.Id == request.OffereePlayerId);

            if (offeree == null)
                return new ServiceResponse<TradeOfferDto>(false, "Гравця, який отримує пропозицію обміну, не знайдено", HttpStatusCode.NotFound, null);

            if (offerer.Id == offeree.Id)
                return new ServiceResponse<TradeOfferDto>(false, "Неможливо створити пропозицію обміну самому собі", HttpStatusCode.BadRequest, null);

            if (offerer.Balance < request.OffererMoneyProposition)
                return new ServiceResponse<TradeOfferDto>(false, "Гравцю, який робить пропозицію обміну, не вистачає коштів", HttpStatusCode.BadRequest, null);

            if (offeree.Balance < request.OffereeMoneyProposition)
                return new ServiceResponse<TradeOfferDto>(false, "Гравцю, який отримує пропозицію обміну, не вистачає коштів", HttpStatusCode.BadRequest, null);

            if (request.OffererCellNumbers != null)
            {
                foreach (int cellNumber in request.OffererCellNumbers)
                {
                    if (!offerer.OwnedCells.Any(c => c.Number == cellNumber))
                    {
                        return new ServiceResponse<TradeOfferDto>(false, "Ви намагаєтесь віддати клітину, якою не володієте", HttpStatusCode.BadRequest, null);
                    }
                }
            }

            if (request.OffereeCellNumbers != null)
            {
                foreach (int cellNumber in request.OffereeCellNumbers)
                {
                    if (!offeree.OwnedCells.Any(c => c.Number == cellNumber))
                    {
                        return new ServiceResponse<TradeOfferDto>(false, "Ви просите клітину, якою інший гравець не володіє", HttpStatusCode.BadRequest, null);
                    }
                }
            }

            return new ServiceResponse<TradeOfferDto>(true, "Валідація успішна", HttpStatusCode.OK, null);
        }
        private ServiceResponse<AcceptTradeDto> ValidateAcceptTradeOffer(Game? game, Guid offereeAccountId,
            out Player? offerer, out Player? offeree)
        {
            offerer = null;
            offeree = null;

            if (game == null)
                return new ServiceResponse<AcceptTradeDto>(false, "Гру не знайдено", HttpStatusCode.NotFound, null);

            if (game.CurrentTradeOffer == null)
                return new ServiceResponse<AcceptTradeDto>(false, "Немає активної пропозиції обміну", HttpStatusCode.NotFound, null);

            offerer = game.Players.FirstOrDefault(p => p.Id == game.CurrentTradeOffer.OffererId);

            if (offerer == null)
                return new ServiceResponse<AcceptTradeDto>(false, "Гравця, який робить пропозицію обміну, не знайдено", HttpStatusCode.NotFound, null);

            if (offerer.AccountId == offereeAccountId)
                return new ServiceResponse<AcceptTradeDto>(false, "Неможливо прийняти пропозицію обміну, якщо ви є її автором", HttpStatusCode.BadRequest, null);

            if (offerer.Balance < game.CurrentTradeOffer.OffererProposition.Money)
                return new ServiceResponse<AcceptTradeDto>(false, "Гравцю, який робить пропозицію обміну, не вистачає коштів", HttpStatusCode.BadRequest, null);

            offeree = game.Players.FirstOrDefault(p => p.Id == game.CurrentTradeOffer.OffereeId);

            if (offeree == null)
                return new ServiceResponse<AcceptTradeDto>(false, "Гравця, який отримує пропозицію обміну, не знайдено", HttpStatusCode.NotFound, null);

            if (offeree.AccountId != offereeAccountId)
                return new ServiceResponse<AcceptTradeDto>(false, "Неможливо прийняти пропозицію, яку Вам не пропонували", HttpStatusCode.BadRequest, null);

            if (offeree.Balance < game.CurrentTradeOffer.OffereeProposition.Money)
                return new ServiceResponse<AcceptTradeDto>(false, "Гравцю, який отримує пропозицію обміну, не вистачає коштів", HttpStatusCode.BadRequest, null);

            foreach (int cellNumber in game.CurrentTradeOffer.OffererProposition.CellNumbers)
            {
                if (!offerer.OwnedCells.Any(c => c.Number == cellNumber))
                    return new ServiceResponse<AcceptTradeDto>(false, "Гравець, який робить пропозицію обміну, не володіє всіма клітинками, які він пропонує", HttpStatusCode.BadRequest, null);
            }

            foreach (int cellNumber in game.CurrentTradeOffer.OffereeProposition.CellNumbers)
            {
                if (!offeree.OwnedCells.Any(c => c.Number == cellNumber))
                    return new ServiceResponse<AcceptTradeDto>(false, "Гравець, який отримує пропозицію обміну, не володіє всіма клітинками, які він пропонує", HttpStatusCode.BadRequest, null);
            }

            return new ServiceResponse<AcceptTradeDto>(true, "Валідація успішна", HttpStatusCode.OK, null);
        }
        private ServiceResponse<CancelTradeDto> ValidateCancelTradeOffer(Game? game, Guid cancelerAccountId, out Player? canceler)
        {
            canceler = null;

            if (game == null)
                return new ServiceResponse<CancelTradeDto>(false, "Гру не знайдено", HttpStatusCode.NotFound, null);

            if (game.CurrentTradeOffer == null)
                return new ServiceResponse<CancelTradeDto>(false, "Немає активної пропозиції обміну", HttpStatusCode.NotFound, null);

            canceler = game.Players.FirstOrDefault(p => p.AccountId == cancelerAccountId);

            if (canceler == null)
                return new ServiceResponse<CancelTradeDto>(false, "Гравця, який скасовує пропозицію обміну, не знайдено", HttpStatusCode.NotFound, null);

            if (game.CurrentTradeOffer.OffererId != canceler.Id && game.CurrentTradeOffer.OffereeId != canceler.Id)
                return new ServiceResponse<CancelTradeDto>(false, "Неможливо скасувати пропозицію обміну, якщо ви не є її учасником", HttpStatusCode.BadRequest, null);

            return new ServiceResponse<CancelTradeDto>(true, "Валідація успішна", HttpStatusCode.OK, null);
        }
    }
}