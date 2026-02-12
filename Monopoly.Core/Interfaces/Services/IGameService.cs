using Monopoly.Core.DTO.Games;
using Monopoly.Core.Models.Service;

namespace Monopoly.Core.Interfaces.Services
{
    public interface IGameService
    {
        Task<ServiceResponse<GameStateDto>> StatsOfGameAsync(Guid gameId);
        Task<ServiceResponse<MoveDto>> MoveAsync(Guid gameId, Guid accountId);
        Task<ServiceResponse<PayDto>> PayRentAsync(Guid gameId, Guid accountId);
        Task<ServiceResponse<PayDto>> PayToLeavePrisonAsync(Guid gameId, Guid accountId);
        Task<ServiceResponse<BuyDto>> BuyCellAsync(Guid gameId, Guid accountId);
        Task<ServiceResponse<LevelChangeDto>> LevelUpAsync(Guid gameId, Guid accountId, int cellNumber);
        Task<ServiceResponse<LevelChangeDto>> LevelDownAsync(Guid gameId, Guid accountId, int cellNumber);
        Task<ServiceResponse<NextActionDto>> EndActionAsync(Guid gameId, Guid accountId);
        Task<ServiceResponse<LeaveGameDto>> LeaveGameAsync(Guid gameId, Guid accountId);
    }
}
