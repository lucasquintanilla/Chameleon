using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Data.EF.EntityTypeConfigurations
{
    public class MediaEntityTypeConfiguration : IEntityTypeConfiguration<Attachment>
    {
        public void Configure(EntityTypeBuilder<Attachment> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Url)
               .IsUnicode(true);

            builder.Property(x => x.ThumbnailUrl)
               .IsUnicode(true);
        }
    }
}
