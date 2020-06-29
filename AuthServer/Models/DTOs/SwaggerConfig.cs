using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthServer.Models.DTOs
{
	public class SwaggerConfig
	{
		public string JSONRoute { get; set; }

		public string Description { get; set; }

		public string UIEndpoint { get; set; }
	}
}
