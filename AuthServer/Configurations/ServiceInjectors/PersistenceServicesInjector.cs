using AuthServer.Models.DTOs;
using AuthServer.Configurations.ServiceInjectors.Interfaces;
using AuthServer.Persistence;
using AuthServer.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using MySql.Data.EntityFrameworkCore.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using AuthServer.Models.Entities;
using AuthServer.Persistence.Repositories.Interfaces;
using AuthServer.Persistence.Repositories;

namespace AuthServer.Configurations.ServiceInjectors
{
	public class PersistenceServicesInjector : IServiceInjector
	{
		public void InjectServices(IConfiguration configuration, IServiceCollection services)
		{
            // to avoid needing to rebuild docker containers each time something has to change
            // using this way, can inject env vars to containers at runtime
            string DBServer = configuration["DBServer"] ?? "localhost";
            string DBPort = configuration["DBPort"] ?? "3306";
            string DBName = configuration["DBName"] ?? "AuthServer";
            string DBUser = configuration["DBUser"] ?? "root"; 
            string DBPassword = configuration["DBPassword"] ?? "root";

            //TODO: create new standalone server for production
            //TODO: create separate user for production and avoid using root
            //TODO: Research storing sensitive data in Azure DevOps or Equivalent to avoid storing password in plain text
            //TODO: Implement custom retry logic with strategies and remove transactions

            string connection = $"server={DBServer};userid={DBUser};pwd={DBPassword};port={DBPort};database={DBName};";

            services.AddDbContextPool<AuthServerContext>(options => options
                .UseMySql(connection
                //TODO: Implement custom retry logic with strategies and remove transactions by using Pomelo library and the below lines
                // , mySqlOptions => mySqlOptions
                //     .ServerVersion(new Version(8, 0, 18), ServerType.MySql)
                //     .EnableRetryOnFailure(
                //     maxRetryCount: 10,
                //     maxRetryDelay: TimeSpan.FromSeconds(30),
                //     errorNumbersToAdd: null
                //     )
            ));

            services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()
				.AddEntityFrameworkStores<AuthServerContext>(); 
                
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IOrganisationRepository, OrganisationRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IPolicyRepository, PolicyRepository>();

            services.AddTransient<IUnitOfWork, UnitOfWork>();
        }
	}
}
