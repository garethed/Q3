using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Logging;
using System.Diagnostics;

[assembly: OwinStartup(typeof(Q3Server.Startup))]

namespace Q3Server
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var manager = new QManager(new QEventsListener(GlobalHost.ConnectionManager));

            GlobalHost.DependencyResolver.Register(
                typeof(QHub),
                () => new QHub(
                    manager,
                    app.CreateLogger<QHub>()));

            app.Use<SimpleHeaderAuthenticator>();
            app.MapSignalR();
        }
    }
}
