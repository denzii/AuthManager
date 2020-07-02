using System.Collections.Generic;
using System.Linq;
using AuthServer.Contracts.Version1.RequestContracts.Queries;
using AuthServer.Contracts.Version1.ResponseContracts;
using AuthServer.Models.DTOs;
using AuthServer.Models.Services.Interfaces;

namespace AuthServer.Models.Helpers
{
    public class Paginator
    {
        public static PagedResponse<T> CreatePagedResponse<T>(IURIService URIService, PageFilter filter, List<T> responses, string path){
            string nextPage = filter.PageNumber >= 1 
            ? URIService.GetPaginationURI(path, new PaginationQuery(filter.PageNumber +1, filter.PageSize)).ToString()
            : null;

            string previousPage = filter.PageNumber - 1 >= 1 
            ? URIService.GetPaginationURI(path, new PaginationQuery(filter.PageNumber -1, filter.PageSize)).ToString()
            : null;
            
            return new PagedResponse<T>(){
                    Data = responses,
                    PageNumber = filter.PageNumber >= 1 ? filter.PageNumber : (int?) null,
                    PageSize = filter.PageSize >= 1 ? filter.PageSize : (int?) null,
                    NextPage = responses.Any() ? nextPage : null,
                    PreviousPage = previousPage
                    };
        }       
    }
}