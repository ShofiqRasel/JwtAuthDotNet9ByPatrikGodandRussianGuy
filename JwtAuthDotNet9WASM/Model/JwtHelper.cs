using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace JwtAuthDotNet9WASM.Model
{
    public class JwtHelper
    {
        public static string GetNameIdentifier(string jwtToken)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwtToken);

            var nameIdentifier = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            return nameIdentifier;
        }

        public static string GetUserName(string jwtToken)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwtToken);

            var UserName = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            return UserName;
        }
    }
}
