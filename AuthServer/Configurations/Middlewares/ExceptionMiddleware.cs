using System;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Serilog;
using Newtonsoft.Json;

namespace AuthServer.Configurations.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch(Exception e)
            {
                Log.Fatal(e, "Fatal Exception logged");
                await HandleExceptionAsync(httpContext);
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private Task HandleExceptionAsync(HttpContext httpContext)
        {
            
            httpContext.Response.ContentType = MediaTypeNames.Application.Json;
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

            return httpContext.Response.WriteAsync(
                JsonConvert.SerializeObject(new {
                    StatusCode = httpContext.Response.StatusCode,
                    Message = "It worked on my machine :/"
                })
            );

        }
    }
}