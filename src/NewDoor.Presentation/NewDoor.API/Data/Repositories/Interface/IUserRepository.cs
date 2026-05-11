using DoWhatta.Platform.Core.DependencyInjection;
using DoWhatta.Platform.Data.Base;
using DoWhatta.Platform.Entities;

namespace NewDoor.API.Data.Repositories.Interface
{
    public interface IUserRepository:IBaseRepository<UserDetail>,IscopedService
    {
        public Task<UserDetail?> GetUserDetail(string? PhoneNumber);
        
        public Task<UserDetail?> FindUserByEmailOrPhoneNumber(string password, string? email, long? PhoneNumber);
    }
}
