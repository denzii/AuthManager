using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using AuthServer.Contracts.Version1;
using AuthServer.Models.Entities;
using Xunit;
using static AuthServer.Contracts.Version1.ApiRoutes;
using static AuthServer.Contracts.Version1.ResponseContracts.Authentication;
using static AuthServer.Contracts.Version1.ResponseContracts.Organisations;

namespace Tests.IntegrationTests
{
    public class AuthenticationControllerTest : IntegrationTest
    {
        // [Fact]
        // public async Task RegistrationTest()
        // {                       
            // var httpResponse = await RequestRegistration();
            // httpResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            
            // RegistrationResponse response = await httpResponse.Content.ReadAsAsync<RegistrationResponse>();

            // response.Email.Should().Be(USER_EMAIL);
            // response.Errors.Should().BeNull();

            // httpResponse = await RequestRegistration();
            // httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            // response = await httpResponse.Content.ReadAsAsync<RegistrationResponse>();

            // response.Errors.SingleOrDefault().Should().Be("No Organisation found with the given Organisation Name");
        // }
    }
}