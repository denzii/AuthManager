using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthServer.Contracts.Version1.ResponseContracts
{
    public static class Authentication
    {
        public class LoginResponse
        {
            public string Token { get; set; }
            public string RefreshToken { get; set; }
            public string Email { get; set; }
        }
        public class LoginResponseExample : IExamplesProvider<LoginResponse>
        {
            public LoginResponse GetExamples()
            {
                return new LoginResponse
                {
                    Token = DataFixtures.Token,
                    RefreshToken = DataFixtures.RefreshToken,
                    Email = DataFixtures.Email3
                };
            }
        }

        public class RegistrationResponse
        {
            public string Token { get; set; }
            public string RefreshToken { get; set; }
            public string Id { get; set; }
            public string Email { get; set; }
            public DateTime RegisteredOn { get; set; }
        }
        public class RegistrationResponseExample : IExamplesProvider<RegistrationResponse>
        {
            public RegistrationResponse GetExamples()
            {
                return new RegistrationResponse
                {
                    Token = DataFixtures.Token,
                    RefreshToken = DataFixtures.RefreshToken,
                    Id = DataFixtures.Identifier,
                    Email = DataFixtures.Email2,
                    RegisteredOn = DataFixtures.Now
                };
            }
        }

        public class RefreshTokenResponse
		{
            public string Token { get; set; }
            public string RefreshToken { get; set; }
            public string Email { get; set; }
        }
		public class RefreshTokenResponseExample: IExamplesProvider<RefreshTokenResponse>
        {
            public RefreshTokenResponse GetExamples()
            {
                return new RefreshTokenResponse
                {
                    Token = DataFixtures.Token,
                    RefreshToken = DataFixtures.RefreshToken,
                    Email = DataFixtures.Email3
                };
            }
		}
    }
}
