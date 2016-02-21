using Mocha.Refs.Core.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Mocha.Refs.Data.Mapping
{
    public class TagStatisticsMap : EntityTypeConfiguration<TagStatistics>
    {
        public TagStatisticsMap()
        {
            // Primary Key
            this.HasKey(t => t.TagId);

            // Properties
            this.Property(t => t.TagId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Relationships
            this.HasRequired(t => t.Tag)
                .WithRequiredDependent(t => t.Statistics)
                .WillCascadeOnDelete(true);
        }
    }
}
