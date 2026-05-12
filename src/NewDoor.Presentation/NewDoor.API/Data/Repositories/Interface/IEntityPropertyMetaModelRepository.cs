using DoWhatta.Platform.Core.DependencyInjection;
using DoWhatta.Platform.Data.Base;
using DoWhatta.Platform.DTO.Features.MetaModel;
using DoWhatta.Platform.Entities;

namespace NewDoor.API.Data.Repositories.Interface
{
    public interface IEntityPropertyMetaModelRepository : IBaseRepository<EntityPropertyMetaModel>, IscopedService
    {
        public Task<List<EntityPropertyMetaModel>> GetallProperties(int EntityID);
        public Task<int> AddRangeAsync(ICollection<EntityPropertyMetaModel> properties);
    }
}
