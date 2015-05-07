using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.AspNet.SignalR.Client;

namespace Q3Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<Queue> queues = new ObservableCollection<Queue>();
        private readonly Hub hub;
        private string userId;
        private readonly QueueUpdater queueUpdater;

        public MainWindow()
        {
            InitializeComponent();

            userId = "user" + (DateTime.Now.Ticks%100).ToString();

            hub = new Hub(userId);
            queueUpdater = new QueueUpdater(hub, userId);


            hub.QueueMembershipChanged += QueueMembershipChanged;
            hub.QueueCreated += QueueCreated;
            hub.QueueStatusChanged += QueueStatusChanged;
            
            this.Activated += OnActivated;

            QueueList.ItemsSource = queueUpdater.Queues;
        }

        private void QueueStatusChanged(object sender, QueueActionEventArgs args)
        {
            var queue = args.Queue;
            Dispatcher.Invoke(() =>
            {

                UpdateLabel("queue activated: " + queue);
                if (queue.Status == QueueStatus.Closed)
                {
                    queues.Remove(queue);
                }
            });
        }

        private void QueueMembershipChanged(object sender, QueueActionEventArgs args)
        {
            Dispatcher.Invoke(() =>
            {
                UpdateLabel("membership changed: " + args.Queue);
                queueUpdater.UpdateQueue(args.Queue);
            });
        }

        private async void OnActivated(object sender, EventArgs eventArgs)
        {
            await queueUpdater.RefreshALl();
            UpdateLabel("existing queues " + string.Join(", ", queueUpdater.Queues));
            
        }


        private void QueueCreated(object sender, QueueActionEventArgs args)
        {
            var queue = args.Queue;

            Dispatcher.Invoke(() =>
            {
                queueUpdater.AddQueue(queue);
                UpdateLabel("new queue: " + queue);

            });
        }

        private void UpdateLabel(string message)
        {
            Dispatcher.Invoke(() => MessageLabel.Content += "\n" + message);
            
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await hub.CreateQueue(QueueName.Text);
        }

        private async void JoinQueue_Click(object sender, RoutedEventArgs e)
        {
            await hub.JoinQueue(SelectedQueueId);
        }

        public int SelectedQueueId
        {
            get { return ((Queue) QueueList.SelectedItem).Id; } 
        }

        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            await hub.ActivateQueue(SelectedQueueId);
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            await hub.CloseQueue(SelectedQueueId);
        }

    }
}
