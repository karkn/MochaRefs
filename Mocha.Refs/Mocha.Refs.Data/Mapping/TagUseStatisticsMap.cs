using Mocha.Refs.Core.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Mocha.Refs.Data.Mapping
{
    public class TagUseStatisticsMap : EntityTypeConfiguration<TagUseStatistics>
    {
        public TagUseStatisticsMap()
        {
            // Primary Key
            this.HasKey(t => t.TagUseId);

            // Properties
            this.Property(t => t.TagUseId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Relationships
            this.HasRequired(t => t.TagUse)
                .WithRequiredDependent(t => t.Statistics)
                .WillCascadeOnDelete(true);
        }
    }
}
