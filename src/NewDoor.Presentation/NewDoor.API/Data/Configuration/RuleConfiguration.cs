   using NewDoor.Platform.Entities;
   using DoWhatta.Platform.Data.Marker;
   using Microsoft.EntityFrameworkCore;
   using Microsoft.EntityFrameworkCore.Metadata.Builders;

   namespace NewDoor.API.Data.Configuration
   {
       public class RuleConfig : IEntityTypeConfiguration<Rule> , IProductDbContextMarker
       {
           public void Configure(EntityTypeBuilder<Rule> builder)
           {
   builder.ToTable("Rule");

        builder.HasKey(x => x.Id);
           }
       }
   }