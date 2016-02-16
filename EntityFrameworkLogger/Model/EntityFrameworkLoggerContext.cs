using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace EntityFrameworkLogger.Model
{
    public class EntityFrameworkLoggerContext : DbContext
    {
        public EntityFrameworkLoggerContext() : base("EntityFrameworkLoggerContext")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

        public DbSet<Artist> Artist { get; set; }
        public DbSet<Album> Album { get; set; }
        public DbSet<Track> Track { get; set; }
    }
}
