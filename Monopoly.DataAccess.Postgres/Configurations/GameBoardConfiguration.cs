using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Monopoly.Core.Models.Game;

namespace Monopoly.DataAccess.Postgres.Configurations
{
    public class GameBoardConfiguration : IEntityTypeConfiguration<GameBoard>
    {
        public void Configure(EntityTypeBuilder<GameBoard> builder)
        {
            builder.HasKey(g => g.Id);

            builder.HasMany(g => g.Cells)
                .WithOne(c => c.Board)
                .HasForeignKey(c => c.BoardId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(g => g.Monopolies)
                .WithOne(m => m.Board)
                .HasForeignKey(m => m.BoardId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
