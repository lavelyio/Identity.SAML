﻿using System.Text;
using System.Web.Mvc;
using JAGC.Identity.Saml.Bindings;
using JAGC.Identity.Saml.Request;

namespace JAGC.Identity.Saml.Mvc.Extensions
{
    /// <summary>
    /// Extension methods for Bindings
    /// </summary>
    public static class Saml2BindingExtensions
    {
        /// <summary>
        /// To Redirect Action Result
        /// </summary>
        public static ActionResult ToActionResult(this Saml2RedirectBinding binding)
        {
            return new RedirectResult(binding.RedirectLocation.OriginalString);
        }

        /// <summary>
        /// To Post Action Result
        /// </summary>
        public static ActionResult ToActionResult(this Saml2PostBinding binding)
        {
            return new ContentResult
            {
                Content = binding.PostContent
            };
        }

        /// <summary>
        /// To XML Action Result
        /// </summary>
        public static ActionResult ToActionResult(this Saml2Metadata metadata)
        {
            return new ContentResult
            {
                ContentType = "text/xml",
                ContentEncoding = Encoding.UTF8,
                Content = metadata.ToXml(),
            };
        }
    }
}
