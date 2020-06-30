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
        private readonly JWTBearerAuthConfig _jwtConfig;
        private readonly IMapper _mapper;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly UserManager<User> _userManager;

        public AuthenticationService(
            IUnitOfWork unitOfWork,
            JWTBearerAuthConfig jwtConfig,
            IMapper mapper,
            TokenValidationParameters tokenValidationParameters,
            UserManager<User> userManager
            )
        {
            _unitOfWork = unitOfWork;
            _jwtConfig = jwtConfig;
            _mapper = mapper;
            _tokenValidationParameters = tokenValidationParameters;
            _userManager = userManager;
        }

        public async Task<List<ErrorResponse>> ValidateRegistrationAsync(RegistrationRequest request, Organisation organisation, User newUser)
        {
            User user = await _userManager.FindByEmailAsync(request.Email);
            var errorResponses = new List<ErrorResponse>();

            if (user != null)
            {
                errorResponses.Add(new ErrorResponse{Message = "Email is already registered"});
            }

            if (organisation == null)
            {
                errorResponses.Add(new ErrorResponse{Message = "No Organisation found with the given Organisation Name"});
            }

            if(errorResponses.Any()){
                return errorResponses;
            }

            IdentityResult result = await _userManager.CreateAsync(newUser, request.Password);

            return result.Errors.Select(error => new ErrorResponse { Message = error.Description }).ToList();
        }

        public async Task<RegistrationResponse> RegisterUserAsync(RegistrationRequest request, Organisation organisation, User newUser)
        {
            using (var transaction = _unitOfWork.UserRepository.BeginTransaction())
            {
                if (!organisation.Users.Any())
                {
                    Policy adminPolicy = organisation.Policies.FirstOrDefault();
                    newUser.Policy = adminPolicy;

                    await _userManager.AddClaimAsync(newUser, new Claim(adminPolicy.Claim, "true"));
                }

                await _unitOfWork.CompleteAsync();
                Dictionary<string, string> tokens = await GetTokens(newUser);

                RegistrationResponse response = _mapper.Map<RegistrationResponse>(newUser);

                tokens.TryGetValue("SecurityToken", out string securityToken);
                tokens.TryGetValue("RefreshToken", out string refreshToken);

                response.Token = securityToken;
                response.RefreshToken = refreshToken;

                transaction.Commit();

                return response;
            }
        }

        public async Task<LoginResponse> LoginUserAsync(LoginRequest request)
        {
            User existingUser = await _unitOfWork.UserRepository.GetByEmailAsync(request.Email);

            Dictionary<string, string> tokens = tokens = await GetTokens(existingUser);
            tokens.TryGetValue("SecurityToken", out string securityToken);
            tokens.TryGetValue("RefreshToken", out string refreshToken);

            LoginResponse response = _mapper.Map<LoginResponse>(existingUser);

            response.Token = securityToken;
            response.RefreshToken = refreshToken;

            return response;
        }

        public ClaimsPrincipal IsTokenAuthentic(string token)
        {
            return GetClaimsPrincipalFromToken(token);
        }

        public async Task<RefreshToken> CanTokenBeRefreshed(ClaimsPrincipal validatedToken, string refreshToken)
        {
            var expiryDateUnix = long.Parse(validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

            //if not expired, dont let user refresh
            //tokens contain time in unix format.
            //unix epoch (1970 1 1 000) is used to calculate unix time
            //unix time (nr of seconds elapsed since epoch)
            DateTime expiryDateTimeUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                .AddSeconds(expiryDateUnix);

            string jti = validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Jti).Value;


            RefreshToken storedRefreshToken = await _unitOfWork.RefreshTokenRepository.GetRefreshToken(refreshToken);

            if (expiryDateTimeUtc > DateTime.UtcNow
                || storedRefreshToken == null
                || DateTime.UtcNow > storedRefreshToken.ExpiryDate
                || storedRefreshToken.Invalidated
                || storedRefreshToken.Used
                || storedRefreshToken.JwtID != jti)
            {
                return null;
            }

            return storedRefreshToken;
        }

        public async Task<RefreshTokenResponse> RefreshTokenAsync(ClaimsPrincipal validatedToken, RefreshToken storedRefreshToken, string organisationID)
        {

            var transaction = _unitOfWork.RefreshTokenRepository.BeginTransaction();

            storedRefreshToken.Used = true;
            _unitOfWork.RefreshTokenRepository.Update(storedRefreshToken);
            await _unitOfWork.CompleteAsync();

            string userID = validatedToken.Claims.Single(claim => claim.Type == "ID").Value;
            User user = await _unitOfWork.UserRepository.GetWithDetailsAsync(userID, organisationID);

            Dictionary<string, string> tokens = await GetTokens(user);
            tokens.TryGetValue("SecurityToken", out string securityToken);
            tokens.TryGetValue("RefreshToken", out string newRefreshToken);

            transaction.Commit();

            return new RefreshTokenResponse
            {
                Email = user.Email,
                Token = securityToken,
                RefreshToken = newRefreshToken,
            };
        }

        public ClaimsPrincipal GetClaimsPrincipalFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                TokenValidationParameters tokenValidationParameters = _tokenValidationParameters.Clone();
                tokenValidationParameters.ValidateLifetime = false;
                ClaimsPrincipal principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);

                if (!JwtHasValidAlgorithm(validatedToken))
                {
                    return null;
                }

                return principal;
            }
            catch { return null; }
        }

        private bool JwtHasValidAlgorithm(SecurityToken validatedToken)
        {
            // is operator (> C# 7.0), can be used to check if given expression is of type on the right hand side.
            // StringComparison.InvariantCultureIgnoreCase allows comparing string similarity by disregarding language specific characters
            return (validatedToken is JwtSecurityToken jwtSecurityToken)
                && jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);
        }

        private async Task<Dictionary<string, string>> GetTokens(User user)
        {
            var tokens = new Dictionary<string, string>();
            var tokenHandler = new JwtSecurityTokenHandler();
            byte[] key = Encoding.ASCII.GetBytes(_jwtConfig.Secret); //key = secret in ascii bytes

            var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("ID", user.Id.ToString()),
                    new Claim("OrganisationID", user.Organisation.ID.ToString())
                };

            var userClaims = await _userManager.GetClaimsAsync(user);
            claims.AddRange(userClaims);

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                // SecurityTokenDescriptor takes in an array of Claims wrapped in Claims Identity in its Subject field
                // This is how we specify exactly what the token must include, each rule is defined as a new Claim
                // Each Claim is matched against a registered Claim Name or custom "string".
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.Add(_jwtConfig.TokenLifetime),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                    )
            };

            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            RefreshToken refreshToken = await Task.Run(() => _unitOfWork.RefreshTokenRepository.CreateRefreshToken(token.Id, user.Id));

            await _unitOfWork.RefreshTokenRepository.AddAsync(refreshToken);
            _unitOfWork.Complete();

            tokens.Add("SecurityToken", tokenHandler.WriteToken(token));
            tokens.Add("RefreshToken", refreshToken.Token);

            return tokens;
        }
    }
}
