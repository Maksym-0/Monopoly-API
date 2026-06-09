using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Monopoly.Core.Models.Game.OfferSystem;

namespace Monopoly.DataAccess.Postgres.Configurations
{
    internal class PropositionConfiguration : IEntityTypeConfiguration<Proposition>
    {
        public void Configure(EntityTypeBuilder<Proposition> builder)
        {
            builder.HasKey(p => p.Id);
        }
    }
}
