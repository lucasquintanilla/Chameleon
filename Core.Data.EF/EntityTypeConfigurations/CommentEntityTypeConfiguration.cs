using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;

namespace Core.Data.EF.EntityTypeConfigurations
{
    public class CommentEntityTypeConfiguration : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.HasKey(x => x.ID);

            builder.HasIndex(x => x.Hash).IsUnique();

            builder.Property(x => x.VoxID)
               .IsRequired(true)
               .IsUnicode(true);

            builder.Property(x => x.MediaID)
              .IsRequired(false)
              .IsUnicode(true);

            builder.Property(x => x.UserID)
              .IsRequired(true)
              .IsUnicode(true);

            builder.Property(x => x.Content)
                .IsUnicode(true)
                .HasMaxLength(1000);

            //builder.HasOne(x => x.Vox)
            //  .WithMany()
            //  //.OnDelete(DeleteBehavior.Restrict)
            //  .HasForeignKey(x => x.VoxID);

            builder.HasOne(x => x.Media)
              .WithMany()
              //.OnDelete(DeleteBehavior.Restrict)
              .HasForeignKey(x => x.MediaID);

            builder.HasOne(x => x.User)
              .WithMany()
              //.OnDelete(DeleteBehavior.Restrict)
              .HasForeignKey(x => x.UserID);
        }
    }
}
