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

        public IEnumerable<User> GetAllByOrganisation(Organisation organisation)
        {
            return AppContext.Users
                .Where(u => u.Organisation.ID == organisation.ID)
                .OrderBy(u => u.FirstName)
                .ThenBy(u => u.LastName);
        }

        public User GetByEmail(string email)
        {
            return AppContext.Users
            .Include(u => u.Organisation)
            .SingleOrDefault(u => u.Email == email);
        }

        public User GetUserWithOrganisation(string ID)
        {
            return AppContext.Users.Include(u => u.Organisation)
                .SingleOrDefault(u => u.Id == ID);
        }

        public IEnumerable<User> GetByUserName(string firstName, string lastName, Organisation organisation)
        {
           return AppContext.Users
                .Where(u => u.FirstName == firstName && u.LastName == lastName && u.Organisation.ID == organisation.ID)
                .OrderBy(u => u.FirstName)
                .ThenBy(u => u.LastName);
        }

        public User CreateUser(RegistrationRequest request, Organisation organisation)
        {
            User newUser = new User
            {
                UserName = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Sex = request.Sex,
                Email = request.Email,
                RegisteredOn = DateTime.Now,
                Organisation = organisation,
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
