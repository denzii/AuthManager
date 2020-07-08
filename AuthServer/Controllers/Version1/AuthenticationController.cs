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
using AuthServer.Contracts.Version1.ResponseContracts;
using AuthServer.Models.Helpers;

namespace AuthServer.Controllers.Version1
{
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    public class AuthenticationController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthenticationService _authService;

        private readonly IURIService _URIService;

        private readonly ITokenService _tokenService;

        public AuthenticationController(
            IUnitOfWork unitOfWork,
            IAuthenticationService authService,
            IURIService URIService,
            ITokenService tokenService
            )
        {
            _unitOfWork = unitOfWork;
            _authService = authService;
            _URIService = URIService;
            _tokenService = tokenService;
        }

        /// <summary>
        /// Validates given data and creates a new user for the given organisation.
        /// If the user to be created is the first one within that organisation, the user is created with the Admin role by default.
        /// </summary>
        ///<response code="201"> Creates a user</response>
        ///<response code="400"> Failed due to an error and user was not created.</response>        
        [HttpPost(ApiRoutes.Authentication.Register)]
        [ProducesResponseType(typeof(Response<RegistrationResponse>), 201)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> Register([FromBody] RegistrationRequest request)
        {
            //TODO replace organisation name in the request with organisation UUID
            // this could reduce the change of brute force success on unrightfully registering users on an organisation
            Organisation organisation = await _unitOfWork.OrganisationRepository.GetByNameAsync(request.OrganisationName);
            User newUser = _unitOfWork.UserRepository.CreateUser(request, organisation, null);

            List<ErrorResponse> errorResponses = await _authService.ValidateRegistrationAsync(request, organisation, newUser);

            if (errorResponses.Any())
            {
                return BadRequest(errorResponses);
            }
            var transaction = _unitOfWork.UserRepository.BeginTransaction();

            RegistrationResponse registrationResponse = await _authService.RegisterUserAsync(request, organisation, newUser);
            var locationURI = _URIService.GetUserURI(registrationResponse.ID);

            Dictionary<string, string> tokens = await _tokenService.GetTokensAsync(newUser);
            await transaction.CommitAsync();

            tokens.TryGetValue("SecurityToken", out string securityToken);
            tokens.TryGetValue("RefreshToken", out string refreshToken);

            registrationResponse.Token = securityToken;
            registrationResponse.RefreshToken = refreshToken;

            return Created(locationURI, new Response<RegistrationResponse>(registrationResponse));
        }

        /// <summary>
        /// Authenticates user to provide a JSON Web Token alongside a Refresh Token.
        /// </summary>
        ///<response code="200"> Provides a JWT and Refresh Token.</response>
        ///<response code="400"> Incorrect data, failed to authenticate.</response>
        [HttpPost(ApiRoutes.Authentication.Login)]
        [ProducesResponseType(typeof(Response<LoginResponse>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!await _unitOfWork.UserRepository.UserWithEmailExistsAsync(request.Email))
            {
                return BadRequest(new ErrorResponse { Message = "User with the given email address could not be found." });
            }

            User user = await _unitOfWork.UserRepository.GetByEmailAsync(request.Email);

            LoginResponse loginResponse = await _authService.LoginUserAsync(request, user);

            if (loginResponse == null)
            {
                return BadRequest("Password is not valid");
            }
            Dictionary<string, string> tokens = tokens = await _tokenService.GetTokensAsync(user);

            tokens.TryGetValue("SecurityToken", out string securityToken);
            tokens.TryGetValue("RefreshToken", out string refreshToken);

            loginResponse.Token = securityToken;
            loginResponse.RefreshToken = refreshToken;

            return Ok(new Response<LoginResponse>(loginResponse));
        }

        /// <summary>
        /// Prolongs the login session by checking JSON Web Token and Refresh Token details.
        /// </summary>
        ///<response code="200"> Provides a JWT and Refresh Token.</response>
        ///<response code="400"> Tokens are invalid or not suitable for refresh, session cannot be prolonged.</response>
        [HttpPost(ApiRoutes.Authentication.Refresh)]
        [ProducesResponseType(typeof(Response<RefreshTokenResponse>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            ClaimsPrincipal validatedToken = _tokenService.IsTokenAuthentic(request.Token);

            if (validatedToken == null)
            {
                return BadRequest(new ErrorResponse { Message = "This token has been tampered with." });
            }

            RefreshToken refreshToken = await _tokenService.CanTokenBeRefreshedAsync(validatedToken, request.RefreshToken);

            if (refreshToken == null)
            {
                return BadRequest(new ErrorResponse { Message = "Invalid Token, cannot refresh." });
            }

            string organisationID = ClaimHelper.GetNamedClaim(validatedToken, "OrganisationID");

            var transaction = _unitOfWork.RefreshTokenRepository.BeginTransaction();

            RefreshTokenResponse refreshTokenResponse = await _tokenService.RefreshTokenAsync(validatedToken, refreshToken, organisationID);

            transaction.Commit();

            return Ok(new Response<RefreshTokenResponse>(refreshTokenResponse));
        }
    }
}