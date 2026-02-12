using System.Net;
using Monopoly.Core.Interfaces.Services;
using Monopoly.Core.Models.Game;
using Monopoly.Core.Models.Room;
using Monopoly.Core.Models.Service;
using Monopoly.Core.DTO.Rooms;
using Monopoly.Core.Interfaces.UnitsOfWork;
using Monopoly.Core.Factories;
using Monopoly.Core.Models.Account;

namespace Monopoly.Application.Services
{
    public class RoomService : IRoomService
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly IGameService _gameService;

        public RoomService(IUnitOfWork unitOfWork,
            IGameService gameService) 
        {
            _unitOfWork = unitOfWork;

            _gameService = gameService;
        }

        public async Task<ServiceResponse<List<RoomDto>>> GetAllRoomsAsync()
        {
            List<Room> rooms = await _unitOfWork.Rooms.GetRooms();
            List<RoomDto> roomResponces = new List<RoomDto>();

            ServiceResponse<List<RoomDto>> response = new ServiceResponse<List<RoomDto>>(true, "Отримано список наявних кімнат", HttpStatusCode.OK, roomResponces);

            if (rooms.Count == 0)
                return response;

            for (int i = 0; i < rooms.Count; i++)
                roomResponces.Add(new RoomDto(rooms[i]));

            response.Data = roomResponces;

            return response;
        }
        public async Task<ServiceResponse<RoomDto>> CreateRoomAsync(int maxNumberOfPlayers, string? password, Guid accountId)
        {
            Account? account = await _unitOfWork.Accounts.GetById(accountId);
            PlayerInRoom? playerInRoom = await _unitOfWork.Rooms.GetPlayerByAccountId(accountId);

            ServiceResponse<RoomDto> response = ValidateCreateRoomAsync(maxNumberOfPlayers, playerInRoom, account);
            if (!response.Success)
                return response;
            
            Room room;
            if (password != null)
                room = new Room(Guid.NewGuid(), maxNumberOfPlayers, password);
            else
                room = new Room(Guid.NewGuid(), maxNumberOfPlayers, null);

            int playerIndex = 1;
            PlayerInRoom newPlayerInRoom = new PlayerInRoom(Guid.NewGuid(), accountId, room.Id, account, account.Name, playerIndex);
            
            room.AddPlayer(newPlayerInRoom, password);

            await _unitOfWork.Rooms.AddRoom(room);

            await _unitOfWork.SaveChangesAsync();
            
            RoomDto roomDto = new RoomDto(room);

            response.Message = "Кімнату створено";
            response.Data = roomDto;

            return response;
        }
        public async Task<ServiceResponse<JoinRoomDto>> JoinRoomAsync(Guid roomId, string? password, Guid accountId)
        {
            Room? room = await _unitOfWork.Rooms.GetRoomById(roomId);
            PlayerInRoom? playerInRoom = await _unitOfWork.Rooms.GetPlayerByAccountId(accountId);
            Account? account = await _unitOfWork.Accounts.GetById(accountId);

            ServiceResponse<JoinRoomDto> response = ValidateJoinRoomAsync(room, password, playerInRoom, account);
            if (!response.Success)
                return response;

            PlayerInRoom newPlayerInRoom = new PlayerInRoom(Guid.NewGuid(), accountId, roomId, null, account.Name, room.CountOfPlayers + 1);
            room.AddPlayer(newPlayerInRoom, password);

            await _unitOfWork.Rooms.AddPlayer(roomId, newPlayerInRoom);

            if (room.CountOfPlayers >= room.MaxNumberOfPlayers)
                await StartGameInRoom(room);

            await _unitOfWork.SaveChangesAsync();

            JoinRoomDto joinRoomDto = new JoinRoomDto()
            {
                Success = true,
                IsGameStarted = room.InGame,
                Room = new RoomDto(room)
            };

            if (room.InGame)
                response.Message = "Гравця додано до кімнати. Гру розпочато";
            else
                response.Message = "Гравця додано до кімнати";

            response.Data = joinRoomDto;

            return response;
        }
        public async Task<ServiceResponse<QuitRoomDto>> QuitRoomAsync(Guid accountId)
        {
            PlayerInRoom? playerToRemove = await _unitOfWork.Rooms.GetPlayerByAccountId(accountId);

            ServiceResponse<QuitRoomDto> response = ValidateQuitRoomAsync(playerToRemove);
            if (!response.Success)
                return response;

            Room room = await _unitOfWork.Rooms.GetRoomById(playerToRemove.RoomId);

            QuitRoomDto quitRoomDto = new QuitRoomDto()
            {
                IsRoomDeleted = false,
                PlayerName = playerToRemove.Name,
                Winner = null,
                RemainingPlayers = 0,
                RoomDto = new RoomDto(room)
            };

            if (room.InGame)
            {
                Player gamePlayer = room.Game.Players.First(p => p.AccountId == accountId);
                var serviceResponse = await _gameService.LeaveGameAsync(room.Game.Id, gamePlayer.Id);
                
                if (serviceResponse.Data.IsGameOver)
                {
                    quitRoomDto.IsRoomDeleted = true;
                    quitRoomDto.Winner = serviceResponse.Data.Winner;
                    quitRoomDto.RoomDto = null;
                }

                response.Message = "Гравець покинув гру та кімнату";
            }
            else
            {
                room.RemovePlayerById(playerToRemove.Id);

                if (room.CountOfPlayers > 0)
                {
                    quitRoomDto.IsRoomDeleted = false;
                    quitRoomDto.PlayerName = playerToRemove.Name;
                    quitRoomDto.RemainingPlayers = room.CountOfPlayers;
                    quitRoomDto.Winner = null;
                }
                else
                {
                    await _unitOfWork.Rooms.DeleteById(playerToRemove.RoomId);

                    quitRoomDto.IsRoomDeleted = true;
                    quitRoomDto.PlayerName = playerToRemove.Name;
                    quitRoomDto.RemainingPlayers = room.CountOfPlayers;
                    quitRoomDto.Winner = null;
                    quitRoomDto.RoomDto = null;
                }
            }

            await _unitOfWork.SaveChangesAsync();

            quitRoomDto.RoomDto = new RoomDto(room);
            response.Data = quitRoomDto;

            return response;
        }

        private ServiceResponse<RoomDto> ValidateCreateRoomAsync(int maxNumberOfPlayers, PlayerInRoom? playerInRoom, Account? account)      
        {
            if (account == null)
                return new ServiceResponse<RoomDto>(false, "Акаунт не знайдено", HttpStatusCode.NotFound, null);

            if (playerInRoom != null)
                return new ServiceResponse<RoomDto>(false, "Неможливо створити кімнату. Гравець вже перебуває в кімнаті", HttpStatusCode.BadRequest, null);

            if (maxNumberOfPlayers < 2 || maxNumberOfPlayers > 4)
                return new ServiceResponse<RoomDto>(false, "Максимальна кількість гравців обмежена від 2 до 4 осіб", HttpStatusCode.BadRequest, null);

            return new ServiceResponse<RoomDto>(true, "Валідація успішна", HttpStatusCode.OK, null);
        }
        private ServiceResponse<JoinRoomDto> ValidateJoinRoomAsync(Room? room, string? password, PlayerInRoom? playerInRoom, Account? account)
        {
            if (account == null)
                return new ServiceResponse<JoinRoomDto>(false, "Акаунт не знайдено", HttpStatusCode.NotFound, null);

            if (room == null)
                return new ServiceResponse<JoinRoomDto>(false, "Кімнату не знайдено", HttpStatusCode.NotFound, null);

            if (room.MaxNumberOfPlayers <= room.CountOfPlayers)
                return new ServiceResponse<JoinRoomDto>(false, "Неможливо приєднатись. Кімнату переповнено", HttpStatusCode.BadRequest, null);

            if (room.InGame)
                return new ServiceResponse<JoinRoomDto>(false, "Неможливо приєднатись. В кімнаті розпочато гру", HttpStatusCode.BadRequest, null);

            if (room.Password != null && room.Password != password)
                return new ServiceResponse<JoinRoomDto>(false, "Неможливо приєднатись. Невірний пароль", HttpStatusCode.BadRequest, null);

            if (playerInRoom != null)
                return new ServiceResponse<JoinRoomDto>(false, "Неможливо приєднатись. Гравець вже в кімнаті", HttpStatusCode.BadRequest, null);

            return new ServiceResponse<JoinRoomDto>(true, "Валідація успішна", HttpStatusCode.OK, null);
        }
        private ServiceResponse<QuitRoomDto> ValidateQuitRoomAsync(PlayerInRoom? playerInRoom)
        {
            if (playerInRoom == null)
                return new ServiceResponse<QuitRoomDto>(false, "Неможливо покинути кімнату. Гравець не перебуває в жодній кімнаті", HttpStatusCode.BadRequest, null);

            return new ServiceResponse<QuitRoomDto>(true, "Валідація успішна", HttpStatusCode.OK, null);
        }

        private async Task<Game> StartGameInRoom(Room room)
        {
            Game game = GameFactory.CreateGame(room);

            await _unitOfWork.Games.Add(game);

            room.StartGame();
            game.StartGame();

            return game;
        }
    }
}
