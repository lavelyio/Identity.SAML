using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace JAGC.Identity.Saml.MvcCore.Extensions
{
    /// <summary>
    /// Extension methods for HttpRequest
    /// </summary>
    public static class HttpRequestExtensions
    {
        /// <summary>
        /// Converts a Microsoft.AspNet.Http.HttpRequest to JAGC.Identity.Saml.Http.HttpRequest.
        /// </summary>
        public static JAGC.Identity.Saml.Http.HttpRequest ToGenericHttpRequest(this HttpRequest request)
        {
            return new JAGC.Identity.Saml.Http.HttpRequest
            {
                Method = request.Method,
                QueryString = request.QueryString.Value,
                Query = ToNameValueCollection(request.Query),
                Form = "POST".Equals(request.Method, StringComparison.InvariantCultureIgnoreCase) ? ToNameValueCollection(request.Form) : null,
            };
        }

        private static NameValueCollection ToNameValueCollection(IEnumerable<KeyValuePair<string, StringValues>> items)
        {
            var nv = new NameValueCollection();
            foreach (var item in items)
            {
                nv.Add(item.Key, item.Value.First());
            }
            return nv;
        }
    }
}