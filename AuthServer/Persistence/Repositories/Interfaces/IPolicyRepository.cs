using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AuthServer.Models.Entities;
using AuthServer.Persistence.Repositories.Interfaces;

namespace AuthServer.Persistence.Repositories.Interfaces
{
    public interface IPolicyRepository : IRepository<Policy>
    {
        Policy GetByOrganisation(string ID, string organisationID);

        IEnumerable<Policy> GetAllByOrganisation (string organisationID);
        Task<bool> PolicyExist(string name, string organisationID);
    }
}