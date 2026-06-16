using Monopoly.Core.Models.Account;

namespace Monopoly.Core.Interfaces.Repositories
{
    public interface IAccountsRepository
    {
        Task<Account?> GetByIdAsync(Guid accountId);
        Task<Account?> GetByNameAsync(string name);
        Task AddAsync(Account account);
        Task DeleteByIdAsync(Guid accountId);
    }
}
