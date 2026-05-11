   using NewDoor.Platform.Entities;
   using DoWhatta.Platform.Data.Marker;
   using Microsoft.EntityFrameworkCore;
   using Microsoft.EntityFrameworkCore.Metadata.Builders;

   namespace NewDoor.API.Data.Configuration
   {
       public class RuleConfigurationConfig : IEntityTypeConfiguration<RuleConfiguration> , IProductDbContextMarker
       {
           public void Configure(EntityTypeBuilder<RuleConfiguration> builder)
           {
   builder.ToTable("RuleConfiguration");

        builder.HasKey(x => x.Id);
           }
       }
   }