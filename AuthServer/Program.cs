using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using AuthServer.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using AuthServer.Models.Entities;
using AuthServer.Contracts.Version1;
using AuthServer.Configurations;

namespace AuthServer
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var hostBuilder = CreateHostBuilder(args).Build();
			
			InitializeHostServices(hostBuilder);

			hostBuilder.Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>

			Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseStartup<Startup>();
				});

		private static void InitializeHostServices(IHost host)
		{
			using (var scope = host.Services.CreateScope())
			{
				var services = scope.ServiceProvider;
				
				try
				{
					AuthServerContext context = services.GetRequiredService<AuthServerContext>();
					context.Database.Migrate();			
				}
				catch (Exception e)
				{
					Console.WriteLine(e.Message);
				}
			}
		}
	}
}
