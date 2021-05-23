using Core.Data.EF.EntityTypeConfigurations;
using Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Data.EF.Sqlite
{
    public class SqliteVoxedContext : VoxedContext
    {
        protected readonly IConfiguration Configuration;
        public SqliteVoxedContext(DbContextOptions<VoxedContext> options, IConfiguration configuration) : base(options)
        {
            Configuration = configuration;
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder options)
        //{
        //    options.UseSqlite(Configuration.GetConnectionString("Sqlite"));
        //}
    }

    //public class SqliteVoxedContext : IdentityDbContext<User, Role, Guid>
    //{
    //    public SqliteVoxedContext(DbContextOptions<SqliteVoxedContext> options) : base(options)
    //    {
    //    }

    //    public DbSet<Vox> Voxs { get; set; }
    //    public DbSet<Comment> Comments { get; set; }
    //    public DbSet<Category> Categories { get; set; }
    //    public DbSet<Media> Media { get; set; }
    //    public DbSet<Poll> Polls { get; set; }
    //    public DbSet<Notification> Notifications { get; set; }

    //    protected override void OnModelCreating(ModelBuilder modelBuilder)
    //    {
    //        base.OnModelCreating(modelBuilder);

    //        modelBuilder.Entity<Poll>().ToTable(nameof(Polls));

    //        new UserEntityTypeConfiguration().Configure(modelBuilder.Entity<User>());
    //        new MediaEntityTypeConfiguration().Configure(modelBuilder.Entity<Media>());
    //        new CategoryEntityTypeConfiguration().Configure(modelBuilder.Entity<Category>());
    //        new VoxEntityTypeConfiguration().Configure(modelBuilder.Entity<Vox>());
    //        new CommentEntityTypeConfiguration().Configure(modelBuilder.Entity<Comment>());

    //        if (Database.ProviderName == "Microsoft.EntityFrameworkCore.Sqlite")
    //        {
    //            ////SQLite does not have proper support for DateTimeOffset via Entity Framework Core, see the limitations
    //            ////here: https://docs.microsoft.com/en-us/ef/core/providers/sqlite/limitations#query-limitations
    //            ////To work around this, when the Sqlite database provider is used, all model properties of type DateTimeOffset
    //            ////use the DateTimeOffsetToBinaryConverter
    //            ////Based on: https://github.com/aspnet/EntityFrameworkCore/issues/10784#issuecomment-415769754
    //            ////This only supports millisecond precision, but should be sufficient for most use cases.
    //            ///

    //            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
    //            {
    //                var properties = entityType.ClrType.GetProperties().Where(p => p.PropertyType == typeof(DateTimeOffset)
    //                                                                            || p.PropertyType == typeof(DateTimeOffset?));
    //                foreach (var property in properties)
    //                {
    //                    modelBuilder
    //                        .Entity(entityType.Name)
    //                        .Property(property.Name)
    //                        .HasConversion(new DateTimeOffsetToBinaryConverter());
    //                }
    //            }
    //        }
    //    }
    //}
}
