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
            services.AddScoped<ITokenService, TokenService>();

            services.AddScoped<IURIService, URIService>(sProvider => {
                //set up the service in a way which guarantees the base url to be the same as the one accessed by user
                var accessor = sProvider.GetRequiredService<IHttpContextAccessor>();
                HttpRequest request = accessor.HttpContext.Request;
                var absoluteURI = string.Concat(request.Scheme, "://", request.Host.ToUriComponent());

                return new URIService(absoluteURI);
            });
        }
    }
}
