using Mocha.Refs.Core.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Mocha.Refs.Data.Mapping
{
    public class RefListStatisticsMap : EntityTypeConfiguration<RefListStatistics>
    {
        public RefListStatisticsMap()
        {
            // Primary Key
            this.HasKey(t => t.RefListId);

            // Properties
            this.Property(t => t.RefListId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Relationships
            this.HasRequired(t => t.RefList)
                .WithRequiredDependent(t => t.Statistics)
                .WillCascadeOnDelete(true);
        }
    }
}
