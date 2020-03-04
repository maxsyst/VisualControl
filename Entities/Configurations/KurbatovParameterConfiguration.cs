using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace VueExample.Entities.Configurations
{
    public class KurbatovParameterConfiguration : IEntityTypeConfiguration<KurbatovParameterEntity>
    {
        public void Configure(EntityTypeBuilder<KurbatovParameterEntity> builder)
        {
            builder.HasOne(k => k.StandartParameterEntity).WithMany().HasForeignKey(fk => fk.StandartParameterId);
            builder.HasOne(k => k.StandartMeasurementPatternEntity).WithMany(k => k.KurbatovParameters).HasForeignKey(fk => fk.SmpId);
            builder.HasOne(k => k.KurbatovParameterBordersEntity).WithMany(k => k.KurbatovParameters).HasForeignKey(fk => fk.BordersId);
        }
    }
}