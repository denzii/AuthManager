using AuthServer.Models.Entities;
using AuthServer.Persistence.Contexts;
using AuthServer.Persistence.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthServer.Persistence.Repositories
{
	public class OrganisationRepository : Repository<Organisation>, IOrganisationRepository
    {
        public OrganisationRepository(AuthServerContext context) : base(context)
        {
        }

        public Task<List<Organisation>> GetAllWithUsers(){
            return AppContext.Organisations
            .Include(organisation => organisation.Users)
            .ToListAsync();
        }

        public IEnumerable<Organisation> GetByUser(User user)
        {
            return AppContext.Organisations.Where(o => o.Users.Contains(user));
        }

        public Task<Organisation> GetByNameAsync(string organisationName)
        {
            return AppContext.Organisations
            .Include(organisation => organisation.Users)
            .SingleOrDefaultAsync(o => o.OrganisationName == organisationName);
        }

        public AuthServerContext AppContext
        {
            get { return _context as AuthServerContext; }
        }

    }
}
