using DoWhatta.Platform.Data.Marker;
using NewDoor.Platform.Entities;
   using Microsoft.EntityFrameworkCore;
   using Microsoft.EntityFrameworkCore.Metadata.Builders;

   namespace NewDoor.API.Data.Configuration
   {
       public class BuildingConfig : IEntityTypeConfiguration<Building> , IProductDbContextMarker
       {
           public void Configure(EntityTypeBuilder<Building> builder)
           {
                builder.ToTable("Building");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id);

        builder.Property(x => x.BuildingCode);

        builder.Property(x => x.Name);

        builder.Property(x => x.Address);

        builder.Property(x => x.Status);

        builder.Property(x => x.TotalDevices);

        builder.Property(x => x.OnlineDevices);

        builder.Property(x => x.OfflineDevices);

        builder.Property(x => x.ActiveAlarms);

        builder.Property(x => x.CreatedOn);

        builder.Property(x => x.UpdatedOn);
           }
       }
   }