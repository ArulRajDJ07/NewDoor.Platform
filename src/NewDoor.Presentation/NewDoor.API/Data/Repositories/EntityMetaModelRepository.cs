using NewDoor.API.Data.Repositories.Interface;
using DoWhatta.Platform.Data.Base;
using DoWhatta.Platform.Entities;
using Microsoft.EntityFrameworkCore;

namespace NewDoor.API.Data.Repositories
{
    public class EntityMetaModelRepository(DoWhattaDBContext dbContext)
     : BaseRepository<EntityMetaModel>(dbContext), IEntityMetaModelRepository
    {
        public async Task<EntityMetaModel?> GetWithPropertiesAsync(long id)
        {
            return await DbSet.Include(e => e.Properties).FirstOrDefaultAsync(e => e.Id == id);
        }
    }
}
