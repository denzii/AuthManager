using AuthServer.Models.DataTransferObjects;
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
		Task<List<UserDTO>> GetAllByOrganisation(string organisationID);

        Task<UserDTO> GetByOrganisation(string ID, string organisationID);

		IEnumerable<User> GetByUserName(string firstName, string lastName, string organisationID);
        
        Task<User> GetByEmail(string email);

		Task<bool> UserWithEmailExistsAsync(string email);

		Task<User> GetWithDetails(string ID, string organisationID);

		User CreateUser(RegistrationRequest request, Organisation organisation, Policy policy);
    }
}
