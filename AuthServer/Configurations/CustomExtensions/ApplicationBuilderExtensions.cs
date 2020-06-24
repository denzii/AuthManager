using AuthServer.Configurations.MiddlewareInvokers.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthServer.Configurations.CustomExtensions
{
	public static class ApplicationBuilderExtensions
	{
        public static void UseRegisteredMiddleware(this IApplicationBuilder app, IConfiguration configuration)
        {
            //Get All Types in the codebase, Refine the results to the concrete ones implementing IServiceInstaller
            List<Type> middlewareInvokerTypes = typeof(Startup).Assembly.GetExportedTypes()
                .Where(type => typeof(IMiddlewareInvoker).IsAssignableFrom(type) && !type.IsAbstract && !type.IsInterface)
                .ToList();

            //Project the Types acquired into instances of the types
            //cast the instantiated Objects into strictly typed ones (IMiddlewareInvoker) so they can be collected in a list.
            List<IMiddlewareInvoker> middlewareInvokers = middlewareInvokerTypes
                .Select(Activator.CreateInstance)
                .Cast<IMiddlewareInvoker>()
                .ToList();

            middlewareInvokers.ForEach(mInvoker => mInvoker.InvokeMiddleware(app, configuration));
        }
    }
}
