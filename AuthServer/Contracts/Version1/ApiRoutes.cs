using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthServer.Contracts.Version1
{

	public static class ApiRoutes
	{
		public const string EndpointRoot = "api";
		public const string APIVersion = "v1";
		public const string BaseRoute = EndpointRoot + "/" + APIVersion;
		public const string Identifier = "{ID}";
		public const string Name = "{Name}";


		//TODO: Refactor the non-RESTful endpoints to be more REST appropriated
		public static class Authentication
		{
			public const string Register = BaseRoute + "/" + nameof(Authentication) + "/" + "Register";
			public const string Login = BaseRoute + "/" + nameof(Authentication) + "/" + "Login";
			public const string Refresh = BaseRoute + "/" + nameof(Authentication) + "/" + "Refresh";
		}

		public static class Authorization
		{
			public const string Assign = BaseRoute + "/" + nameof(Authorization) + "/" + "Assign";
			public const string Unassign = BaseRoute + "/" + nameof(Authorization) + "/" + "Unassign";
		}

		public static class Organisations
		{
			public const string GetAll = BaseRoute + "/" + nameof(Organisations);
			public const string Post = BaseRoute + "/" + nameof(Organisations);
			public const string Delete = BaseRoute + "/" + nameof(Organisations) + "/" + Identifier;
			public const string Get = BaseRoute + "/" + nameof(Organisations) + "/" + Identifier;
			public const string Patch = BaseRoute + "/" + nameof(Organisations) + "/" + Identifier;
		}

		public static class Policies
		{
			public const string GetAll = BaseRoute + "/" + nameof(Policies) + "/";
			public const string Post = BaseRoute + "/" + nameof(Policies);
            public const string Get = BaseRoute + "/" + nameof(Policies) + "/" + Name;
        }


		public static class Users
		{
			public const string GetAll = BaseRoute + "/" + nameof(Users);
			public const string Post = BaseRoute + "/" + nameof(Users);
			public const string Delete = BaseRoute + "/" + nameof(Users) + "/" + Identifier;
			public const string Get = BaseRoute + "/" + nameof(Users) + "/" + Identifier;
			public const string Patch = BaseRoute + "/" + nameof(Users) + "/" + Identifier;

		}
	}
}
