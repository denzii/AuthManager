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
			services.AddMvc().AddFluentValidation(config => config.RegisterValidatorsFromAssemblyContaining<Startup>())
			.SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.InjectRegisteredServices(Configuration);
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
		{
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
