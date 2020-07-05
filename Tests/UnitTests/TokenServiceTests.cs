using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using AuthServer.Contracts.Version1;
using AuthServer.Models.DTOs;
using AuthServer.Models.Entities;
using AuthServer.Models.Helpers;
using AuthServer.Models.Services;
using AuthServer.Models.Services.Interfaces;
using AuthServer.Persistence;
using AuthServer.Persistence.Repositories;
using AuthServer.Persistence.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Tests.Helpers;
using Xunit;

namespace Tests.UnitTests
{
    public class TokenServiceTests : IDisposable
    {
        private readonly JWTBearerAuthConfig _jwtConfig = MockConfigurator.MockJwtBearerConfig();

        private readonly TokenValidationParameters _validationParameters = MockConfigurator.MockValidationParameters();

        private readonly SigningCredentials _signingCredentials = MockConfigurator.MockSigningCredentials();

        private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler = new JwtSecurityTokenHandler();

        private readonly Mock<IUnitOfWork> _unitOfWork = new Mock<IUnitOfWork>();

        private readonly Mock<UserManager<User>> _userManager = MockConfigurator.MockUserManager();

        private readonly SecurityTokenDescriptor _securityTokenDescriptor = MockConfigurator.MockSecurityTokenDescriptor();

        private readonly ITokenService _tokenService;

        public TokenServiceTests()
        {
            _tokenService = new TokenService(
                 _jwtSecurityTokenHandler,
                 _unitOfWork.Object,
                 _jwtConfig,
                 _userManager.Object,
                 _signingCredentials,
                 _validationParameters,
                 _securityTokenDescriptor
                );
        }

        [Fact]
        public void IsTokenAuthentic_MatchingSecretsScenario_Test()
        {            
            //(Token signed with matching secret)
            var claimsPrincipal = _tokenService.IsTokenAuthentic(DataFixtures.Token2);

            Assert.IsType<ClaimsPrincipal>(claimsPrincipal);
            Assert.NotNull(claimsPrincipal);
        }

        [Fact]
        public void IsTokenAuthentic_UnmatchingSecretsScenario_Test()
        {   
            //(Token signed with unmatching secret)       
            Assert.Null(_tokenService.IsTokenAuthentic(DataFixtures.Token1));
        }

        [Fact]
        public void IsTokenAuthentic_UnmatchingAlgorithmScenario_Test()
        {   
            //(Token not signed with HmacSha256)
            Assert.Null(_tokenService.IsTokenAuthentic(DataFixtures.Token3));
        }

        [Fact]
        public void IsTokenAuthentic_UnknownTokenFormatScenario_Test()
        {   
            //(Token not of JWT format)
            Assert.Null(_tokenService.IsTokenAuthentic(DataFixtures.Token4));
        }

        [Fact]
        public async Task GetTokensTest(){
            User user = MockConfigurator.MockUser();
            var claims = new List<Claim>{new Claim(DataFixtures.PolicyClaim1, "true")};

            _userManager.Setup(uManager => uManager.GetClaimsAsync(user)).ReturnsAsync(claims);

            var repository = new Mock<IRefreshTokenRepository>();
            
            repository.Setup(repository => repository.CreateRefreshToken(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(MockConfigurator.MockRefreshToken());

            _unitOfWork.Setup(uOfWork => uOfWork.RefreshTokenRepository).Returns(repository.Object);

            var tokens = await _tokenService.GetTokensAsync(user);

            tokens.TryGetValue("SecurityToken", out string securityToken);
            tokens.TryGetValue("RefreshToken", out string refreshToken);

            Assert.NotNull(tokens);
            Assert.NotNull(securityToken);
            Assert.NotNull(refreshToken);
        }

        [Fact]
        public async Task CanTokenBeRefreshedAsync_SuccessScenario_Test()
        {
            //arrange
            var claimsPrincipal = new ClaimsPrincipal();

            ClaimHelper.GetJTI = (claimsPrincipal) => DataFixtures.GUID;
            ClaimHelper.GetUnixExpiryDate = (claimsPrincipal) => DateTimeOffset.Now.ToUnixTimeSeconds();

            var refreshToken = MockConfigurator.MockRefreshToken();
            var repository = new Mock<IRefreshTokenRepository>();


            repository.Setup(repository => repository.GetRefreshToken(It.IsAny<string>()))
            .ReturnsAsync(refreshToken);
            _unitOfWork.Setup(uOfWork => uOfWork.RefreshTokenRepository).Returns(repository.Object);

            //assert
            Assert.NotNull(await _tokenService.CanTokenBeRefreshedAsync(claimsPrincipal, DataFixtures.RefreshToken));
        }
        
        [Fact]
        public async Task CanTokenBeRefreshedAsync_ExpiredTokenScenario_Test()
        {
            //arrange
            var claimsPrincipal = new ClaimsPrincipal();

            ClaimHelper.GetJTI = (claimsPrincipal) => DataFixtures.GUID;
            ClaimHelper.GetUnixExpiryDate = (claimsPrincipal) => DateTimeOffset.Now.ToUnixTimeSeconds();

            var refreshToken = MockConfigurator.MockRefreshToken();
            var repository = new Mock<IRefreshTokenRepository>();

            //assert
            refreshToken.ExpiryDate = DateTime.Now.AddDays(-1);
            repository.Setup(repository => repository.GetRefreshToken(It.IsAny<string>()))
            .ReturnsAsync(refreshToken);
            _unitOfWork.Setup(uOfWork => uOfWork.RefreshTokenRepository).Returns(repository.Object);

            Assert.Null(await _tokenService.CanTokenBeRefreshedAsync(claimsPrincipal, DataFixtures.RefreshToken));
        }

        [Fact]
        public async Task CanTokenBeRefreshedAsync_UnmatchingJTIScenario_Test()
        {
            //arrange
            var claimsPrincipal = new ClaimsPrincipal();

            ClaimHelper.GetJTI = (claimsPrincipal) => DataFixtures.GUID;
            ClaimHelper.GetUnixExpiryDate = (claimsPrincipal) => DateTimeOffset.Now.ToUnixTimeSeconds();

            var refreshToken = MockConfigurator.MockRefreshToken();
            var repository = new Mock<IRefreshTokenRepository>();

            //assert 
            refreshToken.JwtID = Guid.NewGuid().ToString();
            repository.Setup(repository => repository.GetRefreshToken(It.IsAny<string>()))
            .ReturnsAsync(refreshToken);
            _unitOfWork.Setup(uOfWork => uOfWork.RefreshTokenRepository).Returns(repository.Object);            

            Assert.Null(await _tokenService.CanTokenBeRefreshedAsync(claimsPrincipal, DataFixtures.RefreshToken));
        }

        
        [Fact]
        public async Task CanTokenBeRefreshedAsync_UsedTokenScenario_Test()
        {
            //arrange
            var claimsPrincipal = new ClaimsPrincipal();

            ClaimHelper.GetJTI = (claimsPrincipal) => DataFixtures.GUID;
            ClaimHelper.GetUnixExpiryDate = (claimsPrincipal) => DateTimeOffset.Now.ToUnixTimeSeconds();

            var refreshToken = MockConfigurator.MockRefreshToken();
            var repository = new Mock<IRefreshTokenRepository>();

            //assert 
            refreshToken = MockConfigurator.MockRefreshToken();
            refreshToken.Used = true;
            repository.Setup(repository => repository.GetRefreshToken(It.IsAny<string>()))
            .ReturnsAsync(refreshToken);
             _unitOfWork.Setup(uOfWork => uOfWork.RefreshTokenRepository).Returns(repository.Object);            

            Assert.Null(await _tokenService.CanTokenBeRefreshedAsync(claimsPrincipal, DataFixtures.RefreshToken));
        }

        [Fact]
        public async Task CanTokenBeRefreshedAsync_InvalidatedTokenScenario_Test()
        {
            //arrange
            var claimsPrincipal = new ClaimsPrincipal();

            ClaimHelper.GetJTI = (claimsPrincipal) => DataFixtures.GUID;
            ClaimHelper.GetUnixExpiryDate = (claimsPrincipal) => DateTimeOffset.Now.ToUnixTimeSeconds();

            var refreshToken = MockConfigurator.MockRefreshToken();
            var repository = new Mock<IRefreshTokenRepository>();

            //assert 
            refreshToken = MockConfigurator.MockRefreshToken();
            refreshToken.Invalidated = true;
            repository.Setup(repository => repository.GetRefreshToken(It.IsAny<string>()))
            .ReturnsAsync(refreshToken);
             _unitOfWork.Setup(uOfWork => uOfWork.RefreshTokenRepository).Returns(repository.Object);      

            Assert.Null(await _tokenService.CanTokenBeRefreshedAsync(claimsPrincipal, DataFixtures.RefreshToken));
        }

        [Fact]
        public async Task CanTokenBeRefreshedAsync_TokenDoesNotExistScenario_Test()
        {
            //arrange
            var claimsPrincipal = new ClaimsPrincipal();

            ClaimHelper.GetJTI = (claimsPrincipal) => DataFixtures.GUID;
            ClaimHelper.GetUnixExpiryDate = (claimsPrincipal) => DateTimeOffset.Now.ToUnixTimeSeconds();

            var refreshToken = MockConfigurator.MockRefreshToken();
            var repository = new Mock<IRefreshTokenRepository>();

            //assert 
            repository.Setup(repository => repository.GetRefreshToken(It.IsAny<string>()))
            .ReturnsAsync((RefreshToken)null);
            _unitOfWork.Setup(uOfWork => uOfWork.RefreshTokenRepository).Returns(repository.Object);

            Assert.Null(await _tokenService.CanTokenBeRefreshedAsync(claimsPrincipal, DataFixtures.RefreshToken));
        }

        [Fact]
        public async Task RefreshTokenAsyncTest()
        {            
            //arrange
            var claimsPrincipal = new ClaimsPrincipal();
            var organisationID = Guid.NewGuid().ToString();
            var user = MockConfigurator.MockUser();

            var claims = new List<Claim>{new Claim(DataFixtures.PolicyClaim1, "true")};
            _userManager.Setup(uManager => uManager.GetClaimsAsync(user)).ReturnsAsync(claims);

            var refreshToken = MockConfigurator.MockRefreshToken();
            var refreshTokenRepository = new Mock<IRefreshTokenRepository>();

            refreshTokenRepository.Setup(repository => repository.CreateRefreshToken(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(refreshToken);
            _unitOfWork.Setup(uOfWork => uOfWork.RefreshTokenRepository).Returns(refreshTokenRepository.Object);

            string claimName = "ID";
            ClaimHelper.GetNamedClaim = (claimsPrincipal, claimName) => DataFixtures.GUID;
            ClaimHelper.GetUnixExpiryDate = (claimsPrincipal) => DateTimeOffset.Now.ToUnixTimeSeconds();
            ClaimHelper.GetJTI = (claimsPrincipal) => DataFixtures.GUID;

            var UserRepository = new Mock<IUserRepository>();
            UserRepository.Setup(repository => repository.GetWithDetailsAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(user);
            _unitOfWork.Setup(uOfWork => uOfWork.UserRepository).Returns(UserRepository.Object);

            var response = await _tokenService.RefreshTokenAsync(claimsPrincipal, refreshToken, organisationID);

            //assert
            Assert.NotNull(response);
            Assert.Same(user.Email, response.Email);
        }

        public void Dispose()
        {
            typeof(ClaimHelper).TypeInitializer.Invoke(null, null); //set the mocked delegate values back to normal
        }
    }
}