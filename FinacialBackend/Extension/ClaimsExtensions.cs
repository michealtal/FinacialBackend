using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens; // for JwtRegisteredClaimNames

namespace FinacialBackend.Extension
{
    public static class ClaimsExtensions
    {
        public static string GetUsername(this ClaimsPrincipal user)
        {
            return user?.Claims.FirstOrDefault(x =>
                x.Type == ClaimTypes.Name ||
                x.Type == ClaimTypes.GivenName ||
                x.Type == JwtRegisteredClaimNames.GivenName
            )?.Value;
        }
    }
}
