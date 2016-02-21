using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Mocha.Refs.Data.Models.Mapping
{
    public class RefListMap : EntityTypeConfiguration<RefList>
    {
        public RefListMap()
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
            this.ToTable("RefLists");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Title).HasColumnName("Title");
            this.Property(t => t.Comment).HasColumnName("Comment");
            this.Property(t => t.PublishingStatus).HasColumnName("PublishingStatus");
            this.Property(t => t.IsDeleted).HasColumnName("IsDeleted");
            this.Property(t => t.RowVersion).HasColumnName("RowVersion");
            this.Property(t => t.AuthorId).HasColumnName("AuthorId");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            this.Property(t => t.CreatedUserId).HasColumnName("CreatedUserId");
            this.Property(t => t.UpdatedUserId).HasColumnName("UpdatedUserId");

            // Relationships
            this.HasMany(t => t.TagUses)
                .WithMany(t => t.RefLists)
                .Map(m =>
                    {
                        m.ToTable("RefListTagUses");
                        m.MapLeftKey("RefList_Id");
                        m.MapRightKey("TagUse_Id");
                    });

            this.HasRequired(t => t.User)
                .WithMany(t => t.RefLists)
                .HasForeignKey(d => d.AuthorId);

        }
    }
}
