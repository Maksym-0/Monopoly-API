using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Monopoly.Core.Models.Game.Cells;

namespace Monopoly.DataAccess.Postgres.Configurations
{
    public class CompanyCellConfiguration : IEntityTypeConfiguration<CompanyCell>
    {
        public void Configure(EntityTypeBuilder<CompanyCell> builder)
        {
            builder.HasOne(c => c.Monopoly)
                .WithMany(m => m.Cells)
                .HasForeignKey(c => c.MonopolyId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(c => c.Owner)
                .WithMany(o => o.OwnedCells)
                .HasForeignKey(c => c.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
