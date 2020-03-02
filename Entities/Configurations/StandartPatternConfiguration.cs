using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace VueExample.Entities.Configurations
{
    public class StandartPatternConfiguration : IEntityTypeConfiguration<StandartPatternEntity>
    {
        public void Configure(EntityTypeBuilder<StandartPatternEntity> builder)
        {
            builder.HasOne(k => k.DieType).WithMany().HasForeignKey(fk => fk.DieTypeId);
            builder.HasMany(k => k.StandartMeasurementPatterns).WithOne().HasPrincipalKey(pk => pk.Id);
        }
    }
}