using Microsoft.EntityFrameworkCore;
using VueExample.Models;
using VueExample.Models.Vertx;

namespace VueExample.Contexts
{
    public class LivePointContext : DbContext
    {
        public DbSet<LivePoint> LivePoints { get; set; }
        public DbSet<Graphic> Graphics { get; set; }
        public LivePointContext(DbContextOptions<LivePointContext> options): base(options)
        {
            
        }
    }
}