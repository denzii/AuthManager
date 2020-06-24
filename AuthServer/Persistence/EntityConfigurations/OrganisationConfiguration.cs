using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthServer.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthServer.Persistence.EntityConfigurations
{
	public class OrganisationConfiguration : IEntityTypeConfiguration<Organisation>
	{
		public void Configure(EntityTypeBuilder<Organisation> modelBuilder)
		{
			modelBuilder
				.HasKey(organisation => new {
				  organisation.ID,
				  organisation.EstablishedOn 
			  });

			modelBuilder
				.HasMany(organisation => organisation.Users)
				.WithOne(user => user.Organisation);
		}
	}
}
