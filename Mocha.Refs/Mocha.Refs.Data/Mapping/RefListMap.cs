using Mocha.Refs.Core.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Mocha.Refs.Data.Mapping
{
    public class RefListMap : EntityTypeConfiguration<RefList>
    {
        public RefListMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Relationships
            this.HasMany(t => t.TagUses)
                .WithMany(t => t.RefLists)
                .Map(m =>
                    {
                        m.ToTable("RefListTagUses");
                        m.MapLeftKey("RefListId");
                        m.MapRightKey("TagUseId");
                    });

            this.HasRequired(t => t.Author)
                .WithMany(t => t.AuthoredRefLists)
                .HasForeignKey(d => d.AuthorId)
                .WillCascadeOnDelete(false);

        }
    }
}
