using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Mocha.Refs.Data.Models.Mapping
{
    public class RefMap : EntityTypeConfiguration<Ref>
    {
        public RefMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            // Table & Column Mappings
            this.ToTable("Refs");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Kind).HasColumnName("Kind");
            this.Property(t => t.Uri).HasColumnName("Uri");
            this.Property(t => t.Title).HasColumnName("Title");
            this.Property(t => t.Comment).HasColumnName("Comment");
            this.Property(t => t.IsRecommended).HasColumnName("IsRecommended");
            this.Property(t => t.DisplayOrder).HasColumnName("DisplayOrder");
            this.Property(t => t.RefListId).HasColumnName("RefListId");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            this.Property(t => t.CreatedUserId).HasColumnName("CreatedUserId");
            this.Property(t => t.UpdatedUserId).HasColumnName("UpdatedUserId");

            // Relationships
            this.HasRequired(t => t.RefList)
                .WithMany(t => t.Refs)
                .HasForeignKey(d => d.RefListId);

        }
    }
}
