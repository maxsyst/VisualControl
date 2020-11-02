using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VueExample.Models;

namespace VueExample.Entities.Configurations
{
    public class CodeProductConfiguration : IEntityTypeConfiguration<CodeProduct>
    {
        public void Configure(EntityTypeBuilder<CodeProduct> builder)
        {
            builder.HasMany(k => k.Wafers).WithOne(e => e.CodeProduct).HasForeignKey(fk => fk.CodeProductId);
        }
    }
}