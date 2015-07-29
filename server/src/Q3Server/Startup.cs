using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Q3Server.Startup))]

namespace Q3Server
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.Use<SimpleHeaderAuthenticator>();
            app.MapSignalR();
        }
    }
}
