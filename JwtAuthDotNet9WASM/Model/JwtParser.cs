using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace JwtAuthDotNet9WASM.Model
{
    public class JwtParser
    {
        public static string? GetClaimFromJwt(string jwtToken, string claimType)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwtToken);

            var claim = token.Claims.FirstOrDefault(c => c.Type == claimType);
            return claim?.Value;
        }
    }
}
