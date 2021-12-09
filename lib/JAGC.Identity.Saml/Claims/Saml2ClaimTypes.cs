namespace JAGC.Identity.Saml.Claims
{
    public static class Saml2ClaimTypes
    {
        public const string NameId = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name";
        public const string NameIdFormat = "urn:oasis:names:tc:SAML:2.0:nameid-­format:persistent";
        public const string Email = "urn:oasis:names:tc:SAML:2.0:attrname-­format:basic";
        public const string GivenName = "https://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname";
        public const string Surname = "https://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname";
        public const string SessionIndex = "http://schemas.xmlsoap.org/ws/2014/02/identity/claims/saml2sessionindex";
    }
}