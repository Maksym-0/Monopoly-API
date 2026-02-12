using Microsoft.EntityFrameworkCore;
using Monopoly.Core.Interfaces.Repositories;
using Monopoly.Core.Models.Account;

namespace Monopoly.DataAccess.Postgres.Repositories
{
    public class AccountsRepository : IAccountsRepository
    {
        private readonly MonopolyDbContext _context;

        public AccountsRepository(MonopolyDbContext context)
        {
            _context = context;
        }

        public async Task<Account?> GetById(Guid accountId)
        {
            return await _context.Accounts
                .FirstOrDefaultAsync(a => a.Id == accountId);
        }
        public async Task<Account?> GetByName(string name)
        {
            return await _context.Accounts
                .FirstOrDefaultAsync(a => a.Name == name);
        }
        public async Task Add(Account account)
        {
            await _context.AddAsync(account);
        }
        public async Task DeleteById(Guid accountId)
        {
            Account? account = await GetById(accountId);

            if(account != null)
                _context.Accounts.Remove(account);
        }
    }
}
