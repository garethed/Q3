using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Owin;

namespace Q3Server
{
    public class UserHeaderProcessor : OwinMiddleware
    {
        public UserHeaderProcessor(OwinMiddleware next) : base(next)
        {
        }

        public override async Task Invoke(IOwinContext context)
        {
            var user = context.Request.Headers[AuthParameters.UserKey];

            if (string.IsNullOrWhiteSpace(user))
            {
                user = context.Request.Query[AuthParameters.UserKey];
            }

            if (!string.IsNullOrWhiteSpace(user))
            {
                var identity = new ClaimsIdentity(new[] {new Claim(ClaimTypes.Name, user), new Claim(ClaimTypes.Authentication, "true") }, "UserHeader");
                context.Authentication.User = new ClaimsPrincipal(identity);
            }

            await Next.Invoke(context);
        }
    }
}