using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Monopoly.Core.Models.Game;

namespace Monopoly.DataAccess.Postgres.Configurations
{
    public class TurnStateConfiguration : IEntityTypeConfiguration<TurnState>
    {
        public void Configure(EntityTypeBuilder<TurnState> builder)
        {
            builder.HasKey(t => t.Id);

            builder.HasOne(t => t.Game)
                .WithOne(g => g.TurnState)
                .HasForeignKey<TurnState>(t => t.GameId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
