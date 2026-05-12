   using NewDoor.Platform.Entities;
   using DoWhatta.Platform.Data.Marker;
   using Microsoft.EntityFrameworkCore;
   using Microsoft.EntityFrameworkCore.Metadata.Builders;

   namespace NewDoor.API.Data.Configuration
   {
       public class DeviceConfig : IEntityTypeConfiguration<Device> , IProductDbContextMarker
       {
           public void Configure(EntityTypeBuilder<Device> builder)
           {
   builder.ToTable("Device");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id);

        builder.Property(x => x.DeviceId);

        builder.Property(x => x.DeviceName);

        builder.Property(x => x.DeviceType);

        builder.Property(x => x.BuildingId);

        builder.Property(x => x.Floor);

        builder.Property(x => x.Zone);

        builder.Property(x => x.FirmwareVersion);

        builder.Property(x => x.Status);

        builder.Property(x => x.CreatedOn);

        builder.Property(x => x.UpdatedOn);
           }
       }
   }