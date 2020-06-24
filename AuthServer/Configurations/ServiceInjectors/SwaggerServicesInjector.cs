﻿using AuthServer.Configurations.ServiceInjectors.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthServer.Configurations.ServiceInjectors
{
	public class SwaggerServicesInjector : IServiceInjector
	{
		public void InjectServices(IConfiguration configuration, IServiceCollection services)
		{
			services.AddSwaggerGen(options => { 
				options.SwaggerDoc("v1", new OpenApiInfo { Title = "AuthServer", Version = "v1" });

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
	}
}
