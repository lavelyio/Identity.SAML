using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using JAGC.Identity.Saml.Cryptography;

namespace JAGC.Identity.Saml.Extensions
{
    /// <summary>
    /// Extension methods for Saml2X509Certificate.
    /// </summary>
    public static class X509Certificate2Extensions
    {
        /// <summary>
        /// Get the private RSA key from either Saml2X509Certificate or X509Certificate2.
        /// </summary>
        public static RSA GetSamlRSAPrivateKey(this X509Certificate2 certificate)
        {
            if(certificate is Saml2X509Certificate)
            {
                return (certificate as Saml2X509Certificate).GetRSAPrivateKey();
            }
            else
            {
                return certificate.GetRSAPrivateKey();
            }
        }
    }
}
