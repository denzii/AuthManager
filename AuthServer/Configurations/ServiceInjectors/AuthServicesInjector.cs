using AuthServer.Configurations.DataTransferObjects;
using AuthServer.Configurations.ServiceInjectors.Interfaces;
using AuthServer.Contracts.Version1;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

namespace AuthServer.Configurations.ServiceInjectors
{
	public class AuthServicesInjector : IServiceInjector
	{
		public void InjectServices(IConfiguration configuration, IServiceCollection services)
		{
            JWTBearerAuthConfig jwtBearerAuthConfig = new JWTBearerAuthConfig();
            configuration.GetSection(nameof(JWTBearerAuthConfig)).Bind(jwtBearerAuthConfig);
            Console.WriteLine(configuration.GetSection(nameof(JWTBearerAuthConfig)));
            services.AddSingleton(jwtBearerAuthConfig);

            //JWT Bearer with Azure Active Directory for client app authentication
            //services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            //    .AddJwtBearer(options => {
            //        options.Audience = jwtBearerAuthConfig.ResourceID;
            //        options.Authority = $"{jwtBearerAuthConfig.InstanceID}{jwtBearerAuthConfig.TenantID}";
            //    });


            //JWT Bearer for personal authentication
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtBearerAuthConfig.Secret)),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        RequireExpirationTime = false,
                        ValidateLifetime = true
                    };

            services.AddSingleton(tokenValidationParameters);
            services.AddAuthentication(options => { 
               options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
               options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
               options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
           })
                .AddJwtBearer(options => {
                    options.SaveToken = true;
                    options.TokenValidationParameters = tokenValidationParameters;
                });
        
             services.AddAuthorization(options => {
                 options.AddPolicy(AuthorizationPolicies.AdminPolicy, builder => builder.RequireClaim(AuthorizationPolicies.AdminClaim, "true"));
                 });
        }
	}
}
