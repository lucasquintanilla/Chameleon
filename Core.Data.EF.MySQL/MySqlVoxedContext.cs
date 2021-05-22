using Microsoft.EntityFrameworkCore;
using System;

namespace Core.Data.EF.MySql
{
    public class MySqlVoxedContext : VoxedContext
    {
        public MySqlVoxedContext(DbContextOptions<VoxedContext> options) : base(options)
        {
        }
    }
}
