using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VueExample.Models;

namespace VueExample.Entities.Configurations
{
    public class ProcessConfiguration : IEntityTypeConfiguration<Process>
    {
        public void Configure(EntityTypeBuilder<Process> builder)
        {
            builder.HasMany(k => k.CodeProducts).WithOne(e => e.Process).HasForeignKey(fk => fk.ProcessId);
        }
    }
}