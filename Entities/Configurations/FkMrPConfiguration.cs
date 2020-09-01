using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace VueExample.Entities.Configurations
{
    public class FkMrPConfiguration : IEntityTypeConfiguration<FkMrP>
    {    
        public void Configure(EntityTypeBuilder<FkMrP> builder)
        {
            builder.HasOne(x=> x.MeasurementRecording).WithMany(f => f.FkMrPs).HasForeignKey(fk => fk.MeasurementRecordingId);
        }
    }
}