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
           }
       }
   }