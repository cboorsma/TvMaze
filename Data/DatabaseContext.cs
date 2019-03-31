using Microsoft.EntityFrameworkCore;
using Model.DbModels;

namespace Model.Data
{
    public class DatabaseContext : DbContext
    {

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) {}

        public DbSet<TvShow> TvShows { get; set; }
        public DbSet<Cast> Cast { get; set; }
    }
}