using AuthServer.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthServer.Persistence.EntityConfigurations
{
	public class PolicyConfiguration : IEntityTypeConfiguration<Policy>
	{
		public void Configure(EntityTypeBuilder<Policy> modelBuilder)
		{			
			modelBuilder
			.HasOne(policy => policy.Organisation)
			.WithMany(organisation => organisation.Policies);

			modelBuilder
			.HasMany(policy => policy.Users)
			.WithOne(user => user.Policy);
		}
    }
}