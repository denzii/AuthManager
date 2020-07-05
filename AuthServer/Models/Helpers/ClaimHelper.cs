using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace AuthServer.Models.Helpers
{
    public static class ClaimHelper
    {
        public static Func<ClaimsPrincipal, long> GetUnixExpiryDate = (validatedToken) => {
            return long.Parse(validatedToken.Claims.Single(claim => claim.Type == JwtRegisteredClaimNames.Exp).Value);
        };

        public static Func<ClaimsPrincipal, string> GetJTI = (validatedToken) => {
            return validatedToken.Claims.Single(claim => claim.Type == JwtRegisteredClaimNames.Jti).Value;
        };

        public static Func<ClaimsPrincipal, string, string> GetNamedClaim = (validatedtoken, claimType) => {
            return validatedtoken.Claims.Single(claim => claim.Type == claimType).Value;
        };
    }
}