using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthServer.Contracts.Version1.ResponseContracts
{
	public static class Organisations
	{
		public class PostResponse
		{
			public int ID { get; set; }
			public string EstablishedOn { get; set; }
			public string Error { get; set; }
		}

		public class PatchResponse
		{
			public List<string> ModifiedFields { get; set; }
			public List<string> OldFieldValues { get; set; }
			public string Error { get; set; }
		}

		public class DeleteResponse
		{
			public string ID { get; set; }
			public string OrganisationName { get; set; }
			public string Error { get; set; }

		}
	}
}
