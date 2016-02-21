using Mocha.Refs.Core.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Mocha.Refs.Data.Mapping
{
    public class UserLoginMap : EntityTypeConfiguration<UserLogin>
    {
        public UserLoginMap()
        {
            // Primary Key
            this.HasKey(t => new
            {
                t.UserId,
                t.LoginProvider,
                t.ProviderKey,
            });
                
            // Properties
            this.Property(t => t.UserId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Relationships
            //this.HasRequired(t => t.User)
            //    .WithMany(t => t.UserLogins)
            //    .HasForeignKey(t => t.UserId)
            //    .WillCascadeOnDelete(true);
        }
    }
}
