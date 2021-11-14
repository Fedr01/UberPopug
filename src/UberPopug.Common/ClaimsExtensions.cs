using System.Linq;
using System.Security.Claims;

namespace UberPopug.Common
{
    public static class ClaimsExtensions
    {
        public static string GetEmail(this ClaimsPrincipal principal)
        {
            return principal.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
        } 
    }
}