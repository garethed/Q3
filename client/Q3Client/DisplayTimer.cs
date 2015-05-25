using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Q3Client
{
    class DisplayTimer
    {
        private QueueList targetWindow;
        private QueueList.eWindowStateExtended previousState;
        private volatile bool cancel;

        public DisplayTimer(QueueList targetWindow)
        {
            this.targetWindow = targetWindow;
            targetWindow.GotFocus += (s, e) => cancel = true;
        }

        public async void ShowAlert()
        {
            if (!targetWindow.IsActive)
            {
                cancel = false;

                if (targetWindow.WindowStateExtended != QueueList.eWindowStateExtended.Normal)
                {
                    previousState = targetWindow.WindowStateExtended;
                    targetWindow.WindowStateExtended = QueueList.eWindowStateExtended.Normal;
                    await Task.Delay(TimeSpan.FromSeconds(4));
                    if (!cancel)
                    {
                        targetWindow.WindowStateExtended = previousState;
                    }
                }
                else
                {
                    Win32.BringToFront(targetWindow);
                    await Task.Delay(TimeSpan.FromSeconds(4));
                    if (!cancel && !targetWindow.IsActive)
                    {
                        Win32.SendToBack(targetWindow);
                    }
                }
            }

        }
    }
}
