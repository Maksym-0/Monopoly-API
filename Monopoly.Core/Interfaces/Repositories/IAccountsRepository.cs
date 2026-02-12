using Monopoly.Core.Models.Account;

namespace Monopoly.Core.Interfaces.Repositories
{
    public interface IAccountsRepository
    {
        Task<Account?> GetById(Guid accountId);
        Task<Account?> GetByName(string name);
        Task Add(Account account);
        Task DeleteById(Guid accountId);
    }
}
