using Mocha.Refs.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocha.Refs.Core.Handlers
{
    public interface IUserHandler
    {
        Task<IEnumerable<User>> GetAllUsersAsync();

        //User GetUserByMochaUserId(Guid mochaUserId);
        User GetUser(long userId);

        Task<User> GetUserAsync(long userId);
        Task<User> GetUserByUserNameAsync(string userName);

        //Task<MochaUser> GetMochaUserAsync(Guid mochaUserId);
        //Task<User> GetUserByMochaUserIdAsync(Guid mochaUserId);

        Task<bool> ExistsUserByUserNameAsync(string userName);
        //Task<bool> ExistsMochaUserByEmailAsync(string email);
        //Task<bool> ExistsUserByMochaUserIdAsync(Guid mochaUserId);
        //Task<bool> ExistsMochaUserByUserNameAsync(string userName);

        //Task<User> CreateUserByMochaUserIdAsync(Guid mochaUserId);
        Task<User> UpdateUserAsync(User user);

        Task<UserData> GetUserDataAsync(long userId, string key);
        Task AddUserDataAsync(long userId, string key, string value);
    }
}
