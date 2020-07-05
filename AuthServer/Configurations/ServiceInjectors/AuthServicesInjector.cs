using AuthServer.Models.DTOs;
using AuthServer.Configurations.ServiceInjectors.Interfaces;
using AuthServer.Contracts.Version1;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;
using System.IdentityModel.Tokens.Jwt;

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
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtBearerAuthConfig.Secret));

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                        IssuerSigningKey = symmetricSecurityKey,
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        RequireExpirationTime = false,
                        ValidateLifetime = true
                    };

            services.AddSingleton(tokenValidationParameters);
            services.AddSingleton(symmetricSecurityKey);
            services.AddSingleton<SigningCredentials>(sCredentials => {
                return new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256Signature);
            });
            services.AddSingleton<SecurityTokenDescriptor>();    
            services.AddSingleton<JwtSecurityTokenHandler>(); 

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
