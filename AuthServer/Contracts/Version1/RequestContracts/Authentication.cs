using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthServer.Contracts.Version1.RequestContracts
{
	public static class Authentication
	{
		public class LoginRequest
		{
			public string Email { get; set; }
			public string Password { get; set; }
		}

		public class RegistrationRequest
		{
			public string Email { get; set; }
			public string Password { get; set; }
			public string FirstName { get; set; }
			public string LastName { get; set; }
			public string Sex { get; set; }
			public string OrganisationName { get; set; }
		}

		public class RefreshTokenRequest
		{
			public string Token { get; set; }
			public string RefreshToken { get; set; }
		}
	}
}
