﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Core.Data.EF.Sqlite
{
    public class SqliteVoxedContext : VoxedContext
    {
        protected readonly IConfiguration Configuration;

        public SqliteVoxedContext(DbContextOptions<VoxedContext> options, IConfiguration configuration) : base(options)
        {
            Configuration = configuration;
        }
    }
}
