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
	public class PolicyRepository : Repository<Policy>, IPolicyRepository
    {
        public PolicyRepository(AuthServerContext context) : base(context)
        {
        }

        public IEnumerable<Policy> GetAllByOrganisation(string organisationID)
        {
            return AppContext.Policies
            .Where(policy => policy.Organisation.ID == organisationID)
            .ToList();
        }

        public Task<Policy> GetByOrganisation(string name, string organisationID)
        {
            return AppContext.Policies
            .Where(policy => policy.Name == name && policy.Organisation.ID == organisationID)
            ?.Include(policy => policy.Users)
            .SingleOrDefaultAsync();
        }

        public Task<bool> PolicyExist(string name, string organisationID)
        {
            var x = AppContext.Policies.ToList();
            
            return AppContext.Policies
            .Where(policy => policy.Name == name && policy.Organisation.ID == organisationID)
            .AnyAsync();
        }

        public AuthServerContext AppContext
        {
            get { return _context as AuthServerContext; }
        }
    }
}