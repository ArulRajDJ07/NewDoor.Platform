   using NewDoor.Platform.Entities;
   using DoWhatta.Platform.Data.Marker;
   using Microsoft.EntityFrameworkCore;
   using Microsoft.EntityFrameworkCore.Metadata.Builders;

   namespace NewDoor.API.Data.Configuration
   {
       public class DeviceRuntimeStatusConfig : IEntityTypeConfiguration<DeviceRuntimeStatus> , IProductDbContextMarker
       {
           public void Configure(EntityTypeBuilder<DeviceRuntimeStatus> builder)
           {
   builder.ToTable("DeviceRuntimeStatus");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id);

        builder.Property(x => x.DeviceId);

        builder.Property(x => x.BuildingId);

        builder.Property(x => x.DeviceType);

        builder.Property(x => x.CurrentStatus);

        builder.Property(x => x.IsOnline);

        builder.Property(x => x.LastHeartbeatUtc);

        builder.Property(x => x.LastSeenUtc);

        builder.Property(x => x.ConsecutiveFailures);

        builder.Property(x => x.CurrentTemperature);

        builder.Property(x => x.LastEventType);

        builder.Property(x => x.LastEventUtc);

        builder.Property(x => x.ActiveAlarmCount);

        builder.Property(x => x.SignalStrength);

        builder.Property(x => x.BatteryLevel);

        builder.Property(x => x.StatusChangedUtc);

        builder.Property(x => x.UpdatedOn);
           }
       }
   }