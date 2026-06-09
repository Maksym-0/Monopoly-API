using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Monopoly.Core.DTO.Games;
using Monopoly.Core.Interfaces.Services;
using Monopoly.Core.Models.ApiRequest;
using System.Security.Claims;

namespace Monopoly.API.Controllers
{
    [ApiController]
    [Route("api/game/{gameId}/trade")]
    [Authorize]
    public class TradingController : ControllerBase
    {
        private readonly ITradingService _tradingService;

        public TradingController(ITradingService tradingService)
        {
            _tradingService = tradingService;
        }
        [HttpGet]
        public async Task<IActionResult> GetCurrentTradeAsync(string gameId)
        {
            try
            {
                var result = await _tradingService.GetCurrentTradeOfferAsync(Guid.Parse(gameId));
                ApiResponse<TradeOfferDto> response = new ApiResponse<TradeOfferDto>()
                {
                    Success = result.Success,
                    Message = result.Message,
                    Data = result.Data
                };
                return StatusCode((int)result.StatusCode, response);
            }
            catch (Exception ex)
            {
                return CatchInternalServerError(ex);
            }
        }
        [HttpPost("create")]
        public async Task<IActionResult> CreateTradeAsync(string gameId, CreateTradeOfferRequest request)
        {
            try
            {
                var result = await _tradingService.CreateTradeOfferAsync(Guid.Parse(gameId), Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value), request);
                ApiResponse<TradeOfferDto> response = new ApiResponse<TradeOfferDto>()
                {
                    Success = result.Success,
                    Message = result.Message,
                    Data = result.Data
                };
                return StatusCode((int)result.StatusCode, response);
            }
            catch (Exception ex)
            {
                return CatchInternalServerError(ex);
            }
        }
        [HttpPut("accept")]
        public async Task<IActionResult> AcceptTradeAsync(string gameId)
        {
            try
            {
                var result = await _tradingService.AcceptTradeOfferAsync(Guid.Parse(gameId), Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value));
                ApiResponse<AcceptTradeDto> response = new ApiResponse<AcceptTradeDto>()
                {
                    Success = result.Success,
                    Message = result.Message,
                    Data = result.Data
                };
                return StatusCode((int)result.StatusCode, response);
            }
            catch (Exception ex)
            {
                return CatchInternalServerError(ex);
            }
        }
        [HttpDelete("decline")]
        public async Task<IActionResult> DeclineTradeAsync(string gameId)
        {
            try
            {
                var result = await _tradingService.CancelTradeOfferAsync(Guid.Parse(gameId), Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value));
                ApiResponse<CancelTradeDto> response = new ApiResponse<CancelTradeDto>()
                {
                    Success = result.Success,
                    Message = result.Message,
                    Data = result.Data
                };
                return StatusCode((int)result.StatusCode, response);
            }
            catch(Exception ex)
            {
                return CatchInternalServerError(ex);
            }
        }

        private IActionResult CatchInternalServerError(Exception ex)
        {
            ApiResponse<object> response = new ApiResponse<object>()
            {
                Success = false,
                Message = ex.Message,
                Data = null
            };
            return StatusCode(500, response);
        }
    }
}
