using System.IdentityModel.Services;
using JAGC.Identity.Saml.Request;

namespace JAGC.Identity.Saml.Mvc.Extensions
{
    public static class Saml2RequestExtensions
    {
        /// <summary>
        /// Delete the current Federated Authentication Session.
        /// </summary>
        public static Saml2LogoutRequest DeleteSession(this Saml2LogoutRequest saml2LogoutRequest)
        {
            FederatedAuthentication.SessionAuthenticationModule.DeleteSessionTokenCookie();
            FederatedAuthentication.SessionAuthenticationModule.SignOut();
            return saml2LogoutRequest;
        }
    }
}
