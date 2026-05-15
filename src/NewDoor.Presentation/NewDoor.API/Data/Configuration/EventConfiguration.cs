   using NewDoor.Platform.Entities;
   using DoWhatta.Platform.Data.Marker;
   using Microsoft.EntityFrameworkCore;
   using Microsoft.EntityFrameworkCore.Metadata.Builders;

   namespace NewDoor.API.Data.Configuration
   {
       public class EventConfig : IEntityTypeConfiguration<Event> , IProductDbContextMarker
       {
           public void Configure(EntityTypeBuilder<Event> builder)
           {
               builder.ToTable("Event");

               builder.HasKey(x => x.Id);

               builder.Property(x => x.Id)
                   .ValueGeneratedOnAdd();

               builder.Property(x => x.EventId)
                   .IsRequired()
                   .HasMaxLength(100);

               builder.Property(x => x.DeviceId)
                   .HasMaxLength(100);

               builder.Property(x => x.BuildingId)
                   .IsRequired();

               builder.Property(x => x.EventType)
                   .IsRequired()
                   .HasMaxLength(50);

               builder.Property(x => x.Temperature);

               builder.Property(x => x.SmokeLevel);

               builder.Property(x => x.BatteryLevel);

               builder.Property(x => x.SignalStrength);

               builder.Property(x => x.Payload)
                   .HasMaxLength(200);

               builder.Property(x => x.Severity)
                   .HasMaxLength(20);

               builder.Property(x => x.EventUtc)
                   .IsRequired();

               builder.Property(x => x.ProcessedUtc)
                   .IsRequired();

               builder.Property(x => x.Status)
                   .HasMaxLength(20);

               builder.Property(x => x.CorrelationId)
                   .HasMaxLength(100);

               // Relationships
               builder.HasOne(x => x.Building)
                   .WithMany()
                   .HasForeignKey(x => x.BuildingId)
                   .OnDelete(DeleteBehavior.Restrict);

               // Indexes
               builder.HasIndex(x => x.EventId).IsUnique();
               builder.HasIndex(x => x.DeviceId);
               builder.HasIndex(x => x.BuildingId);
               builder.HasIndex(x => x.EventType);
               builder.HasIndex(x => x.EventUtc);
               builder.HasIndex(x => x.CorrelationId);
               builder.HasIndex(x => new { x.DeviceId, x.EventUtc });
           }
       }
   }