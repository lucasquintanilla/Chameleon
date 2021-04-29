using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Core.Data.EF.EntityTypeConfigurations
{

    public class VoxEntityTypeConfiguration : IEntityTypeConfiguration<Vox>
    {
        public void Configure(EntityTypeBuilder<Vox> builder)
        {
            builder.HasKey(x => x.ID);

            builder.HasIndex(x => x.Hash).IsUnique();

            builder.HasIndex(x => x.Bump);

            builder.Property(x => x.MediaID)
             .IsRequired(true)
             .IsUnicode(true);

            builder.Property(x => x.UserID)
              .IsRequired(true)
              .IsUnicode(true);

            builder.Property(x => x.CategoryID)
             .IsRequired(true)
             .IsUnicode(true);

            builder.Property(x => x.Content)
                .IsUnicode(true)
                .HasMaxLength(2000);

            builder.Property(x => x.Title)
                .IsUnicode(true)
                .HasMaxLength(100);

            builder.HasOne(x => x.Media)
              .WithMany()
              //.OnDelete(DeleteBehavior.Restrict)
              .HasForeignKey(x => x.MediaID);

            builder.HasOne(x => x.User)
              .WithMany()
              //.OnDelete(DeleteBehavior.Restrict)
              .HasForeignKey(x => x.UserID);           

            builder.HasOne(x => x.Category)
              .WithMany()
              //.OnDelete(DeleteBehavior.Restrict)
              .HasForeignKey(x => x.CategoryID);

            builder.Property(x => x.IpAddress)
                .HasMaxLength(50);

            builder.Property(x => x.UserAgent)
                .HasMaxLength(500);
        }
    }
}
