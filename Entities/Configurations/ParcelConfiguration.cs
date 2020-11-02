using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VueExample.Models.SRV6;

namespace VueExample.Entities.Configurations
{
    public class ParcelConfiguration : IEntityTypeConfiguration<Parcel>
    {
        public void Configure(EntityTypeBuilder<Parcel> builder)
        {
            builder.HasMany(k => k.Wafers).WithOne(e => e.Parcel).HasForeignKey(fk => fk.ParcelId);
        }
    }
}