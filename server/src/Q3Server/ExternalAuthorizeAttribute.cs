using System.Security.Claims;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace Q3Server
{
    // This class bypasses authorization for internal clients. These are indicated by the absence
    // of the header added by the proxy.
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