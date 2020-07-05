using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AuthServer.Configurations.AutoMappings;
using AuthServer.Contracts.Version1;
using AuthServer.Contracts.Version1.RequestContracts;
using AuthServer.Models.DTOs;
using AuthServer.Models.Entities;
using AuthServer.Models.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;
using static AuthServer.Contracts.Version1.RequestContracts.Authentication;

namespace Tests.Helpers
{
    public static class MockConfigurator
    {
        public static Mock<UserManager<User>> MockUserManager()
        {
            var user = MockUser();
            var mockUserStore = new Mock<IUserStore<User>>();
            mockUserStore.Setup(x => x.FindByIdAsync(user.Id.ToString(), CancellationToken.None)).ReturnsAsync(user);

            return new Mock<UserManager<User>>(mockUserStore.Object, null, null, null, null, null, null, null, null);
        }

        public static SigningCredentials MockSigningCredentials()
        {
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(DataFixtures.TokenSecret));
            
            return new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256Signature);
        }

        public static ActionExecutingContext MockActionExecutingContext()
        {
            
            var actionContext = new ActionContext(
                new DefaultHttpContext(),
                (new Mock<RouteData>()).Object,
                (new Mock<ActionDescriptor>()).Object,
                new ModelStateDictionary()
            );

            return new ActionExecutingContext(
                actionContext,
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                new Mock<Controller>()
            );
        }

        public static SecurityTokenDescriptor MockSecurityTokenDescriptor()
        {
            return new SecurityTokenDescriptor()
            {
                Expires = DateTime.UtcNow.Add(DataFixtures.TokenLifetime),
                SigningCredentials =  MockSigningCredentials()
            };
        }

        public static User MockUser()
        {
            var organisation = new Organisation { ID = DataFixtures.GUID };
            var request = MockRegistrationRequest();

            return new User
            {
                Id = Guid.NewGuid().ToString(),
                UserName = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Sex = request.Sex,
                Email = request.Email,
                RegisteredOn = DateTime.Now,
                Organisation = organisation
            };
        }

        public static IMapper MockMapper()
        {
            var configuration = new MapperConfiguration(config => {
                config.AddProfile(new ModelToResponseProfile());
                config.AddProfile(new RequestToModelProfile());
            });

            return new Mapper(configuration);
        }

        public static LoginRequest MockLoginRequest()
        {
            return new LoginRequest
            {
                Email = DataFixtures.Email1,
                Password = DataFixtures.Password
            };
        }

        public static Mock<ITokenService> MockTokenService()
        {
            return new Mock<ITokenService>();
        }

        public static RefreshToken MockRefreshToken()
        {
            return new RefreshToken
			{
                Token = DataFixtures.GUID,
				JwtID = DataFixtures.GUID,
				UserID = DataFixtures.GUID,
				CreatedAt = DateTime.UtcNow,
				ExpiryDate = DateTime.UtcNow.AddMonths(6),
                Used = false,
                Invalidated = false
			};
        }

        public static Authentication.RegistrationRequest MockRegistrationRequest()
        {
            return new RegistrationRequest
            {
                Email = DataFixtures.Email1,
                Password = DataFixtures.Password,
                FirstName = DataFixtures.Firstname1,
                LastName = DataFixtures.Lastname,
                Sex = DataFixtures.Male,
                OrganisationName = DataFixtures.Organisation
            };
        }

        public static TokenValidationParameters MockValidationParameters()
        {
            return new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(DataFixtures.TokenSecret)),
                ValidateIssuer = false,
                ValidateAudience = false,
                RequireExpirationTime = false,
                ValidateLifetime = true
            };
        }

        public static JWTBearerAuthConfig MockJwtBearerConfig()
        {
            return new JWTBearerAuthConfig
            {
                Secret = DataFixtures.TokenSecret,
                TokenLifetime = new TimeSpan(0, 5, 0)
            };
        }

        public static Policy MockPolicy()
        {
            return new Policy
            {
                Name = DataFixtures.PolicyName1,
                Claim = DataFixtures.PolicyClaim1
            };
        }
    }
}