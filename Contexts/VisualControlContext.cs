using Microsoft.EntityFrameworkCore;
using VueExample.Models;

namespace VueExample.Contexts
{
    public class VisualControlContext : DbContext
    {
        public DbSet<DefectType> DefectTypes { get; set; }
        public DbSet<DangerLevel> DangerLevels { get; set; }
        public DbSet<Defect> Defects { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<User> Users { get; set; }

        public VisualControlContext(DbContextOptions<VisualControlContext> options): base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DefectType>()
                .HasMany(c => c.Defects)
                .WithOne(e => e.DefectType);

            modelBuilder.Entity<DangerLevel>()
                .HasMany(c => c.Defects)
                .WithOne(e => e.DangerLevel);
        }
    }
}
