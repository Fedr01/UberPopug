// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Collections.Generic;
using IdentityServer4.Models;

namespace UberPopug.AuthService
{
    public static class Config
    {
        public static string TrackerUrl = "https://localhost:5010";

        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResource("roles", new[] { "role" })
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope("tracker")
            };

        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                // interactive client using code flow + pkce
                new Client
                {
                    ClientId = "tracker",
                    ClientSecrets = { new Secret("tracker-pwd".Sha256()) },

                    AllowedGrantTypes = GrantTypes.Hybrid,

                    RedirectUris = { $"{TrackerUrl}/signin-oidc" },
                    FrontChannelLogoutUri = $"{TrackerUrl}/signout-oidc",
                    PostLogoutRedirectUris = { $"{TrackerUrl}/signout-callback-oidc" },

                    AllowOfflineAccess = true,
                    AllowedScopes = { "openid", "profile", "tracker", "roles" },
                    RequirePkce = false
                },
            };
    }
}