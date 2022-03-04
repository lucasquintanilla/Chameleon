using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Core.Data.EF.MySql
{
    public class MySqlVoxedContext : VoxedContext
    {
        protected readonly IConfiguration Configuration;

        public MySqlVoxedContext(DbContextOptions<VoxedContext> options, IConfiguration configuration) : base(options)
        {
            Configuration = configuration;
        }
    }
}
