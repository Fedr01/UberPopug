using System.Collections.Generic;
using IdentityServer4.Models;
using UberPopug.TaskTrackerService;

namespace UberPopug.AuthService
{
    public static class AuthConfig
    {
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

                    RedirectUris = { $"{EnvironmentConfig.TrackerUrl}/signin-oidc" },
                    FrontChannelLogoutUri = $"{EnvironmentConfig.TrackerUrl}/signout-oidc",
                    PostLogoutRedirectUris = { $"{EnvironmentConfig.TrackerUrl}/signout-callback-oidc" },

                    AllowOfflineAccess = true,
                    AllowedScopes = { "openid", "profile", "tracker", "roles" },
                    RequirePkce = false
                },
                new Client
                {
                    ClientId = "accounting",
                    ClientSecrets = { new Secret("accounting-pwd".Sha256()) },

                    AllowedGrantTypes = GrantTypes.Hybrid,

                    RedirectUris = { $"{EnvironmentConfig.AccountingUrl}/signin-oidc" },
                    FrontChannelLogoutUri = $"{EnvironmentConfig.AccountingUrl}/signout-oidc",
                    PostLogoutRedirectUris = { $"{EnvironmentConfig.AccountingUrl}/signout-callback-oidc" },

                    AllowOfflineAccess = true,
                    AllowedScopes = { "openid", "profile", "accounting", "roles" },
                    RequirePkce = false
                },
            };
    }
}