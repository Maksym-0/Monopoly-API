using Microsoft.EntityFrameworkCore;
using Monopoly.Core.Models.Account;
using Monopoly.Core.Models.Game;
using Monopoly.Core.Models.Room;
using Monopoly.Core.Models.Abstractions.Classes;
using Monopoly.DataAccess.Postgres.Configurations;
using Monopoly.Core.Models.Game.OfferSystem;

namespace Monopoly.DataAccess.Postgres
{
    public class MonopolyDbContext : DbContext
    {
        public MonopolyDbContext(DbContextOptions<MonopolyDbContext> options)
            : base(options) { }

        public DbSet<Account> Accounts { get; set; }
        
        public DbSet<Room> Rooms { get; set; }
        public DbSet<PlayerInRoom> PlayersInRooms { get; set; }
        
        public DbSet<Game> Games { get; set; }
        public DbSet<TurnState> TurnStates { get; set; }

        public DbSet<Player> Players { get; set; }
        public DbSet<Dice> Dices { get; set; }

        public DbSet<GameBoard> Boards { get; set; }
        public DbSet<Cell> Cells { get; set; }
        public DbSet<Core.Models.Game.Monopoly> Monopolies { get; set; }

        public DbSet<TradeOffer> TradeOffers { get; set; }
        public DbSet<Proposition> Propositions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new AccountConfiguration());

            modelBuilder.ApplyConfiguration(new RoomConfiguration());
            modelBuilder.ApplyConfiguration(new PlayerInRoomConfiguration());

            modelBuilder.ApplyConfiguration(new GameConfiguration());
            modelBuilder.ApplyConfiguration(new TurnStateConfiguration());

            modelBuilder.ApplyConfiguration(new PlayerConfiguration());
            modelBuilder.ApplyConfiguration(new DiceConfiguration());

            modelBuilder.ApplyConfiguration(new GameBoardConfiguration());
            modelBuilder.ApplyConfiguration(new CellConfiguration());
            modelBuilder.ApplyConfiguration(new CompanyCellConfiguration());
            modelBuilder.ApplyConfiguration(new MonopolyConfiguration());

            modelBuilder.ApplyConfiguration(new TradeOfferConfiguration());
            modelBuilder.ApplyConfiguration(new PropositionConfiguration());
        }
    }
}
