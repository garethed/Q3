using System;
using System.Collections.Generic;
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


        public MainWindow()
        {
            InitializeComponent();
            hubConnection = new HubConnection("http://localhost:51442/");
            hub = hubConnection.CreateHubProxy("QHub");
            hub.On<string>("NewQueue", NewQueue);
            hub.On<string, IEnumerable<string>>("QueueMembershipChanged", QueueMembershipChanged);
            hub.On<string>("QueueStatusChanged", QueueStatusChanged);
            hubConnection.Start().Wait();

            this.Activated += OnActivated;
        }

        private void QueueStatusChanged(string queueName)
        {
            UpdateLabel("queue activated: " + queueName);
        }

        private void QueueMembershipChanged(string name, IEnumerable<string> members)
        {
            UpdateLabel("membership changed: " + name + " - " + string.Join(", ", members));
        }

        private async void OnActivated(object sender, EventArgs eventArgs)
        {
            var queues = await hub.Invoke<IEnumerable<string>>("ListQueues");
            UpdateLabel("existing queues " + string.Join(", ", queues));
        }

        private void NewQueue(string queueName)
        {
            UpdateLabel("new queue: " + queueName);
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
            await hub.Invoke("JoinQueue", QueueName.Text);

        }

        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            await hub.Invoke("ActivateQueue", QueueName.Text);
        }

    }
}
