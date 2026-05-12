using DoWhatta.Platform.Core.DependencyInjection;
using DoWhatta.Platform.Data.Base;
using DoWhatta.Platform.Entities;

namespace NewDoor.API.Data.Repositories.Interface
{
    public interface IEntityMetaModelRepository : IBaseRepository<EntityMetaModel>, IscopedService
    {
        Task<EntityMetaModel?> GetWithPropertiesAsync(long id);
    }
}
