using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace Q3Client
{
    public static class Win32
    {
        const uint SWP_NOACTIVATE = 0x0010;
        const uint SWP_NOMOVE = 0x0002;
        const uint SWP_NOSIZE = 0x0001;

        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        
        public static void BringToFront(Window window)
        {
            IntPtr HWND_TOP = new IntPtr(0);
            var result = SetWindowPos(new WindowInteropHelper(window).Handle, HWND_TOP, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);
            Trace.WriteLine(Marshal.GetLastWin32Error());
        }

        public static void SendToBack(Window window)
        {
            IntPtr HWND_BOTTOM = new IntPtr(1);
            SetWindowPos(new WindowInteropHelper(window).Handle, HWND_BOTTOM, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);
        }


        public static bool IsActive(Window wnd)
        {
            // workaround for minimization bug
            // Managed .IsActive may return wrong value
            if (wnd == null) return false;
            return GetForegroundWindow() == new WindowInteropHelper(wnd).Handle;
        }

        public static bool IsApplicationActive()
        {
            var active = Application.Current.Windows.OfType<Window>().Any(IsActive);
            Trace.WriteLine("is active: " + active);

            return active;
        }
    }
}
