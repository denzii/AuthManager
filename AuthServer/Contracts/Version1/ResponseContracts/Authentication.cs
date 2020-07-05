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
        public class LoginResponseExample : IExamplesProvider<Response<LoginResponse>>
        {
            public Response<LoginResponse> GetExamples()
            {
                var responseExample = new LoginResponse
                {
                    Token = DataFixtures.Token1,
                    RefreshToken = DataFixtures.RefreshToken,
                    Email = DataFixtures.Email3
                };

                return new Response<LoginResponse>(responseExample);
            }
        }

        public class RegistrationResponse
        {
            public string Token { get; set; }
            public string RefreshToken { get; set; }
            public string ID { get; set; }
            public string Email { get; set; }
            public DateTime RegisteredOn { get; set; }
        }
        public class RegistrationResponseExample : IExamplesProvider<Response<RegistrationResponse>>
        {
            public Response<RegistrationResponse> GetExamples()
            {
                var responseExample = new RegistrationResponse
                {
                    Token = DataFixtures.Token1,
                    RefreshToken = DataFixtures.RefreshToken,
                    ID = DataFixtures.Identifier,
                    Email = DataFixtures.Email2,
                    RegisteredOn = DataFixtures.Now
                };

                return new Response<RegistrationResponse>(responseExample);
            }
        }

        public class RefreshTokenResponse
		{
            public string Token { get; set; }
            public string RefreshToken { get; set; }
            public string Email { get; set; }
        }
		public class RefreshTokenResponseExample: IExamplesProvider<Response<RefreshTokenResponse>>
        {
            public Response<RefreshTokenResponse> GetExamples()
            {
                var responseExample = new RefreshTokenResponse
                {
                    Token = DataFixtures.Token1,
                    RefreshToken = DataFixtures.RefreshToken,
                    Email = DataFixtures.Email3
                };

                return new Response<RefreshTokenResponse>(responseExample);
            }
		}
    }
}
