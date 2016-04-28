using System.Threading.Tasks;
using Microsoft.Owin;

namespace Q3Server
{
    internal class TokenQueryToHeaderConverter : OwinMiddleware
    {
        public TokenQueryToHeaderConverter(OwinMiddleware next) : base(next)
        {
        }

        public async override Task Invoke(IOwinContext context)
        {
            var token = context.Request.Query["adToken"];

            if (string.IsNullOrEmpty(token))
            {
                context.Response.StatusCode = 401;
                return;
            }

            context.Request.Headers["Authorization"] = "Bearer " + token;

            await Next.Invoke(context);
        }
    }
}