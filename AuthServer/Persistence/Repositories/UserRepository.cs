using AuthServer.Models.DTOs;
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

        public Task<List<User>> GetAllByOrganisationAsync(string organisationID, PageFilter filter)
        {
            IQueryable<User> baseQuery = AppContext.Users.Where(user => user.Organisation.ID == organisationID);

            if (filter == null) //will never be null but good to handle for if one day can be made null
            {
                return baseQuery.ToListAsync();
            }

            // eg given pageNumber = 3 && given pageSize = 10
            // 3 - 1 = 2;  2 * 10 = 20
            // hence skip 20 records as each page contains 10 and we are skipping pages 1 and 2
            var recordsToSkip  = (filter.PageNumber - 1) * filter.PageSize;

            return baseQuery.Skip(recordsToSkip).Take(filter.PageSize).ToListAsync();
        }

        public Task<User> GetByOrganisationAsync(string ID, string organisationID)
        {
            return AppContext.Users
            .Where(user => user.Id == ID && user.Organisation.ID == organisationID)
            ?.Include(user => user.Organisation)
            .Include(user => user.Policy)
            .SingleOrDefaultAsync();
        }

        public Task<User> GetByEmailAsync(string email)
        {
            return AppContext.Users
            .Include(user => user.Organisation)
            .Include(user => user.Policy)
            .SingleOrDefaultAsync(user => user.Email == email);
        }

        public Task<User> GetWithDetailsAsync(string ID, string organisationID)
        {
            return AppContext.Users
            .Where(user => user.Id == ID && user.Organisation.ID == organisationID)
            .Include(user => user.Organisation)
            .Include(user => user.Policy)
            .SingleOrDefaultAsync();
        }

        public Task<List<User>> GetByUserNameAsync(string firstName, string lastName, string organisationID)
        {
           return AppContext.Users
            .Where(u => u.FirstName == firstName && u.LastName == lastName && u.Organisation.ID == organisationID)
            ?.OrderBy(u => u.FirstName)
            .ThenBy(u => u.LastName)
            .ToListAsync();
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
