using System.Xml.Linq;

namespace JAGC.Identity.Saml.Schemas.Conditions
{
    public interface ICondition
    {
        XElement ToXElement();
    }
}