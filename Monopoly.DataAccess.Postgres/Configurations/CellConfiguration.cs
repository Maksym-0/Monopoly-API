using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Monopoly.Core.Models.Abstractions.Classes;
using Monopoly.Core.Models.Game.Cells;

namespace Monopoly.DataAccess.Postgres.Configurations
{
    public class CellConfiguration : IEntityTypeConfiguration<Cell>
    {
        public void Configure(EntityTypeBuilder<Cell> builder)
        {
            builder.HasKey(c => c.Id);

            builder.HasOne(c => c.Board)
                .WithMany(b => b.Cells)
                .HasForeignKey(c => c.BoardId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasDiscriminator<string>("CellType")
                .HasValue<StartCell>("Start")
                .HasValue<CasinoCell>("Casino")
                .HasValue<PrisonCell>("Prison")
                .HasValue<RestCell>("Rest")
                .HasValue<AmbulanceCell>("Ambulance")
                .HasValue<ReceiveMoneyCell>("ReceiveMoney")
                .HasValue<ReverceCell>("Reverce")
                .HasValue<LosingMoneyCell>("LosingMoney")
                .HasValue<CompanyCell>("CompanyCell");
        }
    }
}
