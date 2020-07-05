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
                options.AddPolicy(InternalPolicies.AdminPolicy, builder => builder.RequireClaim(InternalPolicies.AdminClaim, "true"));
            });

            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256Signature);

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                // SecurityTokenDescriptor takes in an array of Claims wrapped in Claims Identity in its Subject field
                // This is how we specify exactly what the token must include, each rule is defined as a new Claim
                // Each Claim is matched against a registered Claim Name or custom "string".
                Expires = DateTime.UtcNow.Add(jwtBearerAuthConfig.TokenLifetime),
                SigningCredentials =  signingCredentials
            };
            services.AddSingleton<JWTBearerAuthConfig>(jwtBearerAuthConfig);
            services.AddSingleton<TokenValidationParameters>(tokenValidationParameters);
            services.AddSingleton<SymmetricSecurityKey>(symmetricSecurityKey);
            services.AddSingleton<JwtSecurityTokenHandler>();
            services.AddSingleton<SigningCredentials>(signingCredentials);
            services.AddSingleton<SecurityTokenDescriptor>(tokenDescriptor);
        }
    }
}
