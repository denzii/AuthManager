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
    public class PolicyRepository : Repository<Policy>, IPolicyRepository
    {
        public PolicyRepository(AuthServerContext context) : base(context)
        {
        }

        public Task<List<Policy>> GetAllByOrganisationAsync(string organisationID, PageFilter filter = null)
        {
            IQueryable<Policy> baseQuery = AppContext.Policies
            .Include(policy => policy.Users)
            .Where(policy => policy.Organisation.ID == organisationID);

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

        public Task<Policy> GetByOrganisationAsync(string name, string organisationID)
        {
            return AppContext.Policies
            .Where(policy => policy.Name == name && policy.Organisation.ID == organisationID)
            ?.Include(policy => policy.Users)
            .SingleOrDefaultAsync();
        }

        public Task<bool> PolicyExistAsync(string name, string organisationID)
        {
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