using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AuthServer.Contracts.Version1.ResponseContracts;
using AuthServer.Models.Entities;
using static AuthServer.Contracts.Version1.ResponseContracts.Authentication;

namespace AuthServer.Models.Services.Interfaces
{
    public interface ITokenService
    {
        Task<Dictionary<string, string>> GetTokensAsync(User user);

        Task<RefreshToken> CanTokenBeRefreshedAsync(ClaimsPrincipal validatedToken, string refreshToken);

        ClaimsPrincipal IsTokenAuthentic(string token);

        Task<RefreshTokenResponse> RefreshTokenAsync(ClaimsPrincipal validatedToken, RefreshToken refreshToken, string organisationID);
    }
}