using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using JAGC.Identity.Saml;
using JAGC.Identity.Saml.Configuration;
using JAGC.Identity.Saml.MvcCore.Extensions;
using JAGC.Identity.Saml.Request;
using JAGC.Identity.Saml.Schemas;
using JAGC.Identity.Saml.Schemas.Metadata;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace TestBlazorSP.Controllers
{
    [AllowAnonymous]
    [Route("Metadata")]
    public class MetadataController : Controller
    {
        private readonly Saml2Configuration config;

        public MetadataController(IOptions<Saml2Configuration> configAccessor)
        {
            config = configAccessor.Value;
        }

        public IActionResult Index()
        {
            var defaultSite = new Uri($"{Request.Scheme}://{Request.Host.ToUriComponent()}/");

            var entityDescriptor = new EntityDescriptor(config);
            entityDescriptor.ValidUntil = 365;
            entityDescriptor.SPSsoDescriptor = new SPSsoDescriptor
            {
                WantAssertionsSigned = true,
                SigningCertificates = new X509Certificate2[]
                {
                    config.SigningCertificate
                },
                // EncryptionCertificates = new X509Certificate2[]
                //{
                //   config.DecryptionCertificate
                //},
                SingleLogoutServices = new SingleLogoutService[]
                {
                    new SingleLogoutService { Binding = ProtocolBindings.HttpPost, Location = new Uri(defaultSite, "Auth/SingleLogout"), ResponseLocation = new Uri(defaultSite, "Auth/LoggedOut") }
                },
                //NameIDFormats = new Uri[] { NameIdentifierFormats.X509SubjectName },
                NameIDFormats = new Uri[] { NameIdentifierFormats.Persistent },
                AssertionConsumerServices = new AssertionConsumerService[]
                {
                    new AssertionConsumerService { Binding = ProtocolBindings.HttpPost, Location = new Uri(defaultSite, "Auth/AssertionConsumerService") },
                    new AssertionConsumerService { Binding = ProtocolBindings.HttpPost, Location = new Uri(defaultSite, "Auth/AssertionConsumerService-test"), IsDefault = false }
                },
                AttributeConsumingServices = new AttributeConsumingService[]
                {
                    new AttributeConsumingService { ServiceName = new ServiceName("JAGC SAML Service Provider", "en"), RequestedAttributes = CreateRequestedAttributes() }
                },
            };
            /*
            entityDescriptor.ContactPersons = new[] {
                new ContactPerson(ContactTypes.Administrative)
                {
                    Company = "USAF, JAGC",
                    GivenName = "Josh",
                    SurName = "Lavely",
                    EmailAddress = "joshua.lavely.ctr@us.af.mil",
                    TelephoneNumber = "11111111",
                },
                new ContactPerson(ContactTypes.Technical)
                {
                    Company = "USAF, JAGC",
                    GivenName = "Ryan",
                    SurName = "Lammin",
                    EmailAddress = "ryan.lammin.ctr@us.af.mil",
                    TelephoneNumber = "22222222",
                }
            };
            */
            return new Saml2Metadata(entityDescriptor).CreateMetadata().ToActionResult();
        }

        private IEnumerable<RequestedAttribute> CreateRequestedAttributes()
        {
            yield return new RequestedAttribute("urn:oid:2.5.4.4");
            yield return new RequestedAttribute("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name");
            yield return new RequestedAttribute("piv");
            yield return new RequestedAttribute("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress");
            yield return new RequestedAttribute("urn:oid:2.5.4.3", false);
        }
    }
}