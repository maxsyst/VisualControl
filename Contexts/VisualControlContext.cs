using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server = localhost,1401; Database = VisualControl; User = SA; password = <zx12cv>; MultipleActiveResultSets = True;");
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
