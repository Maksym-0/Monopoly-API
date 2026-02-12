using Monopoly.Core.Interfaces.Repositories;

namespace Monopoly.Core.Interfaces.UnitsOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IAccountsRepository Accounts { get; }
        IRoomsRepository Rooms { get; }
        IGamesRepository Games { get; }
        IBoardsRepository Boards { get; }
        IPlayersRepository Players { get; }
        ITurnStatesRepository TurnStates { get; }

        Task<int> SaveChangesAsync();
    }
}
