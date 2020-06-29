using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using AuthServer.Persistence.Contexts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AuthServer.Configurations.CustomExtensions;
using Microsoft.Extensions.Logging;
using Serilog;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using AuthServer.Configurations.Middlewares;
using AuthServer.Contracts.Version1.ResponseContracts;
using static AuthServer.Contracts.Version1.ResponseContracts.Errors;

namespace AuthServer
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllers();

			services.AddMvc(options => {
				options.EnableEndpointRouting = false;
			})
			.AddFluentValidation(config => config.RegisterValidatorsFromAssemblyContaining<Startup>())
			.SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
			.ConfigureApiBehaviorOptions(options => {
					//with .net 3.1, have to manually map the default error response object into a custom one
					// when context.ModelState.IsValid, return ValidationFail object rather than the inbuilt one
   					options.InvalidModelStateResponseFactory = context => {
                    IEnumerable<ValidationErrorResponse> errors = context.ModelState
                        .Where(x => x.Value.Errors.Any())
                        .SelectMany(pair => pair.Value.Errors.Select(fieldError => new ValidationErrorResponse()
                        {
                            FieldName = pair.Key,
                            Message = fieldError.ErrorMessage
                        }));

                    return new BadRequestObjectResult(errors); 
                };
            });

			services.InjectRegisteredServices(Configuration);
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
		{
		    app.UseMiddleware<ExceptionMiddleware>();
			
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseHsts();
			}

			app.UseHttpsRedirection();

            app.UseStaticFiles();
			app.UseSerilogRequestLogging();
            app.UseRouting();

            app.UseAuthorization();
            app.UseAuthentication();

            app.UseCors();

			app.UseRegisteredMiddleware(Configuration);

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
		}
	}
}
