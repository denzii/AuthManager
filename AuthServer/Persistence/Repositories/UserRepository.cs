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

        public Task<List<UserDTO>> GetAllByOrganisation(string organisationID)
        {
            return AppContext.Users
            .Where(user => user.Organisation.ID == organisationID)
            .Select(user => new UserDTO{
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PolicyName = user.Policy != null ? user.Policy.Name : null
            }).ToListAsync();
        }

        public Task<UserDTO> GetByOrganisation(string ID, string organisationID)
        {
            return AppContext.Users
            .Where(user => user.Id == ID && user.Organisation.ID == organisationID)
            ?.Include(user => user.Organisation)
            .Include(user => user.Policy)
            .Select(user => new UserDTO{
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PolicyName = user.Policy != null ? user.Policy.Name : null
            })
            .SingleOrDefaultAsync();
        }

        public Task<User> GetByEmail(string email)
        {
            return AppContext.Users
            .Include(user => user.Organisation)
            .Include(user => user.Policy)
            .SingleOrDefaultAsync(user => user.Email == email);
        }

        public Task<User> GetWithDetails(string ID, string organisationID)
        {
            return AppContext.Users
            .Where(user => user.Id == ID && user.Organisation.ID == organisationID)
            .Include(user => user.Organisation)
            .Include(user => user.Policy)
            .SingleOrDefaultAsync();
        }

        public IEnumerable<User> GetByUserName(string firstName, string lastName, string organisationID)
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
