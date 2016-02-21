using Mocha.Refs.Core.Entities;
using Mocha.Refs.Core.Handlers;
using Mocha.Refs.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocha.Refs.Core.Handlers
{
    public class UserHandler: IUserHandler
    {
        private IRefsContext _refsContext;

        public UserHandler(IRefsContext refsContext)
        {
            _refsContext = refsContext;
        }

        /// <summary>
        /// Mochaユーザを取得します。
        /// </summary>
        //public async Task<MochaUser> GetMochaUserAsync(Guid mochaUserId)
        //{
        //    var query =
        //        from u in _refsContext.MochaUsers.AsNoTracking()
        //        where u.Id == mochaUserId
        //        select u;

        //    var user = await query.SingleAsync();
        //    return user;
        //}

        public User GetUser(long userId)
        {
            var query = _refsContext.Users.AsNoTracking().Where(u => u.Id == userId);
            return query.SingleOrDefault();
        }

        public async Task<User> GetUserByUserNameAsync(string userName)
        {
            var query =
                from u in _refsContext.Users.AsNoTracking()
                where u.UserName == userName
                select u;

            var user = await query.SingleOrDefaultAsync();
            return user;
        }

        /// <summary>
        /// ユーザを取得します。
        /// </summary>
        //public async Task<User> GetUserByMochaUserIdAsync(Guid mochaUserId)
        //{
        //    var query =
        //        from u in _refsContext.Users.AsNoTracking()
        //        where u.MochaUserId == mochaUserId
        //        select u;

        //    var user = await query.SingleAsync();
        //    return user;
        //}

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            var query =
                from u in _refsContext.Users.AsNoTracking()
                select u;

            var users = await query.ToArrayAsync();
            return users;
        }

        public async Task<User> GetUserAsync(long userId)
        {
            var query =
                from u in _refsContext.Users.AsNoTracking()
                where u.Id == userId
                select u;

            var user = await query.SingleAsync();
            return user;
        }

        //public User GetUserByMochaUserId(Guid mochaUserId)
        //{
        //    var query = _refsContext.Users.AsNoTracking().
        //        Where(u => u.MochaUserId == mochaUserId);
            
        //    var user = query.Single();
        //    return user;
        //}

        //public async Task<bool> ExistsUserByMochaUserIdAsync(Guid mochaUserId)
        //{
        //    var query = _refsContext.Users.AsNoTracking();
        //    var ret = await query.AnyAsync(u => u.MochaUserId == mochaUserId);
        //    return ret;
        //}

        //public async Task<bool> ExistsMochaUserByEmailAsync(string email)
        //{
        //    var query = _refsContext.MochaUsers.AsNoTracking();
        //    return await query.AnyAsync(u => u.Email == email);
        //}

        //public async Task<bool> ExistsMochaUserByUserNameAsync(string userName)
        //{
        //    var query = _refsContext.MochaUsers.AsNoTracking();
        //    return await query.AnyAsync(u => u.UserName == userName);
        //}

        public async Task<bool> ExistsUserByUserNameAsync(string userName)
        {
            var query = _refsContext.Users.AsNoTracking();
            return await query.AnyAsync(u => u.UserName == userName);
        }

        //public async Task<User> CreateUserByMochaUserIdAsync(Guid mochaUserId)
        //{
        //    var mochaUser = await GetMochaUserAsync(mochaUserId);
        //    var user = new User()
        //    {
        //        UserName = mochaUser.UserName,
        //        Email = mochaUser.Email,
        //        MochaUserId = mochaUser.Id,
        //        DisplayName = mochaUser.DisplayName,
        //    };
        //    _refsContext.Users.Add(user);
        //    await _refsContext.SaveChangesAsync();

        //    return user;
        //}

        public async Task<User> UpdateUserAsync(User user)
        {
            var query = _refsContext.Users.Where(u => u.Id == user.Id);
            var storedUser = await query.SingleAsync();
            storedUser.Email = user.Email;
            storedUser.DisplayName = user.DisplayName;
            await _refsContext.SaveChangesAsync();
            return storedUser;
        }

        public async Task<UserData> GetUserDataAsync(long userId, string key)
        {
            var query = _refsContext.UserData.Include("User").AsNoTracking().
                Where(d => d.UserId == userId && d.Key == key);
            var storedUserData = await query.SingleOrDefaultAsync();
            return storedUserData;
        }

        public async Task AddUserDataAsync(long userId, string key, string value)
        {
            var storedUser = _refsContext.Users.Find(userId);
            var data = new UserData()
            {
                Key = key,
                Value = value,
            };
            storedUser.UserData.Add(data);
            await _refsContext.SaveChangesAsync();
        }
    }
}
