using DoWhatta.Platform.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using DoWhatta.Platform.Data.Marker;

namespace NewDoor.API.Data.Configuration
{
    public class EntityMetaModelConfig : IEntityTypeConfiguration<EntityMetaModel>, IPlatformDbContextMarker
    {
        public void Configure(EntityTypeBuilder<EntityMetaModel> builder)
        {
            builder.ToTable("EntityMetaModel");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.ProductId);
            builder.Property(x => x.FeatureId);
            builder.Property(x => x.PageID);
            builder.Property(x => x.SectionId);
            builder.Property(x => x.EntityName);
            builder.Property(x => x.PrimaryKey);
            builder.Property(x => x.TableName);
            builder.Property(x => x.Description);
            builder.Property(x => x.DatabaseMarker);
            builder.Property(x => x.CreatedAt);
            builder.Property(x => x.ModifiedAt);
            builder.Property(x => x.ModifiedBy);

            
        }
    }
}
