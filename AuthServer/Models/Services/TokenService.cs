using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AuthServer.Models.DTOs;
using AuthServer.Models.Entities;
using AuthServer.Models.Helpers;
using AuthServer.Models.Services.Interfaces;
using AuthServer.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using static AuthServer.Contracts.Version1.ResponseContracts.Authentication;

namespace AuthServer.Models.Services
{
    public class TokenService : ITokenService
    {
        private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;
        private readonly IUnitOfWork _unitOfWork;
        private readonly JWTBearerAuthConfig _jwtConfig;
        private readonly UserManager<User> _userManager;
        private readonly SigningCredentials _signingCredentials;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly SecurityTokenDescriptor _securityTokenDescriptor;
        
        public TokenService(JwtSecurityTokenHandler jwtSecurityTokenHandler,
        IUnitOfWork unitOfWork,
        JWTBearerAuthConfig jwtConfig,
        UserManager<User> userManager,
        SigningCredentials signingCredentials,
        TokenValidationParameters tokenValidationParameters,
        SecurityTokenDescriptor securityTokenDescriptor
        )
        {
            _jwtSecurityTokenHandler = jwtSecurityTokenHandler;
            _unitOfWork = unitOfWork;
            _jwtConfig = jwtConfig;
            _userManager = userManager;
            _signingCredentials = signingCredentials;
            _tokenValidationParameters = tokenValidationParameters;
            _securityTokenDescriptor = securityTokenDescriptor;
        }

        public async Task<Dictionary<string, string>> GetTokensAsync(User user)
        {
            var tokens = new Dictionary<string, string>();

            var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("ID", user.Id),
                    new Claim("OrganisationID", user.Organisation.ID)
                };

            var userClaims = await _userManager.GetClaimsAsync(user);
            claims.AddRange(userClaims);

            _securityTokenDescriptor.Subject = new ClaimsIdentity(claims);

            SecurityToken token = _jwtSecurityTokenHandler.CreateToken(_securityTokenDescriptor);
            RefreshToken refreshToken =  _unitOfWork.RefreshTokenRepository.CreateRefreshToken(token.Id, user.Id);

            await _unitOfWork.RefreshTokenRepository.AddAsync(refreshToken);
            _unitOfWork.Complete();

            tokens.Add("SecurityToken", _jwtSecurityTokenHandler.WriteToken(token));
            tokens.Add("RefreshToken", refreshToken.Token);

            return tokens;
        }

        private ClaimsPrincipal GetClaimsPrincipalFromToken(string token)
        {
            try
            {
                TokenValidationParameters tokenValidationParameters = _tokenValidationParameters.Clone();
                tokenValidationParameters.ValidateLifetime = false;
                ClaimsPrincipal principal = _jwtSecurityTokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);

                if (!JwtHasValidAlgorithm(validatedToken))
                {
                    return null;
                }

                return principal;
            }
            catch(Exception e) { 
                Log.Error(e.ToString());

                return null; 
            }
        }

        private bool JwtHasValidAlgorithm(SecurityToken validatedToken)
        {
            return (validatedToken is JwtSecurityToken jwtSecurityToken)
                && jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);
        }

        public async Task<RefreshToken> CanTokenBeRefreshedAsync(ClaimsPrincipal validatedToken, string refreshToken)
        {
            long expiryDateUnix = ClaimHelper.GetUnixExpiryDate(validatedToken);

            //if not expired, dont let user refresh
            //tokens contain time in unix format.
            //unix epoch (1970 1 1 000) is used to calculate unix time
            //unix time (nr of seconds elapsed since epoch)
            DateTime expiryDateTimeUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                .AddSeconds(expiryDateUnix);

            RefreshToken storedRefreshToken = await _unitOfWork.RefreshTokenRepository.GetRefreshToken(refreshToken);

            if (expiryDateTimeUtc > DateTime.UtcNow
                || storedRefreshToken == null
                || DateTime.UtcNow > storedRefreshToken.ExpiryDate
                || storedRefreshToken.Invalidated
                || storedRefreshToken.Used
                || storedRefreshToken.JwtID != ClaimHelper.GetJTI(validatedToken))
            {
                return null;
            }

            return storedRefreshToken;
        }

        public ClaimsPrincipal IsTokenAuthentic(string token)
        {
            return GetClaimsPrincipalFromToken(token);
        }

        public async Task<RefreshTokenResponse> RefreshTokenAsync(ClaimsPrincipal validatedToken, RefreshToken storedRefreshToken, string organisationID)
        {
            storedRefreshToken.Used = true;
            _unitOfWork.RefreshTokenRepository.Update(storedRefreshToken);
            await _unitOfWork.CompleteAsync();

            string userID = ClaimHelper.GetNamedClaim(validatedToken, "ID");
            
            User user = await _unitOfWork.UserRepository.GetWithDetailsAsync(userID, organisationID);

            Dictionary<string, string> tokens = await GetTokensAsync(user);
            tokens.TryGetValue("SecurityToken", out string securityToken);
            tokens.TryGetValue("RefreshToken", out string newRefreshToken);

            return new RefreshTokenResponse
            {
                Email = user.Email,
                Token = securityToken,
                RefreshToken = newRefreshToken,
            };
        }

    }
}