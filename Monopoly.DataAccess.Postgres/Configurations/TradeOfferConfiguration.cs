using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Monopoly.Core.Models.Game.OfferSystem;

namespace Monopoly.DataAccess.Postgres.Configurations
{
    internal class TradeOfferConfiguration : IEntityTypeConfiguration<TradeOffer>
    {
        public void Configure(EntityTypeBuilder<TradeOffer> builder)
        {
            builder.HasKey(t => t.Id);

            builder.HasOne(t => t.Game)
                .WithOne (g => g.CurrentTradeOffer)
                .HasForeignKey<TradeOffer>(t => t.GameId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(t => t.Offerer)
                .WithMany()
                .HasForeignKey(t => t.OffererId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.Offeree)
                .WithMany()
                .HasForeignKey(t => t.OffereeId)
                .OnDelete(DeleteBehavior.Restrict);
           
            builder.HasOne(t => t.OffererProposition)
                .WithOne()
                .HasForeignKey<TradeOffer>(t => t.OffererPropositionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(t => t.OffereeProposition)
                .WithOne()
                .HasForeignKey<TradeOffer>(t => t.OffereePropositionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
