using System;
using System.Threading.Tasks;
using AutoMapper;
using System.Linq;
using AuthServer.Contracts.Version1;
using AuthServer.Models.Services.Interfaces;
using AuthServer.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static AuthServer.Contracts.Version1.RequestContracts.Authentication;
using static AuthServer.Contracts.Version1.ResponseContracts.Authentication;
using Serilog;

namespace AuthServer.Controllers.Version1
{
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthenticationService _authService;

        public AuthenticationController(IUnitOfWork unitOfWork, IAuthenticationService authService)
        {
            _unitOfWork = unitOfWork;
            _authService = authService;
        }

        [HttpPost(ApiRoutes.Authentication.Register)]
        public async Task<IActionResult> Register([FromBody] RegistrationRequest request)
        {
            RegistrationResponse response = await _authService.RegisterUserAsync(request);
                
            if (response.Errors == null){
                string baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
                string locationUri = baseUrl + "/" + ApiRoutes.Users.Get.Replace("{ID}", response.Id.ToString());

                return Created(locationUri, response);
            }

            return Ok(response);
        }

        [HttpPost(ApiRoutes.Authentication.Login)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            LoginResponse response = await Task.Run(() => _authService.LoginUserAsync(request));

            if (response.Error != null)
            {
                return BadRequest(response.Error);
            }

            return Ok(response);
        }

        [HttpPost(ApiRoutes.Authentication.Refresh)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            RefreshTokenResponse response = await _authService.RefreshTokenAsync(request.Token, request.RefreshToken);

            if (response.Error != null)
            {
                return BadRequest(response.Error);
            }

            return Ok(response);
        }
    }
}