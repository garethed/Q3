using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
using WpfAnimatedGif;

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
            text.Inlines.Add(new Bold(new Run(queue.Name)) { FontSize = 20});
            text.Inlines.Add(new Run(" queue"));

            this.LabelTitle.Text = queue.Name;


            MembersChanged();

            queue.PropertyChanged += QueuePropertyChanged;

            this.Loaded += OnLoaded;
            
        }

        public Queue Queue { get { return queue; } }

        private void QueuePropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            switch (propertyChangedEventArgs.PropertyName)
            {
                case "Members":
                    MembersChanged();
                    break;
            }
        }

        private async void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            await UpdateHashtagImage();
        }

        private void RaiseFlashEvent()
        {
            Trace.WriteLine("flash fired");
            RaiseEvent(new RoutedEventArgs(FlashEvent));
        }

        private async Task UpdateHashtagImage()
        {
            var hashtag = HashtagParser.FindHashtags(queue.Name).FirstOrDefault();

            if (hashtag != null)
            {
                // BitmapImage here gives an odd error - https://wpfanimatedgif.codeplex.com/discussions/439040
                var image = BitmapFrame.Create(new Uri("https://softwire.ontoast.io/hashtags/image/" + hashtag, UriKind.Absolute));

                ImageBehavior.SetAnimatedSource(HashtagImage, image);
            }
        }


        private void MembersChanged()
        {
            var oldUsers = Members.Children.Cast<Avatar>().ToList();
            var newUsers = queue.Members.ToList();

            var removedCount = 0;
            var newIndex = 0;

            // First look for any existing users who need to be removed
            // Don't just clear the children since creating new user UI triggers the flash animation
            for (var oldIndex = 0; oldIndex < oldUsers.Count; ++oldIndex)
            {
                if (newIndex < newUsers.Count && oldUsers[oldIndex].User.Equals(newUsers[newIndex]))
                {
                    newIndex++;
                }
                else
                {
                    Members.Children.Remove(oldUsers[oldIndex]);
                    removedCount++;
                }
            }

            // All remaining users are new and need to be added
            for (int i = oldUsers.Count - removedCount; i < newUsers.Count; ++i)
            {
                Members.Children.Add(new Avatar(newUsers[i]));
            }            
        }


        public event EventHandler<QueueActionEventArgs> JoinQueue;
        public event EventHandler<QueueActionEventArgs> LeaveQueue;
        public event EventHandler<QueueActionEventArgs> ActivateQueue;
        public event EventHandler<QueueActionEventArgs> CloseQueue;

        public static readonly RoutedEvent FlashEvent = EventManager.RegisterRoutedEvent("Flash", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(QueueNotification));

        public event RoutedEventHandler Flash
        {
            add
            {
                this.AddHandler(FlashEvent, value);
            }

            remove
            {
                this.RemoveHandler(FlashEvent, value);
            }
        }


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
