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
using System.Windows.Shapes;

namespace Q3Client
{
    /// <summary>
    /// Interaction logic for QueueNotification.xaml
    /// </summary>
    public partial class QueueNotification : UserControl
    {
        private readonly Queue queue;
        private readonly string userId;

        public QueueNotification(Queue queue, string userId)
        {
            this.queue = queue;
            this.userId = userId;
            InitializeComponent();

            var text = new TextBlock();
            text.Inlines.Add(new Bold(new Run(queue.Members.FirstOrDefault() ?? "Someone") { FontSize = 20 }));
            text.Inlines.Add(new Run(" has started a "));
            text.Inlines.Add(new Bold(new Run(queue.Name)) { FontSize = 20});
            text.Inlines.Add(new Run(" queue"));

            this.LabelTitle.Content = text;

            this.queue.MembersChanged += MembersChanged;
            this.queue.StatusChanged += StatusChanged;

            updateButtons();
        }

        private void StatusChanged(object sender, EventArgs eventArgs)
        {
            if (queue.Status == QueueStatus.Closed)
            {
                //qq this.Close();
                return;
            }

            updateButtons();
        }

        private void MembersChanged(object sender, EventArgs eventArgs)
        {
            updateButtons();
        }

        private void updateButtons()
        {
            var containsMe = (queue.Members.Contains(userId));
            ButtonJoin.Visibility = containsMe ? Visibility.Collapsed : Visibility.Visible;
            ButtonLeave.Visibility = !containsMe ? Visibility.Collapsed : Visibility.Visible;
        }

        public event EventHandler<QueueActionEventArgs> JoinQueue;
        public event EventHandler<QueueActionEventArgs> LeaveQueue;

        private void ButtonJoin_Click(object sender, RoutedEventArgs e)
        {
            JoinQueue.SafeInvoke(this, new QueueActionEventArgs(queue));
        }

        private void ButtonLeave_Click(object sender, RoutedEventArgs e)
        {
            LeaveQueue.SafeInvoke(this, new QueueActionEventArgs(queue));
        }
    }
}
