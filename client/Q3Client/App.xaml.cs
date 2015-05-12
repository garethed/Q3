using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Q3Client
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private Hub hub;
        private string userId;
        private QueueUpdater queueUpdater;



        protected async override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);


            userId = "user" + (DateTime.Now.Ticks % 100).ToString();

            hub = new Hub(userId);
            queueUpdater = new QueueUpdater(hub, userId);


            hub.QueueMembershipChanged += QueueMembershipChanged;
            hub.QueueCreated += QueueCreated;
            hub.QueueStatusChanged += QueueStatusChanged;

            await queueUpdater.RefreshALl();

        }
        private void QueueStatusChanged(object sender, QueueActionEventArgs args)
        {
            var queue = args.Queue;
            Dispatcher.Invoke(() => queueUpdater.UpdateQueue(queue));
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

    }
}
