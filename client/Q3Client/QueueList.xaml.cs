using NLog;
using System;
using System.Collections.Generic;
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

namespace Q3Client
{
    /// <summary>
    /// Interaction logic for QueueList.xaml
    /// </summary>
    public partial class QueueList : Window
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public enum eWindowStateExtended
        {
            Normal,
            Minimized,
            Closed
        }

        public QueueList(Hub hub, GroupsCache groupsCache)
        {
            InitializeComponent();
            Header.Hub = hub;
            Header.GroupsCache = groupsCache;

            this.Activated += (sender, args) => logger.Debug("Activated");
            this.Deactivated+= (sender, args) => logger.Debug("Deactivated");
            this.LocationChanged += (sender, args) => AdjustHeight();
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            this.Left = SystemParameters.WorkArea.Right - this.Width;
            AdjustHeight();           
        }

        public void AdjustHeight()
        {
            if (this.Top < SystemParameters.WorkArea.Bottom)
            {
                this.Height = SystemParameters.WorkArea.Bottom - this.Top;
            }
        }

        private void ShowQueuesClicked(object sender, RoutedEventArgs e)
        {
            WindowStateExtended = eWindowStateExtended.Normal;
        }

        private void StartQueueClicked(object sender, RoutedEventArgs e)
        {
            Header.NewQueueClicked(sender, e);
        }

        private void QuitClicked(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public eWindowStateExtended WindowStateExtended
        {
            get { return this.Visibility == Visibility.Hidden ? eWindowStateExtended.Closed : 
                (WindowState == WindowState.Minimized ? eWindowStateExtended.Minimized : eWindowStateExtended.Normal); }
            set
            {
                switch (value)
                {
                    case eWindowStateExtended.Closed:
                        ShowInTaskbar = false;
                        Hide();
                        break;
                    case eWindowStateExtended.Minimized:
                        ShowInTaskbar = true;
                        WindowState = WindowState.Minimized;
                        Show();
                        break;
                    case eWindowStateExtended.Normal:
                        ShowInTaskbar = true;
                        WindowState = WindowState.Normal;
                        Topmost = true;
                        Show();
                        Topmost = false;
                        break;
                }
            }

        }

        internal void RestoreHidden()
        {
            foreach (QueueNotification queue in QueuesPanel.Children)
            {
                if (queue.Queue.Status != QueueStatus.Closed && queue.Visibility == Visibility.Collapsed)
                {
                    queue.Visibility = Visibility.Visible;
                }
            }
        }

        private void ShowClient(object sender, RoutedEventArgs e)
        {
            WindowStateExtended = eWindowStateExtended.Normal;
        }
    }

}
