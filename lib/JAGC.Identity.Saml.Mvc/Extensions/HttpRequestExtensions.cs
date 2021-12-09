using System;
using System.Web;

namespace JAGC.Identity.Saml.Mvc.Extensions
{
    /// <summary>
    /// Extension methods for HttpRequest
    /// </summary>
    public static class HttpRequestExtensions
    {
        /// <summary>
        /// Converts a System.Web.HttpRequestBase to JAGC.Identity.Saml.Http.HttpRequest.
        /// </summary>
        public static Http.HttpRequest ToGenericHttpRequest(this HttpRequestBase request)
        {
            return new Http.HttpRequest
            {
                Method = request.HttpMethod,
                QueryString = request.Url.Query,
                Query = request.QueryString,
                Form = "POST".Equals(request.HttpMethod, StringComparison.InvariantCultureIgnoreCase) ? request.Form : null,
            };
        }
    }
}
