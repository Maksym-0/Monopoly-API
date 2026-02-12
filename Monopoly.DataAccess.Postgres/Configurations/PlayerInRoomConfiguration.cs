using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Monopoly.Core.Models.Room;

namespace Monopoly.DataAccess.Postgres.Configurations
{
    public class PlayerInRoomConfiguration : IEntityTypeConfiguration<PlayerInRoom>
    {
        public void Configure(EntityTypeBuilder<PlayerInRoom> builder)
        {
            builder.HasKey(p => p.Id);

            builder.HasOne(p => p.Account)
                .WithOne()
                .HasForeignKey<PlayerInRoom>(p => p.AccountId);

            builder.HasOne(p => p.Room)
                .WithMany(r => r.Players)
                .HasForeignKey(p => p.RoomId)
                .IsRequired();
        }
    }
}
