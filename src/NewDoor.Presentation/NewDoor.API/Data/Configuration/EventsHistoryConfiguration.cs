   using NewDoor.Platform.Entities;
   using DoWhatta.Platform.Data.Marker;
   using Microsoft.EntityFrameworkCore;
   using Microsoft.EntityFrameworkCore.Metadata.Builders;

   namespace NewDoor.API.Data.Configuration
   {
       public class EventsHistoryConfig : IEntityTypeConfiguration<EventsHistory> , IProductDbContextMarker
       {
           public void Configure(EntityTypeBuilder<EventsHistory> builder)
           {
               builder.ToTable("EventsHistory");

               builder.HasKey(x => x.Id);

               builder.Property(x => x.Id)
                   .ValueGeneratedOnAdd();

               builder.Property(x => x.EventId)
                   .IsRequired();

               builder.Property(x => x.DeviceId)
                   .IsRequired(false);

               builder.Property(x => x.EventType)
                   .IsRequired()
                   .HasMaxLength(50);

               builder.Property(x => x.Severity)
                   .HasMaxLength(20);

               builder.Property(x => x.ProcessingResult)
                   .HasMaxLength(500);

               builder.Property(x => x.ProcessorName)
                   .HasMaxLength(100);

               builder.Property(x => x.Remarks)
                   .HasMaxLength(1000);

               builder.Property(x => x.ProcessedUtc)
                   .IsRequired();

               // Relationships
               builder.HasOne(x => x.Event)
                   .WithMany(e => e.EventsHistories)
                   .HasForeignKey(x => x.EventId)
                   .OnDelete(DeleteBehavior.Cascade);

               builder.HasOne(x => x.Device)
                   .WithMany()
                   .HasForeignKey(x => x.DeviceId)
                   .OnDelete(DeleteBehavior.Restrict)
                   .IsRequired(false);

               // Indexes
               builder.HasIndex(x => x.EventId);
               builder.HasIndex(x => x.DeviceId);
               builder.HasIndex(x => x.EventType);
               builder.HasIndex(x => x.ProcessedUtc);
               builder.HasIndex(x => new { x.EventId, x.ProcessedUtc });
           }
       }
   }