using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Q3Server
{
    internal class SimpleHeaderAuthenticator
    {
        private RequestDelegate next;

        public SimpleHeaderAuthenticator(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var user = context.Request.Headers["User"];

            if (!string.IsNullOrWhiteSpace(user))
            {
                var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, user), new Claim(ClaimTypes.Authentication, "true") }, "SimpleHeader");
                context.User = new ClaimsPrincipal(identity);

                await this.next(context);
            }
            else
            {
                context.Response.StatusCode = 401;
            }
        }
    }
}