using AuthServer.Models.DataTransferObjects;
using AuthServer.Models.Entities;
using AuthServer.Persistence.Contexts;
using AuthServer.Persistence.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static AuthServer.Contracts.Version1.RequestContracts.Authentication;

namespace AuthServer.Persistence.Repositories
{
	public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(AuthServerContext context) : base(context)
        {
        }

        public IEnumerable<UserDTO> GetAllByOrganisation(int organisationID)
        {
            return AppContext.Users
            .Where(u => u.Organisation.ID == organisationID)
            ?.Include(u => u.Organisation)
            .Include(u => u.Policy)
            .OrderBy(u => u.FirstName)
            .ThenBy(u => u.LastName)
            .ToList()
            .Select(u => new UserDTO{
                FirstName = u.FirstName,
                LastName = u.LastName,
                OrganisationName = u.Organisation.OrganisationName,
                PolicyName = u.Policy.PolicyName
            });
        }

        public User GetByEmail(string email)
        {
            return AppContext.Users
            .Include(u => u.Organisation)
            .SingleOrDefault(u => u.Email == email);
        }

        public User GetUserWithDetails(string ID)
        {
            return AppContext.Users
            .Include(user => user.Organisation)
            .Include(user => user.Policy)
            .SingleOrDefault(u => u.Id == ID);
        }
        public IEnumerable<User> GetAllWithDetails(string ID)
        {
            return AppContext.Users
            .Include(user => user.Organisation)
            .Include(user => user.Policy)
            .ToList();
        }

        public IEnumerable<User> GetByUserName(string firstName, string lastName, int organisationID)
        {
           return AppContext.Users
            .Where(u => u.FirstName == firstName && u.LastName == lastName && u.Organisation.ID == organisationID)
            ?.OrderBy(u => u.FirstName)
            .ThenBy(u => u.LastName);
        }

        public User CreateUser(RegistrationRequest request, Organisation organisation, Policy policy)
        {
            User newUser = new User
            {
                Id = Guid.NewGuid().ToString(),
                UserName = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Sex = request.Sex,
                Email = request.Email,
                RegisteredOn = DateTime.Now,
                Organisation = organisation,
                Policy = policy
            };

            return newUser;
        }


        public Task<bool> UserWithEmailExistsAsync(string email)
        {
            return AppContext.Users.AnyAsync(u => u.Email == email);
        }

        public AuthServerContext AppContext
        {
            get { return _context as AuthServerContext; }
        }
    }
}
