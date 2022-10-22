using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Data.EF.EntityTypeConfigurations
{
    public class CategoryEntityTypeConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired(true)
                .IsUnicode(true)
                .HasMaxLength(50);

            builder.Property(x => x.ShortName)
                .IsRequired(true)
                .IsUnicode(true)
                .HasMaxLength(50);

            builder.Property(x => x.AttachmentId)
              .IsRequired(true)
              .IsUnicode(true);

            builder.HasOne(x => x.Attachment)
             .WithMany()
             //.OnDelete(DeleteBehavior.Restrict)
             .HasForeignKey(x => x.AttachmentId);
        }
    }
}
