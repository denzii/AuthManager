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
using Serilog;

namespace AuthServer
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var config = new ConfigurationBuilder()
			.AddJsonFile("appsettings.json")
			.Build();
			
			Log.Logger = new LoggerConfiguration()
			.ReadFrom.Configuration(config)
			.CreateLogger();

			try
			{
				Log.Information("Spinning application up {Date}", DateTime.Now);
				
				var hostBuilder = CreateHostBuilder(args).Build();
				InitializeHostServices(hostBuilder);
				hostBuilder.Run();
			} 
			catch(Exception e)
			{
				Log.Fatal(e, "Application couldn't be started");
			} 
			finally
			{
				Log.CloseAndFlush();
			}
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>

			Host.CreateDefaultBuilder(args)
				.UseSerilog()
				.ConfigureWebHostDefaults(webBuilder => {
					webBuilder.UseStartup<Startup>();
				});

		private static void InitializeHostServices(IHost host)
		{
			//TODO doesnt work properly
			using (var scope = host.Services.CreateScope())
			{
				var services = scope.ServiceProvider;
				AuthServerContext context = services.GetRequiredService<AuthServerContext>();
				context.Database.Migrate();	
			}
		}
	}
}
