using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Monopoly.Core.Interfaces.Services;
using Monopoly.Core.Models.Request;
using System.Security.Claims;
using Monopoly.API;
using Monopoly.Core.DTO.Rooms;

namespace Monopoly.Controllers
{
    [ApiController]
    [Route("api/rooms")]
    [Authorize]
    public class RoomController : ControllerBase
    {
        private readonly IRoomService _roomService;

        public RoomController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        [HttpGet]
        public async Task<IActionResult> GetRooms()
        {
            try
            {
                var result = await _roomService.GetAllRoomsAsync();
                ApiResponse<List<RoomDto>> response = new(result.Success, result.Message, result.Data);
                return StatusCode((int)result.StatusCode, response); 
            }
            catch(Exception ex)
            {
                return CatchInternalServerError(ex);
            }
        }
        [HttpPost("create")]
        public async Task<IActionResult> CreateRoom([FromBody] CreateRoomRequest dto)
        {
            try
            {
                var result = await _roomService.CreateRoomAsync(dto.MaxNumberOfPlayers, dto.Password, Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value));
                ApiResponse<RoomDto> response = new ApiResponse<RoomDto>(result.Success, result.Message, result.Data);
                return StatusCode((int)result.StatusCode, response);
            }
            catch (Exception ex)
            {
                return CatchInternalServerError(ex);
            }
        }
        [HttpPut("{roomId}/join")]
        public async Task<IActionResult> JoinRoom([FromBody] JoinRoomRequest dto)
        {
            try
            {
                var result = await _roomService.JoinRoomAsync(dto.RoomId, dto.Password, Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value));
                ApiResponse<JoinRoomDto> response = new(result.Success, result.Message, result.Data);
                return StatusCode((int)result.StatusCode, response);
            }
            catch(Exception ex)
            {
                return CatchInternalServerError(ex);
            }
        }
        [HttpPut("quit")]
        public async Task<IActionResult> QuitRoom()
        {
            try
            {
                var result = await _roomService.QuitRoomAsync(Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value));
                ApiResponse<QuitRoomDto> response = new(result.Success, result.Message, result.Data);
                return StatusCode((int)result.StatusCode, response);
            }
            catch(Exception ex)
            {
                return CatchInternalServerError(ex);
            }
        }

        private IActionResult CatchInternalServerError(Exception ex)
        {
            ApiResponse<List<Object>> response = new ApiResponse<List<Object>>()
            {
                Success = false,
                Message = ex.Message,
                Data = null
            };
            return StatusCode(500, response);
        }
    }
}
