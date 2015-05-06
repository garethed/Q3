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
    public partial class QueueNotification : Window
    {
        private Queue queue;

        public QueueNotification(Queue queue)
        {
            this.queue = queue;
            InitializeComponent();

            var text = new TextBlock();
            text.Inlines.Add(new Bold(new Run(queue.Members.First())));
            text.Inlines.Add(new Run(" has started a "));
            text.Inlines.Add(new Bold(new Run(queue.Name)));
            text.Inlines.Add(new Run(" queue"));

            this.LabelTitle.Content = text;
        }        
    }
}
