using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthClient
{
	public class JWTBearerAuthConfig
	{
		public string Secret { get; set; }

		public TimeSpan TokenLifetime { get;  set; }
	}
}
