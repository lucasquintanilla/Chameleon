using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Core.Data.EF.MySql
{
    public class MySqlVoxedContext
    {
        protected readonly IConfiguration Configuration;

        public MySqlVoxedContext(DbContextOptions<VoxedContext> options, IConfiguration configuration)
        {
            Configuration = configuration;
        }
    }
}
