using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Logging;
using System.Diagnostics;
using System.IdentityModel.Tokens;
using Pysco68.Owin.Logging.NLogAdapter;
using System.Web.Hosting;
using NLog.Config;
using System.IO;
using System.Runtime.Caching;
using NLog.Targets;
using NLog;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Security.ActiveDirectory;

[assembly: OwinStartup(typeof(Q3Server.Startup))]

namespace Q3Server
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            initLog();
            app.UseNLog();

            var manager = new QManager(new QEventsListener(GlobalHost.ConnectionManager));

            var userGetter = new UserGetter(new UserGetterSerialized(), new UserGetterDomain());
            var userGetterCached = new CachedObjectGetter<User>(MemoryCache.Default, userGetter);

            var groupGetterCached = new CachedObjectGetter<List<string>>(MemoryCache.Default, new GroupGetterDomain());

            GlobalHost.DependencyResolver.Register(
                typeof(QHub),
                () => new QHub(
                    manager,
                    app.CreateLogger<QHub>(),
                    userGetterCached,
                    groupGetterCached));

            // The AuthParameterChecker must come first in the middleware and auth pipeline, as it ensures
            // both external and internal requests are attempting to authenticate in the correct ways.
            app.Use<AuthParameterChecker>();
            app.Use<UserHeaderProcessor>();
            app.SetUpAzureAdAuth();

            app.UseCors(CorsOptions.AllowAll);
            app.MapSignalR();

            var logger = LogManager.GetCurrentClassLogger();
            logger.Info("Application started");
        }

        private void initLog()
        {
            var path = HostingEnvironment.MapPath("~/App_Data");

            var config = new LoggingConfiguration();

            var fileTarget = new FileTarget()
            {
                FileName = Path.Combine(path, "activity.log"),
                ArchiveFileName = Path.Combine(path, "activity.{#####}.log"),
                ArchiveAboveSize = 1024 * 1024,
                ArchiveNumbering = ArchiveNumberingMode.Sequence,
                ConcurrentWrites = false,
                Layout = "${longdate} | ${level} | ${logger} | ${message} ${exception:format=tostring}",
                AutoFlush = true,
                MaxArchiveFiles = 50
            };

            config.AddTarget("file", fileTarget);
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Info, fileTarget));

            var traceTarget = new TraceTarget() { Layout = "${level} | ${logger} | ${message} ${exception:format=tostring}" };
            config.AddTarget("trace", traceTarget);
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Info, traceTarget));

            LogManager.Configuration = config;
        }
    }
}
