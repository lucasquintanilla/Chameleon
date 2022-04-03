using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Data.EF.EntityTypeConfigurations
{
    public class UserVoxActionEntityConfiguration : IEntityTypeConfiguration<UserVoxAction>
    {
        public void Configure(EntityTypeBuilder<UserVoxAction> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasIndex(x => new { x.UserId, x.VoxId }).IsUnique();

            builder.Property(x => x.UserId)
              .IsRequired(true)
              .IsUnicode(true);

            builder.HasOne(x => x.User)
              .WithMany()
              //.OnDelete(DeleteBehavior.Restrict)
              .HasForeignKey(x => x.UserId);

            builder.Property(x => x.VoxId)
              .IsRequired(true)
              .IsUnicode(true);

            builder.HasOne(x => x.Vox)
              .WithMany()
              //.OnDelete(DeleteBehavior.Restrict)
              .HasForeignKey(x => x.VoxId);
        }
    }
}
