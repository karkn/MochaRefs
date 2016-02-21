using Mocha.Refs.Core.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Mocha.Refs.Data.Mapping
{
    public class FavoriteMap : EntityTypeConfiguration<Favorite>
    {
        public FavoriteMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);


            // Relationships
            this.HasRequired(t => t.Owner)
                .WithMany(t => t.OwnedFavorites)
                .HasForeignKey(d => d.OwnerId);

            this.HasOptional(t => t.RefList)
                .WithMany(t => t.FavoringFavorites)
                .HasForeignKey(d => d.RefListId)
                .WillCascadeOnDelete(false);

            this.HasOptional(t => t.Tag)
                .WithMany(t => t.FavoringFavorites)
                .HasForeignKey(d => d.TagId)
                .WillCascadeOnDelete(false);

            this.HasOptional(t => t.User)
                .WithMany(t => t.FavoringFavorites)
                .HasForeignKey(d => d.UserId)
                .WillCascadeOnDelete(false);
        }
    }
}
