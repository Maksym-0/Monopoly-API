using Monopoly.Core.DTO.Games;
using Monopoly.Core.Models.ApiRequest;
using Monopoly.Core.Models.Service;

namespace Monopoly.Core.Interfaces.Services
{
    public interface ITradingService
    {
        Task<ServiceResponse<TradeOfferDto>> GetCurrentTradeOfferAsync(Guid gameId);
        Task<ServiceResponse<TradeOfferDto>> CreateTradeOfferAsync(Guid gameId, Guid accountId, CreateTradeOfferRequest createTradeOfferRequest);
        Task<ServiceResponse<AcceptTradeDto>> AcceptTradeOfferAsync(Guid gameId, Guid accountId);
        Task<ServiceResponse<CancelTradeDto>> CancelTradeOfferAsync(Guid gameId, Guid accountId);
    }
}
