using AuthServer.Contracts.Version1.RequestContracts;
using AuthServer.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static AuthServer.Contracts.Version1.RequestContracts.Authentication;

namespace AuthServer.Persistence.Repositories.Interfaces
{
	public interface IUserRepository : IRepository<User>
	{
		IEnumerable<User> GetAllByOrganisation(Organisation organisation);

		IEnumerable<User> GetByUserName(string firstName, string lastName, Organisation organisation);

		User GetByEmail(string email);

		Task<bool> UserWithEmailExistsAsync(string email);

		User GetUserWithOrganisation(string ID);

		User CreateUser(RegistrationRequest request, Organisation organisation);
    }
}
