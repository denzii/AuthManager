using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthServer.Configurations.DataTransferObjects
{
	public class JWTBearerAuthConfig
	{
		public string ResourceID { get; set; }

		public string InstanceID { get; set; }

		public string TenantID { get; set; }

		public string Secret { get; set; }

		public TimeSpan TokenLifetime { get;  set; }
	}
}
