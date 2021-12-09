using System.Xml;
using JAGC.Identity.Saml.Util;

namespace JAGC.Identity.Saml.Extensions
{
    /// <summary>
    /// Extension methods for XmlAttribute
    /// </summary>
    internal static class XmlAttributeExtensions
    {
        public static T GetValueOrNull<T>(this XmlAttribute xmlAttribute)
        {
            return GenericTypeConverter.ConvertValue<T>(xmlAttribute?.Value, xmlAttribute);
        }
    }
}
