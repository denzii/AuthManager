using System;
using AuthServer.Contracts.Version1.RequestContracts.Queries;

namespace AuthServer.Models.Services.Interfaces
{
    public interface IURIService
    {
        Uri GetPaginationURI(PaginationQuery query = null);

        Uri GetUserURI(string userID);
        Uri GetOrganisationURI(string organisationID);
        Uri GetPolicyURI(string policyName);
    }
}