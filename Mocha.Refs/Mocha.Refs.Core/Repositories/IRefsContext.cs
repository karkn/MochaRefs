using Mocha.Refs.Core.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocha.Refs.Core.Repositories
{
    public interface IRefsContext: IDisposable
    {
        IDbSet<RefList> RefLists { get; }
        IDbSet<Ref> Refs { get; }
        IDbSet<RefListStatistics> RefListStatistics { get; }

        IDbSet<Tag> Tags { get; }
        IDbSet<TagUse> TagUses { get; }
        IDbSet<TagUseStatistics> TagUseStatistics { get; }

        IDbSet<User> Users { get; }
        IDbSet<UserData> UserData { get; }
        IDbSet<UserLogin> UserLogins { get; }
        //IDbSet<MochaUser> MochaUsers { get; }

        IDbSet<Favorite> Favorites { get; }

        int SaveChanges();
        Task<int> SaveChangesAsync();

        DbContextTransaction BeginTransaction();
        DbContextTransaction BeginTransaction(IsolationLevel isolationLevel);

        bool IsModified<TEntity>(TEntity entity) where TEntity : class;
        void MarkAsModified<TEntity>(TEntity entity) where TEntity : class;

        Task<int> ExecuteSqlCommandAsync(string sql, params object[] parameters);
        int ExecuteSqlCommand(string sql, params object[] parameters);
    }
}
