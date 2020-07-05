
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using System.Threading.Tasks;
using AuthServer.Configurations.Middlewares;
using AuthServer.Contracts.Version1;
using AuthServer.Models.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Tests.Helpers;
using Xunit;

namespace Tests.UnitTests
{
    public class ApiKeyAuthAttributeTests : IDisposable
    {

        private readonly Mock<ActionExecutionDelegate> _next = new Mock<ActionExecutionDelegate>();

        private readonly ApiKeyAuthAttribute _middleware;

        public ApiKeyAuthAttributeTests()
        {
            _middleware = new ApiKeyAuthAttribute();
        }

        [Fact]
        public async Task OnActionExecutionAsync_ApiKeyNotPresent_Test()
        {
            //arrange
            ActionExecutingContext context = MockConfigurator.MockActionExecutingContext();

            //act
            await _middleware.OnActionExecutionAsync(context, _next.Object);

            //assert
            Assert.IsType<UnauthorizedResult>(context.Result);
        }

        [Fact]
        public async Task OnActionExecutionAsync_SuccessScenario_Test()
        {
            //arrange
            var context = MockConfigurator.MockActionExecutingContext();

            var requestServices = new Mock<IServiceProvider>();
            var config = new Mock<IConfiguration>();
            var configSection = new Mock<IConfigurationSection>();
            
            requestServices.Setup(rServices => rServices.GetService(typeof(IConfiguration))).Returns(config.Object);

            // configSection.Object.Value = DataFixtures.ApiKeyValue;
            configSection.Setup(cSection => cSection.Value).Returns(DataFixtures.ApiKeyValue);

            config.Setup(config => config.GetSection(It.IsAny<String>())).Returns(configSection.Object);

            context.HttpContext.RequestServices = requestServices.Object;

            //add a header to emulate successful response
            context.HttpContext.Request.Headers.Add(ApiKeyAuthAttribute.ApiKeyHeaderName, $"ApiKey {DataFixtures.ApiKeyValue}");          

            //act
            await _middleware.OnActionExecutionAsync(context, _next.Object);

            //assert
            Assert.Null(context.Result);
        }

        public void Dispose()
        {

        }
    }
}