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

            this.LabelTitle.Content = queue.Name;


            MembersChanged();

            queue.PropertyChanged += QueuePropertyChanged;

            this.Loaded += OnLoaded;
            
        }

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
            if (queue.Members.Any())
            {

                var text = new TextBlock() { TextWrapping = TextWrapping.WrapWithOverflow};
                text.Inlines.Add(new Bold(new Run(queue.Members.First().FullName)));

                foreach (User member in queue.Members.Skip(1))
                {
                    text.Inlines.Add(new Run(", " + member.FullName));
                }
                LabelMembers.Content = text;
            }
            else
            {
                LabelMembers.Content = null;

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
