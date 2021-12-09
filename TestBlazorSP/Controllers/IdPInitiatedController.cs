using JAGC.Identity.Saml;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens.Saml2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using JAGC.Identity.Saml.Bindings;
using JAGC.Identity.Saml.Configuration;
using JAGC.Identity.Saml.MvcCore.Extensions;
using JAGC.Identity.Saml.Request;
using JAGC.Identity.Saml.Schemas;
using JAGC.Identity.Saml.Util;

namespace TestBlazorSP.Controllers
{
    [AllowAnonymous]
    [Route("IdPInitiated")]
    public class IdPInitiatedController : Controller
    {
        [HttpGet("[action]")]
        public IActionResult InitiateADFS()
        {
            var serviceProviderRealm = "https://localhost:44306";

            var binding = new Saml2PostBinding();
            binding.RelayState = $"RPID={Uri.EscapeDataString(serviceProviderRealm)}";

            var config = new Saml2Configuration();

            config.Issuer = "testidpcore";
            config.SingleSignOnDestination = new Uri("https://localhost:44305/Auth/login");
            config.SigningCertificate = CertificateUtil.Load(Environment. Startup.AppEnvironment.MapToPhysicalFilePath("jagc-az-sp.pfx"), "C4ir0sC4ir0s");
            config.SignatureAlgorithm = Saml2SecurityAlgorithms.RsaSha256Signature;
            config.SignAuthnRequest = true;

            // Audience
            var appliesToAddress = "https://localhost:44306";

            var response = new Saml2AuthnResponse(config);
            response.Status = Saml2StatusCodes.Success;

            var claimsIdentity = new ClaimsIdentity(CreateClaims());
            response.NameId = new Saml2NameIdentifier(
                claimsIdentity.Claims
                    .Where(c => c.Type == ClaimTypes.NameIdentifier)
                    .Select(c => c.Value)
                    .Single(),
                NameIdentifierFormats.Email);

            response.ClaimsIdentity = claimsIdentity;
            var token = response.CreateSecurityToken(appliesToAddress);

            return binding.Bind(response).ToActionResult();
        }

        [HttpGet("[action]")]
        public IActionResult InitiateAzureIDP()
        {
            var serviceProviderRealm = "https://localhost:44306";

            var binding = new Saml2PostBinding();
            binding.RelayState = $"RPID={Uri.EscapeDataString(serviceProviderRealm)}";

            var config = new Saml2Configuration();

            config.Issuer = "6cf2a243-3b50-4767-9298-3a4c0518b31f";
            config.SingleSignOnDestination = new Uri("https://login.microsoftonline.us/fe738758-04ef-41bc-ac38-bcee2c0da636/saml2");
            config.SigningCertificate = CertificateUtil.Load(Startup.AppEnvironment.MapToPhysicalFilePath("jagc-az-sp.pfx"), "C4ir0sC4ir0s");
            config.SignatureAlgorithm = Saml2SecurityAlgorithms.RsaSha256Signature;

            // Audience
            var appliesToAddress = "https://localhost:44306";

            var response = new Saml2AuthnResponse(config);
            response.Status = Saml2StatusCodes.Success;

            var claimsIdentity = new ClaimsIdentity(CreateClaims());
            response.NameId = new Saml2NameIdentifier(
                claimsIdentity.Claims
                    .Where(c => c.Type == ClaimTypes.NameIdentifier)
                    .Select(c => c.Value)
                    .Single(),
                NameIdentifierFormats.Email);

            response.ClaimsIdentity = claimsIdentity;
            var token = response.CreateSecurityToken(appliesToAddress);

            return binding.Bind(response).ToActionResult();
        }

        [HttpGet("[action]")]
        public IActionResult Initiate()
        {
            var serviceProviderRealm = "https://localhost:44306";

            var binding = new Saml2PostBinding();
            binding.RelayState = $"RPID={Uri.EscapeDataString(serviceProviderRealm)}";

            var config = new Saml2Configuration();

            config.Issuer = "testidpcore";
            config.SingleSignOnDestination = new Uri("https://localhost:44305/Auth/login");
            config.SigningCertificate = CertificateUtil.Load(Startup.AppEnvironment.MapToPhysicalFilePath("jagc-az-sp.pfx"), "C4ir0sC4ir0s");
            config.SignatureAlgorithm = Saml2SecurityAlgorithms.RsaSha256Signature;

            // Audience
            var appliesToAddress = "https://localhost:44306";

            var response = new Saml2AuthnResponse(config);
            response.Status = Saml2StatusCodes.Success;

            var claimsIdentity = new ClaimsIdentity(CreateClaims());
            response.NameId = new Saml2NameIdentifier(
                claimsIdentity.Claims
                    .Where(c => c.Type == ClaimTypes.NameIdentifier)
                    .Select(c => c.Value)
                    .Single(),
                NameIdentifierFormats.Email);

            response.ClaimsIdentity = claimsIdentity;
            var token = response.CreateSecurityToken(appliesToAddress);

            return binding.Bind(response).ToActionResult();
        }

        private IEnumerable<Claim> CreateClaims()
        {
            yield return new Claim(ClaimTypes.NameIdentifier, "Joshua.Lavely");
            yield return new Claim(ClaimTypes.Email, "Joshua.Lavely@usafjas.onmicrosoft.us");
        }
    }
}
