using System.Configuration;
using System.IdentityModel.Tokens;
using Microsoft.Owin.Security.ActiveDirectory;
using Owin;

namespace Q3Server
{
    internal static class AppBuilderExtensions
    {
        internal static IAppBuilder SetUpAzureAdAuth(this IAppBuilder app)
        {
            // The signalR client can only send the token in a query string, but the Azure AD package
            // expects it in an "Authorization: Bearer" header.
            app.Use<TokenQueryToHeaderConverter>();

            app.UseWindowsAzureActiveDirectoryBearerAuthentication(
                new WindowsAzureActiveDirectoryBearerAuthenticationOptions
                {
                    Tenant = ConfigurationManager.AppSettings["azureTenant"],
                    TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidAudience = ConfigurationManager.AppSettings["serverAppId"]
                    }
                }
            );

            return app;
        }
    }
}
