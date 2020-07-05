using AuthServer.Models.Entities;
using AuthServer.Models.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthServer.Models.DTOs;
using static AuthServer.Contracts.Version1.ResponseContracts.Authentication;
using AuthServer.Persistence;
using static AuthServer.Contracts.Version1.RequestContracts.Authentication;
using AutoMapper;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using AuthServer.Contracts.Version1;
using AuthServer.Configurations;
using static AuthServer.Contracts.Version1.ResponseContracts.Errors;

namespace AuthServer.Models.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;

        public AuthenticationService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            UserManager<User> userManager
            )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<List<ErrorResponse>> ValidateRegistrationAsync(RegistrationRequest request, Organisation organisation, User newUser)
        {
            User user = await _userManager.FindByEmailAsync(request.Email);
            var errorResponses = new List<ErrorResponse>();

            if (user != null)
            {
                errorResponses.Add(new ErrorResponse { Message = "Email is already registered" });
            }

            if (organisation == null)
            {
                errorResponses.Add(new ErrorResponse { Message = "No Organisation found with the given Organisation Name" });
            }

            if (errorResponses.Any())
            {
                return errorResponses;
            }

            IdentityResult result = await _userManager.CreateAsync(newUser, request.Password);

            return result.Errors.Select(error => new ErrorResponse { Message = error.Description }).ToList();
        }

        public async Task<RegistrationResponse> RegisterUserAsync(RegistrationRequest request, Organisation organisation, User newUser)
        {
            if (!organisation.Users.Skip(1).Any() && organisation.Users.Where(user => user.Email == newUser.Email).Any())
            {
                Policy adminPolicy = organisation.Policies.FirstOrDefault();
                newUser.Policy = adminPolicy;

                await _userManager.AddClaimAsync(newUser, new Claim(adminPolicy.Claim, "true"));
            }

            await _unitOfWork.CompleteAsync();

            return _mapper.Map<RegistrationResponse>(newUser);
        }

        public async Task<LoginResponse> LoginUserAsync(LoginRequest request, User user)
        {
            bool loginValid = await _userManager.CheckPasswordAsync(user, request.Password);

            if (!loginValid)
            {
                return null;
            }

            LoginResponse response = _mapper.Map<LoginResponse>(user);

            return response;
        }
    }
}
