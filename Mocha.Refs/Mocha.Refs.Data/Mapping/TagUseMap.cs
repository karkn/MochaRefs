using Mocha.Refs.Core.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Mocha.Refs.Data.Mapping
{
    public class TagUseMap : EntityTypeConfiguration<TagUse>
    {
        public TagUseMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Relationships
            this.HasRequired(t => t.Tag)
                .WithMany(t => t.TagUses)
                .HasForeignKey(d => d.TagId);

            this.HasRequired(t => t.Owner)
                .WithMany(t => t.OwnedTagUses)
                .HasForeignKey(d => d.OwnerId)
                .WillCascadeOnDelete(false);
        }
    }
}
