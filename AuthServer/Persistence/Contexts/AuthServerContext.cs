using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AuthServer.Models.Entities;
using AuthServer.Persistence.EntityConfigurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuthServer.Persistence.Contexts
{
	public class AuthServerContext : IdentityDbContext<User>
	{
		protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            foreach (var foreignKey in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
            }
                modelBuilder.ApplyConfiguration(new OrganisationConfiguration());
                modelBuilder.ApplyConfiguration(new PolicyConfiguration());
                modelBuilder.ApplyConfiguration(new UserConfiguration());
        }

		public AuthServerContext(DbContextOptions<AuthServerContext> options)
		: base(options){}

		public DbSet<Organisation> Organisations { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Policy> Policies { get; set; }
	}
}
