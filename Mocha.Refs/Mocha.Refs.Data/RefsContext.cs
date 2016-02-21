using Microsoft.Practices.Unity;
using Mocha.Common.Data;
using Mocha.Common.Unity;
using Mocha.Refs.Core;
using Mocha.Refs.Core.Entities;
using Mocha.Refs.Core.Repositories;
using Mocha.Refs.Data.Mapping;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocha.Refs.Data
{
    public class RefsContext: DbContext, IRefsContext
    {
        public IDbSet<RefList> RefLists
        {
            get { return Set<RefList>(); }
        }
        public IDbSet<Ref> Refs
        {
            get { return Set<Ref>(); }
        }
        public IDbSet<RefListStatistics> RefListStatistics
        {
            get { return Set<RefListStatistics>(); }
        }

        public IDbSet<Tag> Tags
        {
            get { return Set<Tag>(); }
        }
        public IDbSet<TagUse> TagUses
        {
            get { return Set<TagUse>(); }
        }
        public IDbSet<TagStatistics> TagStatistics
        {
            get { return Set<TagStatistics>(); }
        }
        public IDbSet<TagUseStatistics> TagUseStatistics
        {
            get { return Set<TagUseStatistics>(); }
        }

        public IDbSet<Favorite> Favorites
        {
            get { return Set<Favorite>(); }
        }

        public IDbSet<User> Users
        {
            get { return Set<User>(); }
        }
        public IDbSet<UserData> UserData
        {
            get { return Set<UserData>(); }
        }
        public IDbSet<UserLogin> UserLogins
        {
            get { return Set<UserLogin>(); }
        }
        //public IDbSet<MochaUser> MochaUsers
        //{
        //    get { return Set<MochaUser>(); }
        //}

        public RefsContext()
            : base("Name=MochaRefs")
        {
//#if DEBUG
//            Database.Log = s => System.Diagnostics.Debug.Write(s);
//#endif
        }

        public bool IsModified<TEntity>(TEntity entity) where TEntity : class
        {
            var entry = Entry<TEntity>(entity);
            return entry.State == EntityState.Modified;
        }

        public void MarkAsModified<TEntity>(TEntity entity) where TEntity: class
        {
            var entry = Entry<TEntity>(entity);
            entry.State = EntityState.Modified;
        }

        public DbContextTransaction BeginTransaction()
        {
            return Database.BeginTransaction();
        }

        public DbContextTransaction BeginTransaction(IsolationLevel isolationLevel)
        {
            return Database.BeginTransaction(isolationLevel);
        }

        public async Task<int> ExecuteSqlCommandAsync(string sql, params object[] parameters)
        {
            return await Database.ExecuteSqlCommandAsync(sql, parameters);
        }

        public int ExecuteSqlCommand(string sql, params object[] parameters)
        {
            return Database.ExecuteSqlCommand(sql, parameters);
        }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new FavoriteMap());
            modelBuilder.Configurations.Add(new RefListMap());
            modelBuilder.Configurations.Add(new RefListStatisticsMap());
            modelBuilder.Configurations.Add(new RefMap());
            modelBuilder.Configurations.Add(new TagMap());
            modelBuilder.Configurations.Add(new TagStatisticsMap());
            modelBuilder.Configurations.Add(new TagUseMap());
            modelBuilder.Configurations.Add(new TagUseStatisticsMap());
            modelBuilder.Configurations.Add(new UserMap());
            modelBuilder.Configurations.Add(new UserDataMap());
            modelBuilder.Configurations.Add(new UserLoginMap());

            base.OnModelCreating(modelBuilder);
        }

        #region Audit
        public override int SaveChanges()
        {
            UpdateAuditValues();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync()
        {
            UpdateAuditValues();
            return base.SaveChangesAsync();
        }

        private void UpdateAuditValues()
        {
            var now = DateTime.Now;
            var auditing = (
                from e in ChangeTracker.Entries<IAuditable>()
                where e.State == EntityState.Added || e.State == EntityState.Modified
                select e
            ).ToArray();

            if (auditing.Any())
            {
                var container = MochaContainer.GetContainer();
                var userContext = container.Resolve<IUserContext>();
                if (!userContext.IsAuthenticated)
                {
                    throw new InvalidOperationException();
                }
                var user = userContext.GetUser();
                foreach (var e in auditing)
                {
                    if (e.State == EntityState.Added)
                    {
                        e.Entity.CreatedUserId = user.Id;
                        e.Entity.CreatedDate = now;
                    }
                    e.Entity.UpdatedDate = now;
                    e.Entity.UpdatedUserId = user.Id;
                }
            }
        }

        #endregion // Audit

    }
}
