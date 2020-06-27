using AuthServer.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthServer.Persistence.EntityConfigurations
{
	public class PolicyConfiguration : IEntityTypeConfiguration<Policy>
	{
		public void Configure(EntityTypeBuilder<Policy> modelBuilder)
		{			
			modelBuilder.HasKey(policy => new {policy.Name, policy.OrganisationID});
			
			modelBuilder
			.HasOne(policy => policy.Organisation)
			.WithMany(organisation => organisation.Policies)
			.HasPrincipalKey(policy => policy.Name);

			modelBuilder
			.HasMany(policy => policy.Users)
			.WithOne(user => user.Policy);
		}
    }
}