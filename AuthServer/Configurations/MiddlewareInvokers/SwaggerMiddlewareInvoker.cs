using AuthServer.Configurations.DataTransferObjects;
using AuthServer.Configurations.MiddlewareInvokers.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AuthServer.Configurations.AppBuilders
{
	public class SwaggerMiddlewareInvoker : IMiddlewareInvoker
	{
		public void InvokeMiddleware(IApplicationBuilder app, IConfiguration configuration)
		{
            SwaggerConfig swaggerConfig = new SwaggerConfig();
            configuration.GetSection(nameof(SwaggerConfig)).Bind(swaggerConfig);

            app.UseSwagger(options =>
            {
                options.RouteTemplate = swaggerConfig.JSONRoute;
            });

            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint(swaggerConfig.UIEndpoint, swaggerConfig.Description);
            });
        }
    }
}
