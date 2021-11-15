using System.Collections.Generic;
using IdentityServer4.Models;

namespace UberPopug.AuthService
{
    public static class Config
    {
        public static string TrackerUrl = "https://localhost:5010";
        public static string AccountingUrl = "https://localhost:5020";

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
                new ApiScope("tracker"),
                new ApiScope("accounting")
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
                new Client
                {
                    ClientId = "accounting",
                    ClientSecrets = { new Secret("accounting-pwd".Sha256()) },

                    AllowedGrantTypes = GrantTypes.Hybrid,

                    RedirectUris = { $"{AccountingUrl}/signin-oidc" },
                    FrontChannelLogoutUri = $"{AccountingUrl}/signout-oidc",
                    PostLogoutRedirectUris = { $"{AccountingUrl}/signout-callback-oidc" },

                    AllowOfflineAccess = true,
                    AllowedScopes = { "openid", "profile", "accounting", "roles" },
                    RequirePkce = false
                },
            };
    }
}