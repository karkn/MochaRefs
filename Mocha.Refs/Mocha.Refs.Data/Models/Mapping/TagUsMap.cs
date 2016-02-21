using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Mocha.Refs.Data.Models.Mapping
{
    public class TagUsMap : EntityTypeConfiguration<TagUs>
    {
        public TagUsMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.RowVersion)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            this.ToTable("TagUses");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.RowVersion).HasColumnName("RowVersion");
            this.Property(t => t.TagId).HasColumnName("TagId");
            this.Property(t => t.OwnerId).HasColumnName("OwnerId");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            this.Property(t => t.CreatedUserId).HasColumnName("CreatedUserId");
            this.Property(t => t.UpdatedUserId).HasColumnName("UpdatedUserId");

            // Relationships
            this.HasRequired(t => t.Tag)
                .WithMany(t => t.TagUses)
                .HasForeignKey(d => d.TagId);
            this.HasRequired(t => t.User)
                .WithMany(t => t.TagUses)
                .HasForeignKey(d => d.OwnerId);

        }
    }
}
