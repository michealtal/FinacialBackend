using System.Security.Claims;

namespace FinacialBackend.Extension
{
    public static class ClaimsExtensions
    {
        public static string GetUsername(this ClaimsPrincipal user)
        {
            var claim = user.Claims.SingleOrDefault(x =>
                x.Type.Equals("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname"));

            if (claim == null)
                throw new Exception("Username claim is missing");

            return claim.Value;
        }

        public static string GetUserId(this ClaimsPrincipal user)
        {
            var claim = user.Claims.SingleOrDefault(x =>
                x.Type.Equals("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"));

            if (claim == null)
                throw new Exception("User ID claim is missing");

            return claim.Value;
        }
    }
}
