using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using ConnectionState = Microsoft.AspNet.SignalR.Client.ConnectionState;

namespace Q3Client
{
    using System.Threading;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private Hub hub;
        private User user;
        private QueueUpdater queueUpdater;
        private GroupsCache groupsCache;

        private Mutex singleInstanceMutex;
        private bool mutexAcquired;

        protected async override void OnStartup(StartupEventArgs e)
        {
            if (HasAnotherInstanceRunning())
            {
                Current.Shutdown();
            }

            InitLog();

            base.OnStartup(e);

            this.DispatcherUnhandledException += App_DispatcherUnhandledException;

            var config = DataCache.Load<UserConfig>();
            if (config == null)
            {
                config = new UserConfig();
                config.FirstRun = false;
                StartupRegistration.IsRegisteredForStartup = true;
                DataCache.Save(config);
            }

            user = GetUser();

            if (user == null)
            {
                Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
                var editUser = new EditUser();
                editUser.ShowDialog();
                user = new User()
                {
                    EmailAddress = editUser.FirstName + "." + editUser.LastName + "@placeholder",
                    FullName = editUser.FirstName + " " + editUser.LastName,
                    UserName = editUser.FirstName + "~" + editUser.LastName
                };
                DataCache.Save(user);
                Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
            }

            hub = new Hub(user, Dispatcher);
            groupsCache = new GroupsCache();            
            queueUpdater = new QueueUpdater(hub, user, groupsCache);

            hub.QueueMembershipChanged += QueueMembershipChanged;
            hub.QueueCreated += QueueCreated;
            hub.QueueStatusChanged += QueueStatusChanged;
            hub.QueueMessageReceived += QueueMessageReceived;
            hub.QueueNagged += QueueNagged;

            hub.PropertyChanged += HubPropertyChanged;

            if (hub.ConnectionState == ConnectionState.Connected)
            {
                await queueUpdater.RefreshALl();
            }
        }

        private bool HasAnotherInstanceRunning()
        {
            var currentDistinguishedName = UserPrincipal.Current.DistinguishedName;
            singleInstanceMutex = singleInstanceMutex ?? new Mutex(true, currentDistinguishedName + ":{411C91EA-7B41-49DB-8CB9-20D5B58A75F7}");
            try
            {
                mutexAcquired = singleInstanceMutex.WaitOne(TimeSpan.Zero, true);
            }
            catch (AbandonedMutexException)
            {
                mutexAcquired = true;
            }
            return !mutexAcquired; // If mutex is acquired, no other instance is running
        }

        protected override void OnExit(ExitEventArgs e)
        {
            DisposeMutex();
            base.OnExit(e);
        }

        private void DisposeMutex()
        {
            if (singleInstanceMutex == null)
            {
                return;
            }
            if (mutexAcquired)
            {
                singleInstanceMutex.ReleaseMutex();
            }
            singleInstanceMutex.Close();
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            logger.Fatal(e.Exception, "Unhandled exception on dispatcher thread");
        }

        private void InitLog()
        {
            var config = new LoggingConfiguration();
            var basePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Q3");
            //var basePath = AppDomain.CurrentDomain.BaseDirectory;

            var fileTarget = new FileTarget()
            {
                FileName = Path.Combine(basePath, "activity.log"),
                ArchiveFileName = Path.Combine(basePath, "activity.{#####}.log"),
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

            logger.Info("startup " + Assembly.GetExecutingAssembly().GetName().ToString());
        }

        private void QueueMessageReceived(object sender, QueueMessageEventArgs queueMessageEventArgs)
        {
            Dispatcher.Invoke(
                () =>
                    queueUpdater.AddQueueMessage(queueMessageEventArgs.QueueId, queueMessageEventArgs.Sender,
                        queueMessageEventArgs.Message));
        }

        private void QueueNagged(object sender, QueueIdEventArgs e)
        {
            Dispatcher.Invoke(
                () =>
                    queueUpdater.NagQueue(e.QueueId));
        }

        private async void HubPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ConnectionState")
            {
                if (hub.ConnectionState == ConnectionState.Connected)
                {
                    logger.Info("Connected. Refresh all");
                    await queueUpdater.RefreshALl().ConfigureAwait(false);
                }
            }
        }

        private void QueueStatusChanged(object sender, QueueActionEventArgs args)
        {
            var queue = args.Queue;
            Dispatcher.Invoke(() => queueUpdater.UpdateQueueStatus(queue));
        }

        private void QueueMembershipChanged(object sender, QueueActionEventArgs args)
        {
            Dispatcher.Invoke(() => queueUpdater.UpdateQueue(args.Queue));
        }


        private void QueueCreated(object sender, QueueActionEventArgs args)
        {
            var queue = args.Queue;
            Dispatcher.Invoke(() => queueUpdater.AddQueue(queue));
        }

        private User GetUser()
        {
            User user;

            try
            {
                var principal = UserPrincipal.Current;

                user = new User()
                {
                    UserName = principal.SamAccountName,
                    EmailAddress = principal.EmailAddress,
                    FullName = principal.DisplayName
                };
                DataCache.Save(user);

            }
            catch (Exception e)
            {
                logger.Warn("Failed to read user from AD. Using cache");
                user = DataCache.Load<User>();
            }

#if DEBUG 
            user = user ?? new User();
            var suffix = (DateTime.Now.Ticks % 100).ToString();
            var alphabet = "abcdefghijklmnopqrstuvwxyz";
            user.UserName = user.UserName + suffix;
            user.FullName = user.FullName + suffix;

            var first = (int) (DateTime.Now.Ticks%26);
            var second = (int) ((DateTime.Now.Ticks/26) %26);

            user.EmailAddress = alphabet[first] + "." + alphabet[second] + "@softwire.com";

#endif
            return user;
        }

        private void Application_Deactivated(object sender, EventArgs e)
        {
            logger.Info("app deactivated");
        }

    }
}
