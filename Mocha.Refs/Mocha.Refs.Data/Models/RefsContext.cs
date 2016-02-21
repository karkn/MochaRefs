using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using Mocha.Refs.Data.Models.Mapping;

namespace Mocha.Refs.Data.Models
{
    public partial class RefsContext : DbContext
    {
        static RefsContext()
        {
            Database.SetInitializer<RefsContext>(null);
        }

        public RefsContext()
            : base("Name=RefsContext")
        {
        }

        public DbSet<RefList> RefLists { get; set; }
        public DbSet<RefListStatistic> RefListStatistics { get; set; }
        public DbSet<Ref> Refs { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<TagUs> TagUses { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new RefListMap());
            modelBuilder.Configurations.Add(new RefListStatisticMap());
            modelBuilder.Configurations.Add(new RefMap());
            modelBuilder.Configurations.Add(new TagMap());
            modelBuilder.Configurations.Add(new TagUsMap());
            modelBuilder.Configurations.Add(new UserMap());
        }
    }
}
