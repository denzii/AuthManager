using System;
using System.IO;
using System.Threading.Tasks;
using AuthServer.Configurations.Middlewares;
using Microsoft.AspNetCore.Http;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Tests.UnitTests
{
    public class ExceptionMiddlewareTests : IDisposable
    {
        private readonly Mock<RequestDelegate> _next = new Mock<RequestDelegate>();

        private readonly DefaultHttpContext _httpContext = new DefaultHttpContext();

        public ExceptionMiddlewareTests()
        {
        }

        [Fact]
        public async Task InvokeAsync_ExceptionScenario_Test()
        {
            var middleware = new ExceptionMiddleware((innerHttpContext) => throw new Exception("Unit Test Exception"));
            _httpContext.Response.Body = new MemoryStream(); //set body as stream

            await middleware.InvokeAsync(_httpContext); // invoking the middleware fills up the stream

            Assert.Equal(_httpContext.Response.StatusCode, StatusCodes.Status500InternalServerError);
            Assert.Equal("It worked on my machine :/", ReadContextResponse());
        }

        [Fact]
        public async Task InvokeAsync_SuccessScenario_Test()
        {
            var middleware = new ExceptionMiddleware(_next.Object);
            _httpContext.Response.Body = new MemoryStream(); //set body as stream

            await middleware.InvokeAsync(_httpContext); // invoking the middleware fills up the stream

            Assert.Equal(_httpContext.Response.StatusCode, StatusCodes.Status200OK);
            Assert.Null(ReadContextResponse());
        }

        public string ReadContextResponse()
        {
            _httpContext.Response.Body.Seek(0, SeekOrigin.Begin);  //set stream position to the beginning

            var reader = new StreamReader(_httpContext.Response.Body); //configure reader on the stream
            string streamText = reader.ReadToEnd(); //get the stream content into text format
            var response = (JObject)JsonConvert.DeserializeObject(streamText); //serialize text into json linq object

            if (response == null)
            {
                return null;
            }

            return (response["Message"]).ToString(); //access json field
        }

        public void Dispose()
        {
        }
    }
}