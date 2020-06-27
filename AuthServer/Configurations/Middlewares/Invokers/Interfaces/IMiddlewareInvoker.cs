using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthServer.Configurations.Middlewares.Invokers.Interfaces
{
	public interface IMiddlewareInvoker
	{ 
		public void InvokeMiddleware(IApplicationBuilder app, IConfiguration configuration);
	}
}
