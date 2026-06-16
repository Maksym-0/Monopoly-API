using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using Monopoly.Core;
using Monopoly.Core.Interfaces.Services;
using Monopoly.Core.Models.Account;
using Monopoly.Core.DTO.Accounts;
using Monopoly.Core.Models.Service;
using Monopoly.Core.Interfaces.UnitsOfWork;
using Monopoly.Core.Models.Room;

namespace Monopoly.Application.Services
{
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        public AccountService(IUnitOfWork unitOfWork, IConfiguration configuration) 
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }

        public async Task<ServiceResponse<AccountDto>> MeAsync(Guid id)
        {
            Account? account = await _unitOfWork.Accounts.GetByIdAsync(id);

            ServiceResponse<AccountDto> response = ValidateMe(account);
            if (!response.Success)
                return response;

            AccountDto accountDto = new AccountDto(account.Id, account.Name);

            response.Message = "Інформацію акаунту отримано";
            response.Data = accountDto;

            return response;
        }
        public async Task<ServiceResponse<AccountDto>> RegisterAsync(string name, string password)
        {
            Account? account = await _unitOfWork.Accounts.GetByNameAsync(name);

            ServiceResponse<AccountDto> response = ValidateRegister(account, password);
            if (!response.Success)
                return response;

            account = new Account(Guid.NewGuid(), name, password);
            await _unitOfWork.Accounts.AddAsync(account);

            await _unitOfWork.SaveChangesAsync();

            AccountDto accountDto = new AccountDto(account.Id, account.Name);

            response.Message = "Акаунт створено";
            response.Data = accountDto;

            return response;
        }
        public async Task<ServiceResponse<LoginDto>> LoginAsync(string name, string password)
        {
            Account? account = await _unitOfWork.Accounts.GetByNameAsync(name);

            ServiceResponse<LoginDto> response = ValidateLogin(account, password);
            if (!response.Success)
                return response;

            string token = GenerateToken(account.Id, account.Name);
            
            LoginDto loginDto = new LoginDto()
            {
                Account = new AccountDto(account.Id, account.Name),
                Token = token,

                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddHours(12)
            };

            response.Message = "Вхід до облікового запису завершено";
            response.Data = loginDto;

            return response;
        }
        public async Task<ServiceResponse<DeleteAccountDto>> DeleteAsync(string name, string password)
        {
            Account? account = await _unitOfWork.Accounts.GetByNameAsync(name);

            ServiceResponse<DeleteAccountDto> response = await ValidateDeleteAsync(account, password);
            if (!response.Success)
                return response;

            await _unitOfWork.Accounts.DeleteByIdAsync(account.Id);
            
            await _unitOfWork.SaveChangesAsync();

            DeleteAccountDto dto = new DeleteAccountDto()
            {
                AccountId = account.Id,
                Name = account.Name,

                IsDeleted = true
            };

            response.Message = "Видалення акаунту завершено";
            response.Data = dto;

            return response;
        }

        private ServiceResponse<AccountDto> ValidateMe(Account? account)
        {
            if (account == null)
                return new ServiceResponse<AccountDto>(false, "Акаунт не знайдено", HttpStatusCode.NotFound, null);

            return new ServiceResponse<AccountDto>(true, "Валідаія успішна", HttpStatusCode.OK, null);
        }
        private ServiceResponse<AccountDto> ValidateRegister(Account? account, string? password)
        {
            if (account != null)
                return new ServiceResponse<AccountDto>(false, "Акаунт із цим ім'ям вже зареєстровано", HttpStatusCode.BadRequest, null);

            if (password == null)
                return new ServiceResponse<AccountDto>(false, "Неможливо створити акаунт без пароля", HttpStatusCode.BadRequest, null);

            return new ServiceResponse<AccountDto>(true, "Валідація успішна", HttpStatusCode.OK, null);
        }
        private ServiceResponse<LoginDto> ValidateLogin(Account? account, string? password)
        {
            if (account == null)
                return new ServiceResponse<LoginDto>(false, "Акаунт із цим ім'ям не знайдено", HttpStatusCode.NotFound, null);

            if (password == null)
                return new ServiceResponse<LoginDto>(false, "Для входу в акаунт необхідно ввести пароль", HttpStatusCode.BadRequest, null);

            if (account.Password != password)
                return new ServiceResponse<LoginDto>(false, "Введено неправильний пароль", HttpStatusCode.BadRequest, null);

            return new ServiceResponse<LoginDto>(true, "Валідація успішна", HttpStatusCode.OK, null);
        }
        private async Task<ServiceResponse<DeleteAccountDto>> ValidateDeleteAsync(Account? account, string? password)
        {
            if (account == null)
                return new ServiceResponse<DeleteAccountDto>(false, "Акаунт із цим ім'ям не знайдено", HttpStatusCode.NotFound, null);

            if (password == null)
                return new ServiceResponse<DeleteAccountDto>(false, "Для видалення акаунту необхідно ввести пароль", HttpStatusCode.BadRequest, null);

            PlayerInRoom? playerInRoom = await _unitOfWork.Rooms.GetPlayerByAccountIdAsync(account.Id);

            if (account.Password != password)
                return new ServiceResponse<DeleteAccountDto>(false, "Введено неправильний пароль", HttpStatusCode.BadRequest, null);

            if (playerInRoom != null)
                return new ServiceResponse<DeleteAccountDto>(false, "Неможливо видалити акаунт до завершення ігрової сесії", HttpStatusCode.BadRequest, null);

            return new ServiceResponse<DeleteAccountDto>(true, "Валідація успішна", HttpStatusCode.OK, null);
        }

        private string GenerateToken(Guid id, string name)
        {
            var jwtKey = _configuration["JwtSettings:SecretKey"];
            if(jwtKey == null)
                throw new Exception("JWT ключ не налаштовано в конфігурації");

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(jwtKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity
                (new[]{ 
                        new Claim(ClaimTypes.Name, name), 
                        new Claim(ClaimTypes.NameIdentifier, id.ToString()) 
                    }),
                Expires = DateTime.UtcNow.AddHours(12),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
