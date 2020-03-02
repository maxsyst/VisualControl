using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace VueExample.Entities.Configurations
{
    public class StandartMeasurementPatternConfiguration : IEntityTypeConfiguration<StandartMeasurementPatternEntity>
    {
        public void Configure(EntityTypeBuilder<StandartMeasurementPatternEntity> builder)
        {
            builder.HasOne(k => k.Divider).WithMany().HasForeignKey(fk => fk.DividerId);
            builder.HasOne(k => k.Stage).WithMany().HasForeignKey(fk => fk.StageId);
            builder.HasOne(k => k.Element).WithMany().HasForeignKey(fk => fk.ElementId);
            builder.HasOne(k => k.StandartPattern).WithMany().HasForeignKey(fk => fk.PatternId);
        }
    }
}