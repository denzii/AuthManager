using AuthServer.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static AuthServer.Contracts.Version1.RequestContracts.Authentication;
using static AuthServer.Contracts.Version1.ResponseContracts.Authentication;

namespace AuthServer.Models.Services.Interfaces
{
	public interface IAuthenticationService
	{
		public string GetRegistrationValidationResult(User user, Organisation organisation);

		public Task<RegistrationResponse> RegisterUserAsync(RegistrationRequest request);

		public Task<LoginResponse> LoginUserAsync(LoginRequest request);

		public Task<RefreshTokenResponse> RefreshTokenAsync(string token, string refreshToken);
	}
}
