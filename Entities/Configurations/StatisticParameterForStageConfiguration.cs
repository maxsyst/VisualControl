using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VueExample.Models.SRV6;

namespace VueExample.Entities.Configurations
{
    public class StatisticParameterForStageConfiguration : IEntityTypeConfiguration<StatParameterForStage>
    {
        public void Configure(EntityTypeBuilder<StatParameterForStage> builder)
        {
            builder.HasOne(x => x.StatisticParameter).WithMany(x => x.StatParameterForStages).HasForeignKey(x => x.StatisticParameterId);
            builder.HasOne(x => x.Stage).WithMany(x => x.StatParameterForStages).HasForeignKey(x => x.StageId);
        }
    }
}