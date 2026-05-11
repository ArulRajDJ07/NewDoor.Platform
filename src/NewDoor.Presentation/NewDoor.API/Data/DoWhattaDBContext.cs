using DoWhatta.Platform.Core.Common;
using DoWhatta.Platform.Data.Base;
using DoWhatta.Platform.Data.Common;
using DoWhatta.Platform.Data.Marker;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace NewDoor.API.Data
{
    public class DoWhattaDBContext : ApplicationDbContext
    {
        private readonly IUserContextProvider _currentContextProvider;

        public DoWhattaDBContext(
            DbContextOptions<DoWhattaDBContext> options,
            IUserContextProvider currentContextProvider)
            : base(options)
        {
            _currentContextProvider = currentContextProvider;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var markerType = typeof(IPlatformDbContextMarker);

            modelBuilder.ApplyConfigurationsFromAssembly(
                Assembly.GetExecutingAssembly(),
                type =>
                    type.GetInterfaces().Any(i =>
                        i.IsGenericType &&
                        i.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>)) &&
                    markerType.IsAssignableFrom(type)
            );
        }

        public override int SaveChanges()
        {
            ApplyAudit();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            ApplyAudit();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void ApplyAudit()
        {
            var entries = ChangeTracker.Entries()
                .Where(e =>
                    e.State == EntityState.Added ||
                    e.State == EntityState.Modified ||
                    e.State == EntityState.Deleted);

            var user = _currentContextProvider.GetUserIdentity();
            if (user == null) return;

            foreach (var entry in entries)
            {
                entry.Property(DatabaseConstants.ModifiedBy).CurrentValue = user.Id;
                entry.Property(DatabaseConstants.ModifiedAt).CurrentValue = DateTimeOffset.UtcNow;
            }
        }
    }
}
