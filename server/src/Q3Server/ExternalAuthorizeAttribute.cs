using System.Security.Claims;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace Q3Server
{
    public class ExternalAuthorizeAttribute : AuthorizeAttribute
    {
        public override bool AuthorizeHubConnection(HubDescriptor hubDescriptor, IRequest request)
        {
            if (string.IsNullOrEmpty(request.Headers[AuthParameters.ExternalHeaderKey]))
            {
                return true;
            }

            return base.AuthorizeHubConnection(hubDescriptor, request);
        }
    }
}