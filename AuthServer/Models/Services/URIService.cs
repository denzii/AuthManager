using System;
using AuthServer.Contracts.Version1;
using AuthServer.Contracts.Version1.RequestContracts.Queries;
using AuthServer.Models.Services.Interfaces;
using Microsoft.AspNetCore.WebUtilities;

namespace AuthServer.Models.Services
{
    public class URIService : IURIService
    {
        private readonly string _baseURI;

        public URIService(string baseURI)
        {
            _baseURI = baseURI; 
        }

        public Uri GetPaginationURI(string path, PaginationQuery query = null)
        {
            var URI = new Uri(_baseURI);

            if (query == null){
                return URI;
            }

            var endpointURI = $"{_baseURI}{path}";

            var modifiedURI = QueryHelpers.AddQueryString(endpointURI,"pageNumber", query.PageNumber.ToString());
            modifiedURI = QueryHelpers.AddQueryString(modifiedURI, "pageSize", query.PageSize.ToString());

            return new Uri(modifiedURI);
        }

        public Uri GetOrganisationURI(string organisationID)
        {
            return new Uri(_baseURI + "/" + ApiRoutes.Organisations.Get.Replace("{ID}", organisationID));
        }

        public Uri GetPolicyURI(string policyName)
        {
            return new Uri(_baseURI + "/" + ApiRoutes.Policies.Get.Replace("{Name}", policyName));
        }

        public Uri GetUserURI(string userID)
        {
            return new Uri(_baseURI + "/" + ApiRoutes.Users.Get.Replace("{ID}", userID));
        }
    }
}