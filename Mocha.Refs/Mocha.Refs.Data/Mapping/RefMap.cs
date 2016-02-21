using Mocha.Refs.Core.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Mocha.Refs.Data.Mapping
{
    public class RefMap : EntityTypeConfiguration<Ref>
    {
        public RefMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Relationships
            this.HasRequired(t => t.RefList)
                .WithMany(t => t.Refs)
                .HasForeignKey(d => d.RefListId);

        }
    }
}
