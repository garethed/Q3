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
        private IdleTimer idleTimer;

        public DisplayTimer(QueueList targetWindow)
        {
            this.targetWindow = targetWindow;
            targetWindow.GotFocus += (s, e) => cancel = true;
            idleTimer = new IdleTimer();
            idleTimer.Start();
        }

        public async void ShowAlert()
        {
            if (!targetWindow.IsActive)
            {
                cancel = false;
                idleTimer.IsActive = false;

                Action reset;

                targetWindow.Topmost = true;

                if (targetWindow.WindowStateExtended != QueueList.eWindowStateExtended.Normal)
                {
                    previousState = targetWindow.WindowStateExtended;
                    targetWindow.WindowStateExtended = QueueList.eWindowStateExtended.Normal;
                    reset = () => targetWindow.WindowStateExtended = previousState;
                }
                else
                {
                    Win32.BringToFront(targetWindow);
                    reset = () => Win32.SendToBack(targetWindow);
                }

                while (!idleTimer.IsActive)
                {
                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
                await Task.Delay(TimeSpan.FromSeconds(3));

                targetWindow.Topmost = false;

                if (!cancel && !targetWindow.IsActive)
                {
                    reset();
                }

            }

        }
    }
}
