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
using System.Net.Mime;
using static AuthServer.Contracts.Version1.ResponseContracts.Errors;
using System.Security.Claims;
using AuthServer.Models.Entities;
using System.Collections;
using System.Collections.Generic;

namespace AuthServer.Controllers.Version1
{
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    public class AuthenticationController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthenticationService _authService;

        public AuthenticationController(IUnitOfWork unitOfWork, IAuthenticationService authService)
        {
            _unitOfWork = unitOfWork;
            _authService = authService;
        }

        /// <summary>
        /// Validates given data and creates a new user for the given organisation.
        /// If the user to be created is the first one within that organisation, the user is created with the Admin role by default.
        /// </summary>
        ///<response code="201"> Creates a user</response>
        ///<response code="400"> Failed due to an error and user was not created.</response>        
        [HttpPost(ApiRoutes.Authentication.Register)]
        [ProducesResponseType(typeof(RegistrationResponse), 201)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> Register([FromBody] RegistrationRequest request)
        {
            Organisation organisation = await _unitOfWork.OrganisationRepository.GetByNameAsync(request.OrganisationName);
            User newUser =  _unitOfWork.UserRepository.CreateUser(request, organisation, null);

            List<ErrorResponse> errorResponses = await _authService.ValidateRegistrationAsync(request, organisation, newUser);
            
            if (errorResponses.Any()){
                return BadRequest(errorResponses);
            }

            RegistrationResponse response = await _authService.RegisterUserAsync(request, organisation, newUser);
                
            string baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
            string locationUri = baseUrl + "/" + ApiRoutes.Users.Get.Replace("{ID}", response.Id.ToString());

            return Created(locationUri, response);
        }

        /// <summary>
        /// Authenticates user to provide a JSON Web Token alongside a Refresh Token.
        /// If the user to be created is the first one within that organisation, the user is created with the Admin role by default.
        /// </summary>
        ///<response code="200"> Provides a JWT and Refresh Token.</response>
        ///<response code="400"> Incorrect data, failed to authenticate.</response>
        [HttpPost(ApiRoutes.Authentication.Login)]
        [ProducesResponseType(typeof(LoginResponse), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!await _unitOfWork.UserRepository.UserWithEmailExistsAsync(request.Email))
            {
                return BadRequest( new ErrorResponse { Message = "User with the given email address could not be found." });
            }

            LoginResponse response = await Task.Run(() => _authService.LoginUserAsync(request));

            return Ok(response);
        }

        /// <summary>
        /// Prolongs the login session by checking JSON Web Token and Refresh Token details.
        /// </summary>
        ///<response code="200"> Provides a JWT and Refresh Token.</response>
        ///<response code="400"> Tokens are invalid or not suitable for refresh, session cannot be prolonged.</response>
        [HttpPost(ApiRoutes.Authentication.Refresh)]
        [ProducesResponseType(typeof(RefreshTokenResponse), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            ClaimsPrincipal validatedToken = _authService.IsTokenAuthentic(request.Token);

            if(validatedToken == null)
            {
                return BadRequest(new ErrorResponse { Message = "This token has been tampered with." });
            }

            RefreshToken refreshToken = await _authService.CanTokenBeRefreshed(validatedToken, request.RefreshToken);

            if (refreshToken == null){
                return BadRequest( new ErrorResponse { Message = "Invalid Token, cannot refresh." });
            }
            
            string organisationID = validatedToken.Claims.Single(x => x.Type == "OrganisationID").Value;

            RefreshTokenResponse response = await _authService.RefreshTokenAsync(validatedToken, refreshToken, organisationID);

            return Ok(response);
        }
    }
}