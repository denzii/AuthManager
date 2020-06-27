using AuthServer.Configurations.Middlewares.Invokers.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace AuthServer.Configurations.Middlewares.Invokers
{
    public class ExceptionMiddlewareImvoker: IMiddlewareInvoker
    {
        public void InvokeMiddleware(IApplicationBuilder app, IConfiguration configuration)
        {
            app.UseMiddleware<ExceptionMiddleware>();
        }
    }
}