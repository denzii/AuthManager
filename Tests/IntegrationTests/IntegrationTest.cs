using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AuthServer;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using AuthServer.Contracts.Version1;
using AuthServer.Persistence;
using AuthServer.Persistence.Contexts;
using static AuthServer.Contracts.Version1.RequestContracts.Authentication;
using static AuthServer.Contracts.Version1.ResponseContracts.Authentication;
using AuthServer.Contracts.Version1.RequestContracts;
using AuthServer.Models.Entities;
using System.Linq;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Tests.IntegrationTests
{
    public class IntegrationTest
    {
        protected readonly HttpClient _httpClient;
        private readonly IServiceProvider _serviceProvider;

        protected const string ORGANISATION_NAME = "testorganisation";
        protected const string USER_EMAIL = "inmemoryuser@test.com";
        protected const string USER_PASSWORD = "pwd1";

        public IntegrationTest()
        {
            var appFactory = new WebApplicationFactory<Startup>()

            .WithWebHostBuilder(builder => {
                builder.ConfigureServices(services => {

                    var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AuthServerContext>));
                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }
                    services.AddDbContextPool<AuthServerContext>(options => {
                        options.UseInMemoryDatabase("TestDB")
                        .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));
                    });                    
                });
            });

            _serviceProvider = appFactory.Services;
            _httpClient = appFactory.CreateClient();
        }

        protected async Task AuthenticateAsync()
        {
            var registrationResponse = await RequestRegistration();

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", await GetJwtAsync(registrationResponse));
        }
        
        protected async Task<HttpResponseMessage> RequestRegistration()
        {

            await _httpClient.PostAsJsonAsync(ApiRoutes.Organisations.Post, new Organisations.PostRequest
            {
                Name = ORGANISATION_NAME
            });

            return await _httpClient.PostAsJsonAsync(ApiRoutes.Authentication.Register, new RegistrationRequest
            {
                Email = USER_EMAIL,
                Password = USER_PASSWORD,
                FirstName = "inmemory",
                LastName = "user",
                Sex = "f",
                OrganisationName = ORGANISATION_NAME        
            });
        }

        protected async Task<string> GetJwtAsync(HttpResponseMessage responseMessage)
        {
            var registrationResponse = await responseMessage.Content.ReadAsAsync<RegistrationResponse>();

            return registrationResponse.Token;
        }
    }
}