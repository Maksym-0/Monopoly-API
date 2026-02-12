using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Monopoly.Core.Models.Game;

namespace Monopoly.DataAccess.Postgres.Configurations
{
    public class PlayerConfiguration : IEntityTypeConfiguration<Player>
    {
        public void Configure(EntityTypeBuilder<Player> builder)
        {
            builder.HasKey(p => p.Id);

            builder.HasOne(p => p.Account)
                .WithMany()
                .HasForeignKey(p => p.AccountId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(p => p.OwnedCells)
                .WithOne(o => o.Owner)
                .HasForeignKey(o => o.OwnerId);

            builder.HasOne(p => p.Dices)
                .WithOne(d => d.Player)
                .HasForeignKey<Dice>(d => d.PlayerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(p => p.Game)
                .WithMany(g => g.Players)
                .HasForeignKey(p => p.GameId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
