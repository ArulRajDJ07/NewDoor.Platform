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

               builder.Property(x => x.Id)
                   .ValueGeneratedOnAdd();

               builder.Property(x => x.RuleId)
                   .IsRequired();

               builder.Property(x => x.ConfigKey)
                   .IsRequired()
                   .HasMaxLength(50);

               builder.Property(x => x.ConfigValue)
                   .IsRequired()
                   .HasMaxLength(200);

               builder.Property(x => x.Unit)
                   .HasMaxLength(50);

               builder.Property(x => x.IsActive)
                   .HasDefaultValue(true);

               // Indexes
               builder.HasIndex(x => x.RuleId);
               builder.HasIndex(x => x.IsActive);
               builder.HasIndex(x => new { x.RuleId, x.IsActive });
           }
       }
   }