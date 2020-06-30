using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthServer.Models.Entities;
using AuthServer.Contracts.Version1.ResponseContracts;

namespace AuthServer.Configurations.AutoMappings
{
	public class ModelToResponseProfile : Profile
	{
		public ModelToResponseProfile()
		{
			CreateMap<User, Authentication.LoginResponse>();

			CreateMap<Organisation, Organisations.PostResponse>();

			CreateMap<Organisation, Organisations.GetResponse>()
			.ForMember(mappee => mappee.Policies, option => option.MapFrom(mapper => mapper.Policies.Select(policy => policy.Name)));

			CreateMap<User, Authentication.RegistrationResponse>()
			.ForMember(mappee => mappee.ID, option => option.MapFrom(mapper => mapper.Id));

			CreateMap<User, Users.GetResponse>()
			.ForMember(mappee => mappee.PolicyName, option => option.MapFrom(mapper => mapper.Policy != null ? mapper.Policy.Name : null));

			CreateMap<Policy, Policies.GetResponse>()
			.ForMember(mappee => mappee.Users, option => option.MapFrom(mapper => mapper.Users.Select(user => user.Email)));
		}
	}
}
