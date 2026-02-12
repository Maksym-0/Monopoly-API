using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Monopoly.Core.Models.Account;

namespace Monopoly.DataAccess.Postgres.Configurations
{
    public class AccountConfiguration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder) 
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Name).IsRequired();
            builder.Property(a => a.Password).IsRequired();
            
            builder.HasIndex(a => a.Name).IsUnique();
        }
    }
}
