using NewDoor.API.Data.Repositories.Interface;
using DoWhatta.Platform.Data.Base;
using DoWhatta.Platform.DTO.Features.MetaModel;
using DoWhatta.Platform.Entities;
using Microsoft.Azure.Amqp.Framing;
using Microsoft.EntityFrameworkCore;

namespace NewDoor.API.Data.Repositories
{
    public class EntityPropertyMetaModelRepository(DoWhattaDBContext dbContext)
        : BaseRepository<EntityPropertyMetaModel>(dbContext), IEntityPropertyMetaModelRepository
    {
        private readonly DoWhattaDBContext _context = dbContext;
       
        public async Task<int> AddRangeAsync(ICollection<EntityPropertyMetaModel> properties)
        {
            await DbSet.AddRangeAsync(properties);
            return await _context.SaveChangesAsync();
        }

        public async Task<List<EntityPropertyMetaModel>> GetallProperties(int EntityID)
        {
            return await DbSet.AsNoTracking().Where(x => x.EntityMetaModelId == EntityID).ToListAsync();
        }
    }
}

