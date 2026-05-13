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

               builder.Property(x => x.Id)
                   .ValueGeneratedOnAdd();

               builder.Property(x => x.RuleCode)
                   .IsRequired()
                   .HasMaxLength(50);

               builder.Property(x => x.RuleName)
                   .IsRequired()
                   .HasMaxLength(100);

               builder.Property(x => x.RuleType)
                   .IsRequired()
                   .HasMaxLength(50);

               builder.Property(x => x.DeviceType)
                   .IsRequired()
                   .HasMaxLength(50);

               builder.Property(x => x.ThresholdValue)
                   .IsRequired();

               builder.Property(x => x.WindowSeconds)
                   .IsRequired();

               builder.Property(x => x.Severity)
                   .HasMaxLength(20);

               builder.Property(x => x.IsActive)
                   .HasDefaultValue(true);

               builder.Property(x => x.Description)
                   .HasMaxLength(500);

               // Relationship
               builder.HasMany(x => x.RuleConfigurations)
                   .WithOne(x => x.Rule)
                   .HasForeignKey(x => x.RuleId)
                   .OnDelete(DeleteBehavior.Cascade);

               // Indexes
               builder.HasIndex(x => x.RuleCode).IsUnique();
               builder.HasIndex(x => x.DeviceType);
               builder.HasIndex(x => x.IsActive);
               builder.HasIndex(x => new { x.DeviceType, x.IsActive });
           }
       }
   }