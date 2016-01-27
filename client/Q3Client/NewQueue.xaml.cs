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
using WpfAnimatedGif;

namespace Q3Client
{
    /// <summary>
    /// Interaction logic for NewQueue.xaml
    /// </summary>
    public partial class NewQueue : Window
    {
        private readonly string[] defaultGroups = { "Office - London", "Office - Bristol", "Office - Romania" };

        private readonly GroupsCache groupsCache;

        public NewQueue(GroupsCache groupsCache)
        {
            this.groupsCache = groupsCache;
            InitializeComponent();

            var defaultGroup = groupsCache.Groups.Intersect(defaultGroups).FirstOrDefault();

            if (defaultGroup != null)
            {
                GroupSelector.SelectedItem = defaultGroup;

            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {   
            NewQueueName = QueueName.Text;
            Close();
        }

        public string NewQueueName { get; private set; }

        private void Window_Activated(object sender, EventArgs e)
        {
            QueueName.Focus();
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }

        public IEnumerable<string> GroupList
        {
            get { return groupsCache.Groups; }
        }

        private async Task UpdateHashtagImage()
        {
            var hashtag = HashtagParser.FindHashtags(QueueName.Text).FirstOrDefault();
            var queueName = QueueName.Text;

            if (hashtag != null)
            {
                // Debounce to prevent loading while typing
                await Task.Delay(TimeSpan.FromSeconds(0.3));

                if (QueueName.Text == queueName)
                {
                    // BitmapImage here gives an odd error - https://wpfanimatedgif.codeplex.com/discussions/439040
                    var image = BitmapFrame.Create(new Uri("https://softwire.ontoast.io/hashtags/image/" + hashtag, UriKind.Absolute));

                    ImageBehavior.SetAnimatedSource(HashtagImage, image);
                }
            }
        }

        private async void QueueName_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateHashtagImage();
        }
    }
}
