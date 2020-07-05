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
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Http;

namespace AuthServer.Configurations.ServiceInjectors
{
	public class WorkerServicesInjector : IServiceInjector
	{
		public void InjectServices(IConfiguration configuration, IServiceCollection services)
		{
			services.AddScoped<IAuthenticationService, AuthenticationService>();
			services.AddScoped<IURIService, URIService>(sProvider => {
				//set up the service in a way that guarantees that the URL the user 
				//used to call the endpoint is what they will get in the response
				var accessor = sProvider.GetRequiredService<IHttpContextAccessor>();
				var request = accessor.HttpContext.Request;
				var absoluteURI = string.Concat(request.Scheme, "://", request.Host.ToUriComponent());

				return new URIService(absoluteURI);
			});
			services.AddScoped<ITokenService, TokenService>();
		}
	}
}
