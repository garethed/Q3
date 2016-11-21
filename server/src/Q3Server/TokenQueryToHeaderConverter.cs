using System.Threading.Tasks;
using Microsoft.Owin;

namespace Q3Server
{
    internal class TokenQueryToHeaderConverter : OwinMiddleware
    {
        public TokenQueryToHeaderConverter(OwinMiddleware next) : base(next)
        {
        }

        public override async Task Invoke(IOwinContext context)
        {
            var token = context.Request.Query[AuthParameters.TokenKey];

            if (!string.IsNullOrEmpty(token))
            {
                context.Request.Headers["Authorization"] = "Bearer " + token;
            }

            await Next.Invoke(context);
        }
    }
}