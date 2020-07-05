using AuthServer.Contracts.Version1.ResponseContracts;
using AuthServer.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static AuthServer.Contracts.Version1.RequestContracts.Authentication;
using static AuthServer.Contracts.Version1.ResponseContracts.Authentication;
using static AuthServer.Contracts.Version1.ResponseContracts.Errors;

namespace AuthServer.Models.Services.Interfaces
{
	public interface IAuthenticationService
	{
		Task<LoginResponse> LoginUserAsync(LoginRequest request, User user);
		
		Task<List<ErrorResponse>> ValidateRegistrationAsync(RegistrationRequest request, Organisation organisation, User newUser);

		Task<RegistrationResponse> RegisterUserAsync(RegistrationRequest request, Organisation organisation, User newUser);
	}
}
