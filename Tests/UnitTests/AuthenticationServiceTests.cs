using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AuthServer.Contracts.Version1;
using AuthServer.Models.DTOs;
using AuthServer.Models.Entities;
using AuthServer.Models.Services;
using AuthServer.Models.Services.Interfaces;
using AuthServer.Persistence;
using AuthServer.Persistence.Repositories.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Tests.Helpers;
using Xunit;
using static AuthServer.Contracts.Version1.RequestContracts.Authentication;
using static AuthServer.Contracts.Version1.ResponseContracts.Authentication;

namespace Tests.UnitTests
{
    public class AuthenticationServiceTests : IDisposable
    {
        private readonly Mock<IUnitOfWork> _unitOfWork = new Mock<IUnitOfWork>();
        private readonly IMapper _mapper = MockConfigurator.MockMapper();
        private readonly Mock<UserManager<User>> _userManager = MockConfigurator.MockUserManager();
        private readonly RegistrationRequest _registrationRequest = MockConfigurator.MockRegistrationRequest();
        private readonly LoginRequest _loginRequest = MockConfigurator.MockLoginRequest();
        private readonly IAuthenticationService _authService;

        public AuthenticationServiceTests()
        {
            _authService = new AuthenticationService(
                _unitOfWork.Object,
                _mapper,
                _userManager.Object
                );
        }

        [Fact]
        public async Task ValidateRegistrationAsync_EmailExists_OrganisationDoesnt_Test()
        {
            //arrange 
            var organisation = new Mock<Organisation>();
            var user = MockConfigurator.MockUser();
            _userManager.Setup(uManager => uManager.FindByEmailAsync(_registrationRequest.Email))
            .ReturnsAsync(user);

            //assert
            Assert.Collection(await _authService.ValidateRegistrationAsync(_registrationRequest, null, user),
            item => Assert.Contains("Email is already registered", item.Message),
            item => Assert.Contains("No Organisation found with the given Organisation Name", item.Message));
        }

        [Fact]
        public async Task ValidateRegistrationAsync_EmailExists_OrganisationExists_Test()
        {
            //arrange 
            var organisation = new Mock<Organisation>();
            var user = MockConfigurator.MockUser();
            _userManager.Setup(uManager => uManager.FindByEmailAsync(_registrationRequest.Email))
            .ReturnsAsync(user);

            //assert
            Assert.Collection(await _authService.ValidateRegistrationAsync(_registrationRequest, organisation.Object, user),
            item => Assert.DoesNotContain("No Organisation found with the given Organisation Name", item.Message));
        }

        [Fact]
        public async Task ValidateRegistrationAsync_SuccessScenario_Test()
        {
             //arrange 
            var organisation = new Mock<Organisation>();
            var user = MockConfigurator.MockUser();

            _userManager.Setup(uManager => uManager.FindByEmailAsync(_registrationRequest.Email))
            .ReturnsAsync((User)null);

            _userManager.Setup(uManager => uManager.CreateAsync(user, DataFixtures.Password))
            .ReturnsAsync(new IdentityResult());

            //assert
            Assert.Empty(await _authService.ValidateRegistrationAsync(_registrationRequest, organisation.Object, user));
        }

        [Fact]
        public async Task ValidateRegistrationAsync_RegistrationConstraintNotMetScenario_Test()
        {
            //arrange 
            var organisation = new Mock<Organisation>();
            var user = MockConfigurator.MockUser();
            _userManager.Setup(uManager => uManager.FindByEmailAsync(_registrationRequest.Email))
            .ReturnsAsync((User)null);

            var errors = new IdentityError[1]{
                new IdentityError{
                    Description = "Passwords must have at least one non alphanumeric character."
                    }
                };

            _userManager.Setup(uManager => uManager.CreateAsync(user, DataFixtures.Password))
            .ReturnsAsync(IdentityResult.Failed(errors));

            Assert.Collection(await _authService.ValidateRegistrationAsync(_registrationRequest, organisation.Object, user),
            item => Assert.Contains("Passwords must have at least one non alphanumeric character.", item.Message));
        }

        [Fact]
        public async Task RegisterUserAsyncTest()
        {
            var user = MockConfigurator.MockUser();
            var policy = MockConfigurator.MockPolicy();
            _userManager.Setup(uManager => uManager.GetClaimsAsync(user))
            .ReturnsAsync(new List<Claim>{new Claim(policy.Claim, "true")});

            var organisation = new Organisation{
                Users = new List<User>{user},
                Policies = new List<Policy>{MockConfigurator.MockPolicy()}
                };

            var response = await _authService.RegisterUserAsync(_registrationRequest, organisation, user);
            Assert.Null(response.RefreshToken);
            Assert.Null(response.Token);
            Assert.Same(response.Email, user.Email);
        }

        [Fact]
        public async Task LoginUserAsync_FailureScenario_Test()
        {
            var user = MockConfigurator.MockUser();
            _userManager.Setup(uManager => uManager.CheckPasswordAsync(user, _loginRequest.Password))
            .ReturnsAsync(false);

            Assert.Null(await _authService.LoginUserAsync(_loginRequest, user));        
        }

        [Fact]
        public async Task LoginUserAsync_SuccessScenario_Test(){

            var user = MockConfigurator.MockUser();
            _userManager.Setup(uManager => uManager.CheckPasswordAsync(user, _loginRequest.Password))
            .ReturnsAsync(true);
            
            var response = await _authService.LoginUserAsync(_loginRequest, user);

            Assert.IsType<LoginResponse>(response);
            Assert.NotNull(response);
            Assert.Same(response.Email, _loginRequest.Email);
            Assert.Null(response.RefreshToken);
            Assert.Null(response.Token);            
        }

        public void Dispose()
        {
        }
    }
}