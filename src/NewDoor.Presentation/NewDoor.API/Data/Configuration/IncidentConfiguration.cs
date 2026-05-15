   using NewDoor.Platform.Entities;
   using DoWhatta.Platform.Data.Marker;
   using Microsoft.EntityFrameworkCore;
   using Microsoft.EntityFrameworkCore.Metadata.Builders;

   namespace NewDoor.API.Data.Configuration
   {
       public class IncidentConfig : IEntityTypeConfiguration<Incident> , IProductDbContextMarker
       {
           public void Configure(EntityTypeBuilder<Incident> builder)
           {
               builder.ToTable("Incident");

               builder.HasKey(x => x.Id);

               builder.Property(x => x.Id)
                   .ValueGeneratedOnAdd();

               builder.Property(x => x.IncidentCode)
                   .IsRequired()
                   .HasMaxLength(100);

               builder.Property(x => x.BuildingId)
                   .IsRequired();

               builder.Property(x => x.DeviceId)
                   .HasMaxLength(100);

               builder.Property(x => x.IncidentType)
                   .IsRequired()
                   .HasMaxLength(50);

               builder.Property(x => x.Severity)
                   .HasMaxLength(20);

               builder.Property(x => x.Status)
                   .HasMaxLength(20);

               builder.Property(x => x.StartedUtc)
                   .IsRequired();

               builder.Property(x => x.EndedUtc);

               builder.Property(x => x.Summary)
                   .HasMaxLength(1000);

               builder.Property(x => x.RootCause)
                   .HasMaxLength(500);

               builder.Property(x => x.TriggeredByRule);

               builder.Property(x => x.EventCount);

               // Relationships
               builder.HasOne(x => x.Building)
                   .WithMany()
                   .HasForeignKey(x => x.BuildingId)
                   .OnDelete(DeleteBehavior.Restrict);

               // Indexes
               builder.HasIndex(x => x.IncidentCode).IsUnique();
               builder.HasIndex(x => x.DeviceId);
               builder.HasIndex(x => x.BuildingId);
               builder.HasIndex(x => x.IncidentType);
               builder.HasIndex(x => x.Status);
               builder.HasIndex(x => x.StartedUtc);
               builder.HasIndex(x => new { x.BuildingId, x.Status });
           }
       }
   }