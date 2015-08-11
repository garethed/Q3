using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Owin;
using System.Diagnostics;

namespace Q3Server
{
    internal class SimpleHeaderAuthenticator : OwinMiddleware
    {
        public SimpleHeaderAuthenticator(OwinMiddleware next) : base(next)
        {
        }

        public async override Task Invoke(IOwinContext context)
        {
            var user = context.Request.Headers["User"];

            if (!string.IsNullOrWhiteSpace(user))
            {
                var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, user), new Claim(ClaimTypes.Authentication, "true") }, "SimpleHeader");
                context.Authentication.User = new ClaimsPrincipal(identity);
                Trace.WriteLine("Request from " + user + " for " + context.Request.Uri);

                await this.Next.Invoke(context);
            }
            else
            {
                context.Response.StatusCode = 401;
            }
        }
    }
}