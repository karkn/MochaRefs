using Microsoft.AspNet.Identity;
using Mocha.Refs.Core.Entities;
using Mocha.Refs.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocha.Refs.Core.Auth
{
    public class UserStore: IUserStore<IdentityUser>, IUserLoginStore<IdentityUser>, IUserSecurityStampStore<IdentityUser>
    {
        private IRefsContext _refsContext;

        public UserStore(IRefsContext refsContext)
        {
            _refsContext = refsContext;
        }

        #region IUserStore<IdentityUser> メンバー

        public async Task CreateAsync(IdentityUser user)
        {
            _refsContext.Users.Add(user.User);
            await _refsContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(IdentityUser user)
        {
            _refsContext.Users.Remove(user.User);
            await _refsContext.SaveChangesAsync();
        }

        public async Task<IdentityUser> FindByIdAsync(string userId)
        {
            var id = Convert.ToInt64(userId);
            var set = _refsContext.Users as DbSet<User>;
            if (set == null)
            {
                var found = _refsContext.Users.Find(id);
                return ToIdentityUser(found);
            }
            else
            {
                var found = await set.FindAsync(id);
                return ToIdentityUser(found);
            }
        }

        public async Task<IdentityUser> FindByNameAsync(string userName)
        {
            var query = _refsContext.Users.Where(u => u.UserName == userName);
            var found = await query.SingleOrDefaultAsync();
            return ToIdentityUser(found);
        }

        public async Task UpdateAsync(IdentityUser user)
        {
            //_refsContext.MarkAsModified(user);
            await _refsContext.SaveChangesAsync();
        }

        #endregion

        #region IDisposable メンバー

        public void Dispose()
        {
            if (_refsContext != null)
            {
                _refsContext.Dispose();
            }
        }

        #endregion

        #region IUserLoginStore<IdentityUser> メンバー

        public Task AddLoginAsync(IdentityUser user, UserLoginInfo login)
        {
            var userLogin = new UserLogin()
            {
                User = user.User,
                LoginProvider = login.LoginProvider,
                ProviderKey = login.ProviderKey,
            };
            _refsContext.UserLogins.Add(userLogin);
            return Task.FromResult(0);
        }

        public async Task<IdentityUser> FindAsync(UserLoginInfo login)
        {
            var query = _refsContext.UserLogins.Include(l => l.User).
                Where(l => l.LoginProvider == login.LoginProvider && l.ProviderKey == login.ProviderKey).
                Select(l => l.User);
            var found = await query.SingleOrDefaultAsync();
            return ToIdentityUser(found);
        }

        public async Task<IList<UserLoginInfo>> GetLoginsAsync(IdentityUser user)
        {
            var query = _refsContext.UserLogins.Include(l => l.User).
                Where(l => l.UserId == user.User.Id).
                Select(l => new UserLoginInfo(l.LoginProvider, l.ProviderKey));
            return await query.ToListAsync();
        }

        public Task RemoveLoginAsync(IdentityUser user, UserLoginInfo login)
        {
            var userLogin = user.User.UserLogins.
                Where(l => l.LoginProvider == login.LoginProvider && l.ProviderKey == login.ProviderKey).
                SingleOrDefault();
            if (userLogin != null)
            {
                user.User.UserLogins.Remove(userLogin);
                _refsContext.UserLogins.Remove(userLogin);
            }
            return Task.FromResult(0);
        }

        #endregion

        #region IUserSecurityStampStore<IdentityUser> メンバー

        public Task<string> GetSecurityStampAsync(IdentityUser user)
        {
            return Task.FromResult<string>(user.User.SecurityStamp);
        }

        public Task SetSecurityStampAsync(IdentityUser user, string stamp)
        {
            user.User.SecurityStamp = stamp;
            return Task.FromResult<int>(0);
        }

        #endregion

        private IdentityUser ToIdentityUser(User user)
        {
            return user == null ? null : new IdentityUser(user);
        }

    }
}
