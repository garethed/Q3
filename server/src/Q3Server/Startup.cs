using System;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Microsoft.Framework.DependencyInjection;

namespace Q3Server
{
    public class Startup
    {
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSignalR();
            services.AddSingleton<IQManager, QManager>();
            services.AddSingleton<IQEventsListener, QEventsListener>();
            services.AddTransient<IUserAccessor, UserAccessor>();
            services.AddInstance<IGroupCache>(new GroupCache());
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseMiddleware<SimpleHeaderAuthenticator>();
            app.UseSignalR();            
        }
    }
}
