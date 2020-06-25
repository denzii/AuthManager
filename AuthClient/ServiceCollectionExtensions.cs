using AuthClient;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Linq;
using System.Text;
using Microsoft.OpenApi.Models;
using System.Collections.Generic;

namespace AuthClient
{
	public static class ServiceCollectionExtensions
	{
       public static void InjectJwtBearerAuthSettings(
            this IServiceCollection services,
            IConfiguration configuration,
            string jwtSecret,
            TokenValidationParameters parameters
              )
		{

            services.AddAuthentication(options => { 
               options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
               options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
               options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
           })
                .AddJwtBearer(options => {
                    options.SaveToken = true;
                    options.TokenValidationParameters = parameters;
                });
        
             services.AddAuthorization();
        }

        public static void InjectSwaggerJwtSuite(this IServiceCollection services, IConfiguration configuration, string apiName, string apiVersion)
		{
			services.AddSwaggerGen(options => { 
				options.SwaggerDoc(apiVersion, new OpenApiInfo { Title = apiName, Version = apiVersion});

				var security = new Dictionary<string, IEnumerable<string>>
				{
					{ "Bearer", new string[0] }
				};

				options.AddSecurityDefinition(name: "Bearer", new OpenApiSecurityScheme()
				{
					Description = "JWT Authorization header using the bearer scheme",
					Name = "Authorization",
					In = ParameterLocation.Header,
					Type = SecuritySchemeType.ApiKey
				});

       			options.CustomSchemaIds(x => x.FullName);
				   
				options.AddSecurityRequirement(new OpenApiSecurityRequirement{
					{ new OpenApiSecurityScheme{ Reference = new OpenApiReference
					{
						Id = "Bearer",
						Type = ReferenceType.SecurityScheme

					}}, new List<string>()}
				});
			});
		}

		public static void InjectAuthorizationPolicies(
			this IServiceCollection services, 
			Dictionary<string,string> PolicyClaimPairs)
		{
			services.AddAuthorization(options => {
				foreach(var pair in PolicyClaimPairs)
				{
					options.AddPolicy(pair.Key, builder => builder.RequireClaim(pair.Value, "true"));
				}
			});
		}
	}
    
}
