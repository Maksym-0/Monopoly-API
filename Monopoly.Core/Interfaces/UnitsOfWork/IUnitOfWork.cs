using Monopoly.Core.Interfaces.Repositories;

namespace Monopoly.Core.Interfaces.UnitsOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IAccountsRepository Accounts { get; }
        IRoomsRepository Rooms { get; }
        IGamesRepository Games { get; }

        Task<int> SaveChangesAsync();
    }
}
