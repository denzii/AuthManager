using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthServer.Configurations.CustomExtensions
{
	public static class HttpContextExtensions
	{
		public static string GetUserID(this HttpContext httpContext)
		{
			ClaimsPrincipal user = httpContext.User;

			if (user == null)
			{
				return String.Empty;
			}

			return user.Claims.Single(x => x.Type == "ID").Value;
		}

		public static string GetOrganisationID(this HttpContext httpContext)
		{
			ClaimsPrincipal user = httpContext.User;

			if (user == null)
			{
				return String.Empty;
			}

			return user.Claims.Single(x => x.Type == "OrganisationID").Value;
		}
	}
}
