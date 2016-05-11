using System.Threading.Tasks;
using Microsoft.Owin;

namespace Q3Server
{
    public class AuthParameterChecker : OwinMiddleware
    {
        public AuthParameterChecker(OwinMiddleware next) : base(next)
        {
        }

        public override async Task Invoke(IOwinContext context)
        {
            if (HasCorrectParameterCombination(context.Request))
            {
                await Next.Invoke(context);
            }
            else
            {
                context.Response.StatusCode = 400;
            }
        }

        private bool HasCorrectParameterCombination(IOwinRequest request)
        {
            var externalRequest = !string.IsNullOrEmpty(request.Headers[AuthParameters.ExternalHeaderKey]);
            var hasAuthToken = !string.IsNullOrEmpty(request.Query[AuthParameters.TokenKey]);
            var hasUserParameter = !string.IsNullOrEmpty(request.Headers[AuthParameters.UserKey]) 
                || !string.IsNullOrEmpty(request.Query[AuthParameters.UserKey]);

            if (externalRequest)
            {
                return hasAuthToken && !hasUserParameter;
            }
            else
            {
                return hasUserParameter;
            }
        }
    }
}