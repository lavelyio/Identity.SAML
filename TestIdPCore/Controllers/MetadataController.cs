// ***********************************************************************
// Assembly         : TestWebAppCore
// Author           : joshl
// Created          : 12-07-2021
//
// Last Modified By : joshl
// Last Modified On : 04-03-2022
// ***********************************************************************
// <copyright file="MetadataController.cs" company="TestWebAppCore">
//     Copyright (c) LavelyIO. All rights reserved.
// </copyright>
// <summary>The metadata controller.</summary>
// ***********************************************************************

namespace TestIdPCore.Controllers
{
    using System;
    using System.Security.Cryptography.X509Certificates;

    using JAGC.Identity.Saml.Configuration;
    using JAGC.Identity.Saml.MvcCore.Extensions;
    using JAGC.Identity.Saml.Request;
    using JAGC.Identity.Saml.Schemas;
    using JAGC.Identity.Saml.Schemas.Metadata;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// The metadata controller.
    /// </summary>
    [AllowAnonymous]
    [Route("Metadata")]
    public class MetadataController : Controller
    {
        /// <summary>
        /// The config.
        /// </summary>
        private readonly Saml2Configuration config;

        /// <summary>
        /// Initializes a new instance of the <see cref="MetadataController"/> class.
        /// </summary>
        /// <param name="configAccessor">
        /// The config accessor.
        /// </param>
        public MetadataController(IOptions<Saml2Configuration> configAccessor)
        {
            config = configAccessor.Value;
        }

        /// <summary>
        /// The index.
        /// </summary>
        /// <returns>
        /// The <see cref="IActionResult"/>.
        /// </returns>
        public IActionResult Index()
        {
            var entityDescriptor = new EntityDescriptor(config);
            entityDescriptor.ValidUntil = 365;
            entityDescriptor.IdPSsoDescriptor = new IdPSsoDescriptor
            {
                SigningCertificates =
                                                            new X509Certificate2[]
                                                                {
                                                                    config.SigningCertificate
                                                                },
                //EncryptionCertificates = new X509Certificate2[]
                //{
                //    config.DecryptionCertificate
                // },
                SingleSignOnServices =
                                                            new SingleSignOnService[]
                                                                {
                                                                    new SingleSignOnService
                                                                        {
                                                                            Binding =
                                                                                ProtocolBindings
                                                                                    .HttpRedirect,
                                                                            Location = config
                                                                                .SingleSignOnDestination
                                                                        }
                                                                },
                SingleLogoutServices =
                                                            new SingleLogoutService[]
                                                                {
                                                                    new SingleLogoutService
                                                                        {
                                                                            Binding =
                                                                                ProtocolBindings
                                                                                    .HttpPost,
                                                                            Location = config
                                                                                .SingleLogoutDestination
                                                                        }
                                                                },
                NameIDFormats =
                                                            new Uri[]
                                                                {
                                                                    NameIdentifierFormats
                                                                        .X509SubjectName
                                                                },
            };
            entityDescriptor.ContactPersons = new[]
                                                  {
                                                      new ContactPerson(ContactTypes.Administrative)
                                                          {
                                                              Company = "USAF",
                                                              GivenName = "Josh",
                                                              SurName = "Lavely",
                                                              EmailAddress = "Joshua.Lavely.ctr@us.af.mil",
                                                              TelephoneNumber = "7038627672",
                                                          },
                                                      new ContactPerson(ContactTypes.Technical)
                                                          {
                                                              Company = "USAF",
                                                              GivenName = "Terry",
                                                              SurName = "Wyatt",
                                                              EmailAddress = "Terry.Wyatt@us.af.mil",
                                                              TelephoneNumber = "22222222",
                                                          }
                                                  };
            return new Saml2Metadata(entityDescriptor).CreateMetadata().ToActionResult();
        }
    }
}