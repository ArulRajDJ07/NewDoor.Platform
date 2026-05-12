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
           }
       }
   }