using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthServer.Contracts.Version1.RequestContracts
{
	public static class Organisations
	{
		public class PostRequest
		{
			public string OrganisationName { get; set; }
		}

		public class PatchRequest
		{
			public string OrganisationName { get; set; }
		}

		public class DeleteRequest
		{
			public string ID { get; set; }
		}
	}
}
