using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Data.EF.MySql
{
    public class MySqlVoxedContext : VoxedContext
    {
        public MySqlVoxedContext(DbContextOptions<VoxedContext> options) : base(options)
        {
        }
    }
}
