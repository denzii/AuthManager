using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;

namespace AuthServer.Configurations.Middlewares
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ApiKeyAuthAttribute : Attribute, IAsyncActionFilter
    {
        public const string ApiKeyHeaderName = "ApiKey";
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //can also get it from query string instead of header via =>
            // context.HttpContext.Request.Query[ApiKeyHeaderName];

            if (!context.HttpContext.Request.Headers.TryGetValue(ApiKeyHeaderName, out StringValues potentialApiKey)){
                context.Result = new UnauthorizedResult();
                return;
            }

            IConfiguration config = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
            var apiKey = $"{ApiKeyHeaderName} {config.GetValue<string>(ApiKeyHeaderName)}";

            if (!apiKey.Equals(potentialApiKey)){
                context.Result = new UnauthorizedResult();
                return;
            }

            await next();
        }
    }
}