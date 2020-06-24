using AuthServer.Configurations.ServiceInjectors;
using AuthServer.Configurations.ServiceInjectors.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthServer.Configurations.CustomExtensions
{
	public static class ServiceCollectionExtensions
	{
        //extends IServiceCollection with a new method by taking in the parameter (this IServiceCollection services)
        //allows direct call eg. services.InstallAllRegisteredServices(Configuration);
		public static void InjectRegisteredServices(this IServiceCollection services, IConfiguration configuration)
		{
            //Get All Types in the codebase, Refine the results to the concrete ones implementing IServiceInjector
            List<Type> serviceInjectorTypes = typeof(Startup).Assembly.GetExportedTypes()
                .Where(type => typeof(IServiceInjector).IsAssignableFrom(type) && !type.IsAbstract && !type.IsInterface)
                .ToList();

            //Project the Types acquired into instances of the types
            //cast the instantiated Objects into strictly typed ones (IServiceInjectors) so they can be collected in a list.
            List<IServiceInjector> serviceInjectors = serviceInjectorTypes
                .Select(Activator.CreateInstance)
                .Cast<IServiceInjector>()
                .ToList();

            serviceInjectors.ForEach(sInjector => sInjector.InjectServices(configuration, services));
        }

        public static void InjectJwtBearerAuthSettings(this IServiceCollection services, IConfiguration configuration)
		{
            //Get All Types in the codebase, Refine the results to the concrete ones implementing IServiceInjector
            List<Type> serviceInjectorTypes = typeof(Startup).Assembly.GetExportedTypes()
                .Where(type => typeof(AuthServicesInjector).IsAssignableFrom(type) && !type.IsAbstract && !type.IsInterface)
                .ToList();

            //Project the Types acquired into instances of the types
            //cast the instantiated Objects into strictly typed ones (IServiceInjectors) so they can be collected in a list.
            List<IServiceInjector> serviceInjectors = serviceInjectorTypes
                .Select(Activator.CreateInstance)
                .Cast<IServiceInjector>()
                .ToList();

            serviceInjectors.ForEach(sInjector => sInjector.InjectServices(configuration, services));
        }
	}
}
