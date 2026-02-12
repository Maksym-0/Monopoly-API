using Monopoly.Core.Interfaces.Repositories;
using Monopoly.Core.Interfaces.UnitsOfWork;

namespace Monopoly.DataAccess.Postgres.UnitsOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MonopolyDbContext _context;

        public UnitOfWork(MonopolyDbContext context,
            IAccountsRepository accountsRepository,
            IRoomsRepository roomsRepository,
            IGamesRepository gamesRepository,
            IBoardsRepository boardsRepository,
            IPlayersRepository playersRepository,
            ITurnStatesRepository turnStatesRepository)
        {
            _context = context;
            Accounts = accountsRepository;
            Rooms = roomsRepository;
            Games = gamesRepository;
            Boards = boardsRepository;
            Players = playersRepository;
            TurnStates = turnStatesRepository;
        }

        public IAccountsRepository Accounts { get; }

        public IRoomsRepository Rooms { get; }

        public IGamesRepository Games { get; }
        public IBoardsRepository Boards { get; }
        public IPlayersRepository Players { get; }
        public ITurnStatesRepository TurnStates { get; }

        public void Dispose()
        {
            _context.Dispose();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
