// ***********************************************************************
// Assembly         : TestWebAppCore
// Author           : joshl
// Created          : 12-07-2021
//
// Last Modified By : joshl
// Last Modified On : 12-07-2021
// ***********************************************************************
// <copyright file="AuthController.cs" company="TestWebAppCore">
//     Copyright (c) LavelyIO. All rights reserved.
// </copyright>
// <summary>Defines the AuthController type.</summary>
// ***********************************************************************

namespace TestIdPCore.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Security.Claims;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading.Tasks;

    using JAGC.Identity.Saml.Bindings;
    using JAGC.Identity.Saml.Configuration;
    using JAGC.Identity.Saml.MvcCore.Extensions;
    using JAGC.Identity.Saml.Request;
    using JAGC.Identity.Saml.Schemas;
    using JAGC.Identity.Saml.Schemas.Metadata;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Tokens.Saml2;

    using TestIdPCore.Models;

    /// <summary>
    /// The auth controller.
    /// </summary>
    [AllowAnonymous]
    [Route("Auth")]
    public class AuthController : Controller
    {
        /// <summary>
        /// The relay state return url.
        /// </summary>
        private const string relayStateReturnUrl = "ReturnUrl";

        /// <summary>
        /// The settings.
        /// </summary>
        private readonly Settings settings;

        /// <summary>
        /// The config.
        /// </summary>
        private readonly Saml2Configuration config;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthController"/> class.
        /// </summary>
        /// <param name="settingsAccessor">
        /// The settings accessor.
        /// </param>
        /// <param name="configAccessor">
        /// The config accessor.
        /// </param>
        public AuthController(IOptions<Settings> settingsAccessor, IOptions<Saml2Configuration> configAccessor)
        {
            settings = settingsAccessor.Value;
            config = configAccessor.Value;
        }

        [Route("Login")]
        public IActionResult Login()
        {
            var requestBinding = new Saml2RedirectBinding();
            var relyingParty = ValidateRelyingParty(ReadRelyingPartyFromLoginRequestAsync(requestBinding).Result);

            var saml2AuthnRequest = new Saml2AuthnRequest(config);
            try
            {
                requestBinding.Unbind(Request.ToGenericHttpRequest(), saml2AuthnRequest);

                // ****  Handle user login e.g. in GUI ****
                // Test user with session index and claims
                var sessionIndex = Guid.NewGuid().ToString();
                var claims = CreateTestUserClaims(saml2AuthnRequest.Subject?.NameID?.ID);

                return LoginResponse(
                    saml2AuthnRequest.Id,
                    Saml2StatusCodes.Success,
                    requestBinding.RelayState,
                    relyingParty,
                    sessionIndex,
                    claims);
            }
            catch (Exception exc)
            {
#if DEBUG
                Debug.WriteLine(
                    $"Saml 2.0 Authn Request error: {exc.ToString()}\nSaml Auth Request: '{saml2AuthnRequest.XmlDocument?.OuterXml}'\nQuery String: {Request.QueryString}");
#endif
                return LoginResponse(
                    saml2AuthnRequest.Id,
                    Saml2StatusCodes.Responder,
                    requestBinding.RelayState,
                    relyingParty);
            }
        }

        /// <summary>
        /// The logout.
        /// </summary>
        /// <returns>
        /// The <see cref="IActionResult"/>.
        /// </returns>
        [HttpPost("Logout")]
        public IActionResult Logout()
        {
            var requestBinding = new Saml2PostBinding();
            var relyingParty = ValidateRelyingParty(ReadRelyingPartyFromLogoutRequest(requestBinding));

            var saml2LogoutRequest = new Saml2LogoutRequest(config);
            saml2LogoutRequest.SignatureValidationCertificates =
                new X509Certificate2[] { relyingParty.SignatureValidationCertificate };
            try
            {
                requestBinding.Unbind(Request.ToGenericHttpRequest(), saml2LogoutRequest);

                // **** Delete user session ****

                return LogoutResponse(
                    saml2LogoutRequest.Id,
                    Saml2StatusCodes.Success,
                    requestBinding.RelayState,
                    saml2LogoutRequest.SessionIndex,
                    relyingParty);
            }
            catch (Exception exc)
            {
#if DEBUG
                Debug.WriteLine(
                    $"Saml 2.0 Logout Request error: {exc.ToString()}\nSaml Logout Request: '{saml2LogoutRequest.XmlDocument?.OuterXml}'");
#endif
                return LogoutResponse(
                    saml2LogoutRequest.Id,
                    Saml2StatusCodes.Responder,
                    requestBinding.RelayState,
                    saml2LogoutRequest.SessionIndex,
                    relyingParty);
            }
        }

        /// <summary>
        /// The read relying party from login request.
        /// </summary>
        /// <param name="binding">
        /// The binding.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private async Task<string> ReadRelyingPartyFromLoginRequestAsync<T>(Saml2Binding<T> binding)
        {
            return binding.ReadSamlRequest(Request.ToGenericHttpRequest(), new Saml2AuthnRequest(config))?.Issuer;
        }

        /// <summary>
        /// The read relying party from logout request.
        /// </summary>
        /// <param name="binding">
        /// The binding.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string ReadRelyingPartyFromLogoutRequest<T>(Saml2Binding<T> binding)
        {
            return binding.ReadSamlRequest(Request.ToGenericHttpRequest(), new Saml2LogoutRequest(config))?.Issuer;
        }

        /// <summary>
        /// The login response.
        /// </summary>
        /// <param name="inResponseTo">
        /// The in response to.
        /// </param>
        /// <param name="status">
        /// The status.
        /// </param>
        /// <param name="relayState">
        /// The relay state.
        /// </param>
        /// <param name="relyingParty">
        /// The relying party.
        /// </param>
        /// <param name="sessionIndex">
        /// The session index.
        /// </param>
        /// <param name="claims">
        /// The claims.
        /// </param>
        /// <returns>
        /// The <see cref="IActionResult"/>.
        /// </returns>
        private IActionResult LoginResponse(
            Saml2Id inResponseTo,
            Saml2StatusCodes status,
            string relayState,
            RelyingParty relyingParty,
            string sessionIndex = null,
            IEnumerable<Claim> claims = null)
        {
            var responsebinding = new Saml2PostBinding();
            responsebinding.RelayState = relayState;

            var saml2AuthnResponse = new Saml2AuthnResponse(config)
            {
                InResponseTo = inResponseTo,
                Status = status,
                Destination =
                                                 relyingParty.SingleSignOnDestination,
            };
            if (status == Saml2StatusCodes.Success && claims != null)
            {
                saml2AuthnResponse.SessionIndex = sessionIndex;

                var claimsIdentity = new ClaimsIdentity(claims);
                saml2AuthnResponse.NameId = new Saml2NameIdentifier(
                    claimsIdentity.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).Single(),
                    NameIdentifierFormats.Persistent);
                //saml2AuthnResponse.NameId = new Saml2NameIdentifier(claimsIdentity.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).Single());
                saml2AuthnResponse.ClaimsIdentity = claimsIdentity;

                var token = saml2AuthnResponse.CreateSecurityToken(
                    relyingParty.Issuer,
                    subjectConfirmationLifetime: 5,
                    issuedTokenLifetime: 60);
            }

            return responsebinding.Bind(saml2AuthnResponse).ToActionResult();
        }

        /// <summary>
        /// The logout response.
        /// </summary>
        /// <param name="inResponseTo">
        /// The in response to.
        /// </param>
        /// <param name="status">
        /// The status.
        /// </param>
        /// <param name="relayState">
        /// The relay state.
        /// </param>
        /// <param name="sessionIndex">
        /// The session index.
        /// </param>
        /// <param name="relyingParty">
        /// The relying party.
        /// </param>
        /// <returns>
        /// The <see cref="IActionResult"/>.
        /// </returns>
        private IActionResult LogoutResponse(
            Saml2Id inResponseTo,
            Saml2StatusCodes status,
            string relayState,
            string sessionIndex,
            RelyingParty relyingParty)
        {
            var responsebinding = new Saml2PostBinding();
            responsebinding.RelayState = relayState;

            var saml2LogoutResponse = new Saml2LogoutResponse(config)
            {
                InResponseTo = inResponseTo,
                Status = status,
                Destination =
                                                  relyingParty
                                                      .SingleLogoutResponseDestination,
                SessionIndex = sessionIndex
            };

            return responsebinding.Bind(saml2LogoutResponse).ToActionResult();
        }

        /// <summary>
        /// The validate relying party.
        /// </summary>
        /// <param name="issuer">
        /// The issuer.
        /// </param>
        /// <returns>
        /// The <see cref="RelyingParty"/>.
        /// </returns>
        /// <exception cref="Exception">
        /// </exception>
        private RelyingParty ValidateRelyingParty(string issuer)
        {
            foreach (var rp in settings.RelyingParties)
            {
                try
                {
                    if (string.IsNullOrEmpty(rp.Issuer))
                    {
                        var entityDescriptor = new EntityDescriptor();
                        entityDescriptor.ReadSPSsoDescriptorFromUrl(new Uri(rp.Metadata));
                        if (entityDescriptor.SPSsoDescriptor != null)
                        {
                            rp.Issuer = entityDescriptor.EntityId;
                            rp.SingleSignOnDestination = entityDescriptor.SPSsoDescriptor.AssertionConsumerServices
                                .Where(a => a.IsDefault).OrderBy(a => a.Index).First().Location;
                            var singleLogoutService = entityDescriptor.SPSsoDescriptor.SingleLogoutServices.First();
                            rp.SingleLogoutResponseDestination =
                                singleLogoutService.ResponseLocation ?? singleLogoutService.Location;
                            rp.SignatureValidationCertificate =
                                entityDescriptor.SPSsoDescriptor.SigningCertificates.First();
                        }
                        else
                        {
                            throw new Exception($"SPSsoDescriptor not loaded from metadata '{rp.Metadata}'.");
                        }
                    }
                }
                catch (Exception exc)
                {
                    //log error
#if DEBUG
                    Debug.WriteLine($"SPSsoDescriptor error: {exc.ToString()}");
#endif
                }
            }

            return settings.RelyingParties.Where(
                    rp => rp.Issuer != null && rp.Issuer.Equals(issuer, StringComparison.InvariantCultureIgnoreCase))
                .Single();
        }

        /// <summary>
        /// The create test user claims.
        /// </summary>
        /// <param name="selectedNameID">
        /// The selected name id.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        private IEnumerable<Claim> CreateTestUserClaims(string selectedNameID)
        {
            var userId = selectedNameID ?? "JAGC.User";
            yield return new Claim(ClaimTypes.NameIdentifier, userId);
            yield return new Claim(ClaimTypes.Upn, $"{userId}@us.af.mil");
            yield return new Claim(ClaimTypes.Email, $"{userId}@us.af.mil");
        }
    }
}