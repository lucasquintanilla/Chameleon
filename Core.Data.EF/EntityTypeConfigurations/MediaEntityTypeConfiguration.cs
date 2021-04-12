using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;

namespace Core.Data.EF.EntityTypeConfigurations
{
    public class MediaEntityTypeConfiguration : IEntityTypeConfiguration<Media>
    {
        public void Configure(EntityTypeBuilder<Media> builder)
        {
            builder.HasKey(x => x.ID);

            builder.Property(x => x.Url)
               .IsUnicode(true);

            builder.Property(x => x.ThumbnailUrl)
               .IsUnicode(true);
        }
    }
}
