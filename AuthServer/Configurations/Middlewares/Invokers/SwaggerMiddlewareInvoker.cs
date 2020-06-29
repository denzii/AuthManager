using AuthServer.Models.DTOs;
using AuthServer.Configurations.Middlewares.Invokers.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AuthServer.Configurations.Middlewares.Invokers
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
