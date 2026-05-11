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
           }
       }
   }