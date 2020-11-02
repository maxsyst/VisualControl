using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VueExample.Models;

namespace VueExample.Entities.Configurations
{
    public class WaferConfiguration : IEntityTypeConfiguration<Wafer>
    {
        public void Configure(EntityTypeBuilder<Wafer> builder)
        {
            builder.HasOne(k => k.Parcel).WithMany(e => e.Wafers).HasForeignKey(fk => fk.ParcelId);
        }
    }
}