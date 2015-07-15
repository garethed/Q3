using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Q3Client
{
    static class StartupRegistration
    {
        public static bool IsRegisteredForStartup
        {
            get
            {
                return File.Exists(StartupShortcutPath);
            }
            set
            {
                if (IsRegisteredForStartup != value)
                {
                    if (value)
                    {
                        RegisterForStartup();
                    }
                    else
                    {
                        UnregisterForStartup();
                    }
                }

            }
        }

        private static void UnregisterForStartup()
        {
            File.Delete(StartupShortcutPath);
        }

        private static void RegisterForStartup()
        {
            if (!IsRegisteredForStartup)
            {
                CreateShortcut(ApplicationName, LaunchingApplicationFilename);
            }
        }

        private static void CreateShortcut(string name, string targetLocation)
        {
            Type t = Type.GetTypeFromCLSID(new Guid("72C24DD5-D70A-438B-8A42-98424B88AFB8")); //Windows Script Host Shell Object
            dynamic shell = Activator.CreateInstance(t);
            try
            {
                var lnk = shell.CreateShortcut(StartupShortcutPath);
                try
                {
                    lnk.TargetPath = targetLocation;
                    lnk.IconLocation = targetLocation + ", 0";
                    lnk.Save();
                }
                finally
                {
                    Marshal.FinalReleaseComObject(lnk);
                }
            }
            finally
            {
                Marshal.FinalReleaseComObject(shell);
            }
        }

        private static string StartupShortcutFolder
        {
            get { return Environment.GetFolderPath(Environment.SpecialFolder.Startup); }
        }

        private static string StartupShortcutPath
        {
            get { return StartupShortcutFolder + @"\" + ApplicationName + ".lnk"; }
        }

        private static string LaunchingApplicationFilename
        {
            get { return Environment.GetCommandLineArgs().First(); }
        }

        private static string ApplicationName
        {
            get { return Path.GetFileNameWithoutExtension(LaunchingApplicationFilename); }
        }

    }
}
