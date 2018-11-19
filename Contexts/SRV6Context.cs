using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VueExample.Models;

namespace VueExample.Contexts
{
    public class Srv6Context : DbContext
    {
        public DbSet<CodeProduct> CodeProduct { get; set; }
        public DbSet<Process> Process { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"data source = SRV6\SRV3; Initial Catalog = db_process; persist security info = True; user id = labuser; password = zxvitr78KK; MultipleActiveResultSets = True;");
        }
    }
}
