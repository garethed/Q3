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

        public QueueNotification(Queue queue)
        {
            this.queue = queue;
            InitializeComponent();

            this.DataContext = queue;

            var text = new TextBlock();
            text.Inlines.Add(new Bold(new Run(queue.Members.FirstOrDefault() ?? "Someone") { FontSize = 20 }));
            text.Inlines.Add(new Run(" has started a "));
            text.Inlines.Add(new Bold(new Run(queue.Name)) { FontSize = 20});
            text.Inlines.Add(new Run(" queue"));

            this.LabelTitle.Content = text;

            updateButtons();
        }

        private void StatusChanged(object sender, EventArgs eventArgs)
        {
            switch (queue.Status)
            {
                    case QueueStatus.Waiting:
                    break;
                    case QueueStatus.Closed:
                    break;
                    case QueueStatus.Activated:
                    break;
            }

            updateButtons();
        }

        private void MembersChanged(object sender, EventArgs eventArgs)
        {
            updateButtons();
        }

        private void updateButtons()
        {
          /*  var containsMe = (queue.Members.Contains(userId));
            ButtonJoin.Visibility = VisibleIf(!containsMe);
            ButtonLeave.Visibility = VisibleIf(containsMe);
            ButtonActivate.Visibility = VisibleIf(queue.Status == QueueStatus.Waiting);*/
        }

        private Visibility VisibleIf(bool visible)
        {
            return visible ? Visibility.Visible : Visibility.Collapsed;
        }

        public event EventHandler<QueueActionEventArgs> JoinQueue;
        public event EventHandler<QueueActionEventArgs> LeaveQueue;
        public event EventHandler<QueueActionEventArgs> ActivateQueue;
        public event EventHandler<QueueActionEventArgs> CloseQueue;

        private void ButtonJoin_Click(object sender, RoutedEventArgs e)
        {
            JoinQueue.SafeInvoke(this, new QueueActionEventArgs(queue));
        }

        private void ButtonLeave_Click(object sender, RoutedEventArgs e)
        {
            LeaveQueue.SafeInvoke(this, new QueueActionEventArgs(queue));
        }

        private void ButtonActivate_Click(object sender, RoutedEventArgs e)
        {
            ActivateQueue.SafeInvoke(this, new QueueActionEventArgs(queue));
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            CloseQueue.SafeInvoke(this, new QueueActionEventArgs(queue));
        }
    }
}
