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
        private HubConnection hubConnection;
        private IHubProxy hub;
        private ObservableCollection<Queue> queues = new ObservableCollection<Queue>(); 


        public MainWindow()
        {
            InitializeComponent();
            hubConnection = new HubConnection("http://localhost:51442/");
            hub = hubConnection.CreateHubProxy("QHub");
            hub.On<Queue>("NewQueue", NewQueue);
            hub.On<Queue>("QueueMembershipChanged", QueueMembershipChanged);
            hub.On<Queue>("QueueStatusChanged", QueueStatusChanged);
            hubConnection.Headers["User"] = DateTime.Now.Ticks.ToString();
            hubConnection.Start().Wait();

            this.Activated += OnActivated;
        }

        private void QueueStatusChanged(Queue queue)
        {
            UpdateLabel("queue activated: " + queue);
            if (queue.Status == QueueStatus.Closed)
            {
                Dispatcher.Invoke(() => queues.Remove(queue));
                updateList();
            }
        }

        private void QueueMembershipChanged(Queue queue)
        {
            UpdateLabel("membership changed: " + queue);
        }

        private async void OnActivated(object sender, EventArgs eventArgs)
        {
            var queues = await hub.Invoke<IEnumerable<Queue>>("ListQueues");
            UpdateLabel("existing queues " + string.Join(", ", queues));
            this.queues.Clear();
            foreach (var q in queues)
            {
                this.queues.Add(q);
            }
            
            updateList();
        }

        private void updateList()
        {
            Dispatcher.Invoke(() =>
            {
                QueueList.ItemsSource = queues;
                QueueList.InvalidateVisual();
            });
        }

        private void NewQueue(Queue queue)
        {
            UpdateLabel("new queue: " + queue);
            Dispatcher.Invoke(() => queues.Add(queue));
            updateList();
        }

        private void UpdateLabel(string message)
        {
            Dispatcher.Invoke(() => MessageLabel.Content += "\n" + message);
            
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await hub.Invoke("StartQueue", QueueName.Text);
        }

        private async void JoinQueue_Click(object sender, RoutedEventArgs e)
        {
            await hub.Invoke("JoinQueue", SelectedQueueId);
        }

        public int SelectedQueueId
        {
            get { return ((Queue) QueueList.SelectedItem).Id; } 
        }

        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            await hub.Invoke("ActivateQueue", SelectedQueueId);
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            await hub.Invoke("CloseQueue", SelectedQueueId);
        }

    }
}
