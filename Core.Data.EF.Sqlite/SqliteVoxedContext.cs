using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
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

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite(Configuration.GetConnectionString("Sqlite"));
        }
    }
}
