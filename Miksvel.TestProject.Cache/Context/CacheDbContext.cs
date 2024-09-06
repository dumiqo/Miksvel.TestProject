using Microsoft.EntityFrameworkCore;

namespace Miksvel.TestProject.Cache.Context
{
    public class CacheDbContext : DbContext
    {
        public CacheDbContext(DbContextOptions<CacheDbContext> options)
        : base(options)
        {
        }
        public DbSet<CacheEntry> Cache { get; set; }
    }
}
