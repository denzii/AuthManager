using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthServer.Models.Entities;
using AuthServer.Contracts.Version1.ResponseContracts;

namespace AuthServer.Configurations.AutoMappings
{
	public class EntityToResponseProfile : Profile
	{
		public EntityToResponseProfile()
		{
			CreateMap<User, Authentication.LoginResponse>();
			CreateMap<Organisation, Organisations.PostResponse>();
			CreateMap<User, Authentication.RegistrationResponse>();
		}
	}
}
