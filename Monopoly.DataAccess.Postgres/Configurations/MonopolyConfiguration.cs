using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Monopoly.DataAccess.Postgres.Configurations
{
    public class MonopolyConfiguration : IEntityTypeConfiguration<Core.Models.Game.Monopoly>
    {
        public void Configure(EntityTypeBuilder<Core.Models.Game.Monopoly> builder)
        {
            builder.HasKey(m => m.Id);

            builder.HasOne(m => m.Board)
                .WithMany(b => b.Monopolies)
                .HasForeignKey(m => m.BoardId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(m => m.Cells)
                .WithOne(c => c.Monopoly)
                .HasForeignKey(c => c.MonopolyId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
