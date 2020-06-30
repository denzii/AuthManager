using AuthServer.Models.DTOs;
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
		Task<List<User>> GetAllByOrganisationAsync(string organisationID, PageFilter pageFilter);

        Task<User> GetByOrganisationAsync(string ID, string organisationID);

		Task<List<User>> GetByUserNameAsync(string firstName, string lastName, string organisationID);
        
        Task<User> GetByEmailAsync(string email);

		Task<bool> UserWithEmailExistsAsync(string email);

		Task<User> GetWithDetailsAsync(string ID, string organisationID);

		User CreateUser(RegistrationRequest request, Organisation organisation, Policy policy);
    }
}
