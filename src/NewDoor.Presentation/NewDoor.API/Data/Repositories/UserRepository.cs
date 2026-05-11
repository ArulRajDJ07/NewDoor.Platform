using NewDoor.API.Data.Repositories.Interface;
using DoWhatta.Platform.Data.Base;
using DoWhatta.Platform.Entities;
using Microsoft.EntityFrameworkCore;

namespace NewDoor.API.Data.Repositories
{
    public class UserRepository(DoWhattaDBContext applicationDbContext) : BaseRepository<UserDetail>(applicationDbContext), IUserRepository
    {
        public async Task<UserDetail?> GetUserDetail(string? PhoneNumber)
        {
            return await DbSet.AsNoTracking().FirstOrDefaultAsync(x => string.Equals(x.PhoneNumber, PhoneNumber));
        }
        public async Task<UserDetail?> FindUserByEmailOrPhoneNumber(string password, string? email, long? PhoneNumber)
        {
            UserDetail? userDetail = default!;
            if (!string.IsNullOrWhiteSpace(email))
            {
                userDetail = await DbSet.Where(predicate: user =>
                 user.UserName== email
                && (user.Password == password)).FirstOrDefaultAsync();
            }
            else if (PhoneNumber.HasValue)
            {
                userDetail = await DbSet.Where(predicate: user => user.PhoneNumber == PhoneNumber && (user.Password == password))
                                  .FirstOrDefaultAsync();
            }
            return userDetail;
        }
    }
}
