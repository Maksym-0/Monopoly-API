using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Monopoly.Core.Models.Game;

namespace Monopoly.DataAccess.Postgres.Configurations
{
    public class DiceConfiguration : IEntityTypeConfiguration<Dice>
    {
        public void Configure(EntityTypeBuilder<Dice> builder)
        {
            builder.HasKey(d => d.Id);

            builder.HasOne(d => d.Player)
                .WithOne(p => p.Dices)
                .HasForeignKey<Dice>(d => d.PlayerId);
        }
    }
}
