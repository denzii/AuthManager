using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthServer.Configurations.CustomExtensions
{
	public static class HttpContextExtensions
	{
		public static string GetUserID(this HttpContext httpContext)
		{
			if (httpContext.User == null)
			{
				return String.Empty;
			}

			return httpContext.User.Claims.Single(x => x.Type == "ID").Value;
		}
	}
}
