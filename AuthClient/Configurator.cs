using System;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace AuthClient
{
    public static class Configurator
    {
        public static JWTBearerAuthConfig GetJwtBearerAuthConfig(string jwtSecret){
            return new JWTBearerAuthConfig{
                Secret = jwtSecret,
                TokenLifetime = new TimeSpan(00,05,00)
            };
        }

        public static TokenValidationParameters GetTokenValidationParameters(JWTBearerAuthConfig jwtBearerAuthConfig){
            return new TokenValidationParameters{
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtBearerAuthConfig.Secret)),
                ValidateIssuer = false,
                ValidateAudience = false,
                RequireExpirationTime = false,
                ValidateLifetime = true
            };
        }
    }
}