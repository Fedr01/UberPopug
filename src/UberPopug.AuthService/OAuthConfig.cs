// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Collections.Generic;
using System.Security.Claims;
using IdentityServer4.Models;
using IdentityServer4.Test;

namespace UberPopug.AuthService
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
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
                
                    RedirectUris = { "https://localhost:5010/signin-oidc" },
                    FrontChannelLogoutUri = "https://localhost:5010/signout-oidc",
                    PostLogoutRedirectUris = { "https://localhost:5010/signout-callback-oidc" },
                
                    AllowOfflineAccess = true,
                    AllowedScopes = { "openid", "profile", "tracker" },
                    RequirePkce = false
                },
            };       
     
    }
}