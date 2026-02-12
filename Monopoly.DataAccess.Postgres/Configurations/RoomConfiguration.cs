using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Monopoly.Core.Models.Game;
using Monopoly.Core.Models.Room;

namespace Monopoly.DataAccess.Postgres.Configurations
{
    public class RoomConfiguration : IEntityTypeConfiguration<Room>
    {
        public void Configure(EntityTypeBuilder<Room> builder)
        {
            builder.HasKey(r => r.Id);

            builder.Property(r => r.MaxNumberOfPlayers)
                .IsRequired();

            builder.HasOne(r => r.Game)
                .WithOne(g => g.Room)
                .HasForeignKey<Game>(g => g.RoomId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(r => r.Players)
                .WithOne(p => p.Room)
                .HasForeignKey(p => p.RoomId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
