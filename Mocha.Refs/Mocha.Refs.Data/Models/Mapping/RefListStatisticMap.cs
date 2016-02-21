using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Mocha.Refs.Data.Models.Mapping
{
    public class RefListStatisticMap : EntityTypeConfiguration<RefListStatistic>
    {
        public RefListStatisticMap()
        {
            // Primary Key
            this.HasKey(t => t.RefListId);

            // Properties
            this.Property(t => t.RefListId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("RefListStatistics");
            this.Property(t => t.RefListId).HasColumnName("RefListId");
            this.Property(t => t.ViewCount).HasColumnName("ViewCount");

            // Relationships
            this.HasRequired(t => t.RefList)
                .WithOptional(t => t.RefListStatistic);

        }
    }
}
