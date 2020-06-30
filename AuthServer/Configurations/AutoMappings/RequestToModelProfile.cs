using AuthServer.Models.Entities;
using AutoMapper;
using AuthServer.Contracts.Version1;
using AuthServer.Contracts.Version1.RequestContracts.Queries;
using AuthServer.Models.DTOs;

namespace AuthServer.Configurations.AutoMappings
{
    public class RequestToModelProfile : Profile
	{
		public RequestToModelProfile()
		{
			CreateMap<PaginationQuery, PageFilter>();
		}
    }
}