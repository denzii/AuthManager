using AuthServer.Configurations.ServiceInjectors.Interfaces;
using AuthServer.Models.Services;
using AuthServer.Models.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthServer.Configurations.ServiceInjectors
{
	public class WorkerServicesInjector : IServiceInjector
	{
		public void InjectServices(IConfiguration configuration, IServiceCollection services)
		{
			services.AddScoped<IAuthenticationService, AuthenticationService>();
		}
	}
}
