using DoWhatta.Platform.Data.Marker;
using DoWhatta.Platform.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NewDoor.API.Data.Configuration
{
    public class UserConfig : IEntityTypeConfiguration<UserDetail>, IPlatformDbContextMarker
    {
        public void Configure(EntityTypeBuilder<UserDetail> builder)
        {
            builder.ToTable("UserDetail");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.ModifiedBy);
            builder.Property(x => x.OrganizationID);
            builder.Property(x => x.UserName);
            builder.Property(x => x.RoleId);
            builder.Property(x => x.PhoneNumber).HasMaxLength(LengthConstants.NameLength);
            builder.Property(x => x.Password).HasMaxLength(LengthConstants.NameLength);
            builder.Property(x => x.Verifcation).HasMaxLength(LengthConstants.NameLength);
            builder.Property(x => x.IsActive);
        }
    }
}
