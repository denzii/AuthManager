using AuthServer.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthServer.Persistence.EntityConfigurations
{
	public class UserConfiguration : IEntityTypeConfiguration<User>
	{
		public void Configure(EntityTypeBuilder<User> modelBuilder)
		{			
			modelBuilder
				.HasOne(user => user.Organisation)
				.WithMany(organisation => organisation.Users);
		}
	}
}
