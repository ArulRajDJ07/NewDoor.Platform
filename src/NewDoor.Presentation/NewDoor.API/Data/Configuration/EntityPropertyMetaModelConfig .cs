using DoWhatta.Platform.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using DoWhatta.Platform.Data.Marker;

namespace NewDoor.API.Data.Configuration
{
    public class EntityPropertyMetaModelConfig : IEntityTypeConfiguration<EntityPropertyMetaModel>, IPlatformDbContextMarker
    {
        public void Configure(EntityTypeBuilder<EntityPropertyMetaModel> builder)
        {
            builder.ToTable("EntityPropertyMetaModel");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.ProductId);
            builder.Property(x => x.EntityMetaModelId);
            builder.Property(x => x.PropertyName);
            builder.Property(x => x.PropertyType);
            builder.Property(x => x.IsRequired);

        }
    }
}
