using Microsoft.AspNet.Hosting;
using System;

namespace Q3Server
{
    public interface IUserAccessor
    {
        string User { get; }
    }

    public class UserAccessor : IUserAccessor
    {
        IHttpContextAccessor context;

        public UserAccessor(IHttpContextAccessor context)
        {
            this.context = context;
        }

        public string User
        {
            get
            {
                return context.Value.User.Identity.Name;
            }
        }
    }
}