using AuthServer.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static AuthServer.Contracts.Version1.RequestContracts.Authentication;

namespace AuthServer.Models.Handlers.Interfaces
{
	public interface ICipherHandler
	{
		// public string GetHash(RegistrationRequest request);

		// public bool ValidateUserPassword(User user, LoginRequest request);
	}
}
