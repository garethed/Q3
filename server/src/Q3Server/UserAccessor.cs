using Microsoft.AspNet.Hosting;
using System;

namespace Q3Server
{
    public interface IUserAccessor
    {
        User User { get; }
    }

    public class UserAccessor : IUserAccessor
    {
        IHttpContextAccessor context;

        public UserAccessor(IHttpContextAccessor context)
        {
            this.context = context;
        }

        public User User
        {
            get
            {
                return new User(context.Value.User.Identity.Name);
            }
        }
    }
}