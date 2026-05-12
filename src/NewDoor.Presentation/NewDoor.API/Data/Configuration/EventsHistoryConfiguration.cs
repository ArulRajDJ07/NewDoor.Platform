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
           }
       }
   }