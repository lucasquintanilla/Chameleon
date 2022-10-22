using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Data.EF.EntityTypeConfigurations
{

    public class VoxEntityTypeConfiguration : IEntityTypeConfiguration<Vox>
    {
        public void Configure(EntityTypeBuilder<Vox> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.Hash).IsUnique();

            builder.HasIndex(x => x.Bump);

            builder.Property(x => x.MediaId)
             .IsRequired(true)
             .IsUnicode(true);

            builder.Property(x => x.UserId)
              .IsRequired(true)
              .IsUnicode(true);

            builder.Property(x => x.CategoryId)
             .IsRequired(true)
             .IsUnicode(true);

            builder.Property(x => x.Content)
                .IsUnicode(true)
                .HasMaxLength(2000);

            builder.Property(x => x.Title)
                .IsUnicode(true)
                .HasMaxLength(100);

            builder.HasOne(x => x.Attachment)
              .WithMany()
              //.OnDelete(DeleteBehavior.Restrict)
              .HasForeignKey(x => x.MediaId);

            builder.HasOne(x => x.Owner)
              .WithMany()
              //.OnDelete(DeleteBehavior.Restrict)
              .HasForeignKey(x => x.UserId);

            builder.HasOne(x => x.Category)
              .WithMany()
              //.OnDelete(DeleteBehavior.Restrict)
              .HasForeignKey(x => x.CategoryId);

            builder.Property(x => x.IpAddress)
                .HasMaxLength(50);

            builder.Property(x => x.UserAgent)
                .HasMaxLength(500);
        }
    }
}
