using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthServer.Contracts.Version1.ResponseContracts
{
	public static class Authentication
	{
		public class LoginResponse
		{
			public string Token { get; set; }
			public string RefreshToken { get; set; }
			public string Email { get; set; }
			public string Error { get; set; }
		}

		public class RegistrationResponse
		{
			public string Token { get; set; }
			public string RefreshToken { get; set; }
			public string Id { get; set; }
			public string Email { get; set; }
			public DateTime RegisteredOn { get; set; }
			public IEnumerable<string> Errors { get; set; }

		}

		public class RefreshTokenResponse
		{
			public string Token { get; set; }
			public string RefreshToken { get; set; }
			public string Email { get; set; }
			public string Error { get; set; }
		}
	}
}
