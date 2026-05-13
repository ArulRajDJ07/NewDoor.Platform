   using NewDoor.Platform.Entities;
   using DoWhatta.Platform.Data.Marker;
   using Microsoft.EntityFrameworkCore;
   using Microsoft.EntityFrameworkCore.Metadata.Builders;

   namespace NewDoor.API.Data.Configuration
   {
       public class AlarmConfig : IEntityTypeConfiguration<Alarm> , IProductDbContextMarker
       {
           public void Configure(EntityTypeBuilder<Alarm> builder)
           {
               builder.ToTable("Alarm");

               builder.HasKey(x => x.Id);

               builder.Property(x => x.Id)
                   .ValueGeneratedOnAdd();

               builder.Property(x => x.AlarmCode)
                   .IsRequired()
                   .HasMaxLength(100);

               builder.Property(x => x.DeviceId)
                   .IsRequired();

               builder.Property(x => x.BuildingId)
                   .IsRequired();

               builder.Property(x => x.RuleId)
                   .IsRequired();

               builder.Property(x => x.IncidentId);

               builder.Property(x => x.Severity)
                   .HasMaxLength(20);

               builder.Property(x => x.AlarmMessage)
                   .HasMaxLength(500);

               builder.Property(x => x.AlarmStatus)
                   .HasMaxLength(20);

               builder.Property(x => x.TriggeredUtc)
                   .IsRequired();

               builder.Property(x => x.AcknowledgedUtc);

               builder.Property(x => x.ResolvedUtc);

               builder.Property(x => x.TriggeredBy)
                   .HasMaxLength(200);

               builder.Property(x => x.ResolutionNotes)
                   .HasMaxLength(1000);

               // Relationships
               builder.HasOne(x => x.Device)
                   .WithMany()
                   .HasForeignKey(x => x.DeviceId)
                   .OnDelete(DeleteBehavior.Restrict);

               builder.HasOne(x => x.Building)
                   .WithMany()
                   .HasForeignKey(x => x.BuildingId)
                   .OnDelete(DeleteBehavior.Restrict);

               builder.HasOne(x => x.Rule)
                   .WithMany()
                   .HasForeignKey(x => x.RuleId)
                   .OnDelete(DeleteBehavior.Restrict);

               builder.HasOne(x => x.Incident)
                   .WithMany(i => i.Alarms)
                   .HasForeignKey(x => x.IncidentId)
                   .OnDelete(DeleteBehavior.SetNull);

               // Indexes
               builder.HasIndex(x => x.AlarmCode).IsUnique();
               builder.HasIndex(x => x.DeviceId);
               builder.HasIndex(x => x.BuildingId);
               builder.HasIndex(x => x.RuleId);
               builder.HasIndex(x => x.IncidentId);
               builder.HasIndex(x => x.AlarmStatus);
               builder.HasIndex(x => x.TriggeredUtc);
               builder.HasIndex(x => new { x.DeviceId, x.AlarmStatus });
               builder.HasIndex(x => new { x.BuildingId, x.AlarmStatus });
           }
       }
   }