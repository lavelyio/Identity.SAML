using System.Threading.Tasks;
using JAGC.Identity.Saml.Request;
using JAGC.Identity.Saml.Schemas;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace JAGC.Identity.Saml.MvcCore.Extensions
{
    public static class Saml2RequestExtensions
    {
        /// <summary>
        /// Delete the current Session.
        /// </summary>
        public static async Task<Saml2LogoutRequest> DeleteSession(this Saml2LogoutRequest saml2LogoutRequest, HttpContext httpContext)
        {
            await httpContext.SignOutAsync(Saml2Constants.AuthenticationScheme);
            return saml2LogoutRequest;
        }
    }
}
