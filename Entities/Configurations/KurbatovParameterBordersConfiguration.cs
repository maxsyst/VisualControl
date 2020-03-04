using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace VueExample.Entities.Configurations
{
    public class KurbatovParameterBordersConfiguration : IEntityTypeConfiguration<KurbatovParameterBordersEntity>
    {
        public void Configure(EntityTypeBuilder<KurbatovParameterBordersEntity> builder)
        {
            builder.HasMany(k => k.KurbatovParameters).WithOne();
        }
    }
}