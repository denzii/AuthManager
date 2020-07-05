using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AuthServer.Models.DTOs;
using AuthServer.Models.Entities;
using AuthServer.Persistence.Repositories.Interfaces;

namespace AuthServer.Persistence.Repositories.Interfaces
{
    public interface IPolicyRepository : IRepository<Policy>
    {
        Task<Policy> GetByOrganisationAsync(string ID, string organisationID);

        Task<List<Policy>> GetAllByOrganisationAsync (string organisationID, PageFilter filter = null);
        
        Task<bool> PolicyExistAsync(string name, string organisationID);
    }
}