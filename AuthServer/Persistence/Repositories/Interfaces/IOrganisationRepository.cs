using AuthServer.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthServer.Persistence.Repositories.Interfaces
{
	public interface IOrganisationRepository : IRepository<Organisation>
	{
		IEnumerable<Organisation> GetByUser(User user);

		Task<Organisation> GetByNameAsync(string organisationName);
		
        Task<List<Organisation>> GetAllWithUsers();
    }
}
