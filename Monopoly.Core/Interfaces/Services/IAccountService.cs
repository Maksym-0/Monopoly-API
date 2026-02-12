using Monopoly.Core.DTO.Accounts;
using Monopoly.Core.Models.Service;

namespace Monopoly.Core.Interfaces.Services
{
    public interface IAccountService
    {
        Task<ServiceResponse<AccountDto>> MeAsync(Guid id);
        Task<ServiceResponse<AccountDto>> RegisterAsync(string name, string password);
        Task<ServiceResponse<LoginDto>> LoginAsync(string name, string password);
        Task<ServiceResponse<DeleteAccountDto>> DeleteAsync(string name, string password);
    }
}
