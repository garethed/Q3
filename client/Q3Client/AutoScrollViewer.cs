using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Q3Client
{
    class AutoScrollViewer : ScrollViewer
    {
        protected override void OnScrollChanged(ScrollChangedEventArgs e)
        {
            base.OnScrollChanged(e);

            if (e.ExtentHeightChange != 0)
            {   
                // Content changed
                ScrollToVerticalOffset(ExtentHeight);
            }
        }
    }
}
