using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VueExample.ResponseObjects;

namespace VueExample.Entities.Configurations
{
    public class PWaferConfiguration : IEntityTypeConfiguration<PWafer>
    {
        public void Configure(EntityTypeBuilder<PWafer> builder)
        {
            builder.HasNoKey();
        }
    }
}