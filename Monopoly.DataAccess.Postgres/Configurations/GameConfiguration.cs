using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Monopoly.Core.Models.Game;
using Monopoly.Core.Models.Game.OfferSystem;

namespace Monopoly.DataAccess.Postgres.Configurations
{
    public class GameConfiguration : IEntityTypeConfiguration<Game>
    {
        public void Configure(EntityTypeBuilder<Game> builder)
        {
            builder.HasKey(g => g.Id);

            builder.HasOne(g => g.Room)
                .WithOne(r => r.Game)
                .HasForeignKey<Game>(g => g.RoomId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(g => g.Players)
                .WithOne(p => p.Game)
                .HasForeignKey(p => p.GameId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(g => g.Board)
                .WithOne(b => b.Game)
                .HasForeignKey<GameBoard>(b => b.GameId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(g => g.TurnState)
                .WithOne(t => t.Game)
                .HasForeignKey<TurnState>(t => t.GameId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(g => g.CurrentTradeOffer)
                .WithOne(t => t.Game)
                .HasForeignKey<TradeOffer>(t => t.GameId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
