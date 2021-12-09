using JAGC.Identity.Saml.Bindings;
using JAGC.Identity.Saml.Request;
using Microsoft.AspNetCore.Mvc;

namespace JAGC.Identity.Saml.MvcCore.Extensions
{
    /// <summary>
    /// Extension methods for Bindings
    /// </summary>
    public static class Saml2BindingExtensions
    {
        /// <summary>
        /// To Redirect Action Result
        /// </summary>
        public static IActionResult ToActionResult(this Saml2RedirectBinding binding)
        {
            return new RedirectResult(binding.RedirectLocation.OriginalString);
        }

        /// <summary>
        /// To Post Action Result
        /// </summary>
        public static IActionResult ToActionResult(this Saml2PostBinding binding)
        {
            return new ContentResult
            {
                ContentType = "text/html",
                Content = binding.PostContent
            };
        }

        /// <summary>
        /// To XML Action Result
        /// </summary>
        public static IActionResult ToActionResult(this Saml2Metadata metadata)
        {
            return new ContentResult
            {
                ContentType = "text/xml",
                Content = metadata.ToXml(),
            };
        }
    }
}
