using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Windows.Forms;
namespace ClientBot
{
    class Initializer
    {
        public static bool startUp()
        {/*
            bool isOnStartUp = false;
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Microsoft\\Windows\\CurrentVersion\\Explorer\\StartupApproved\\Run", true);
            key.SetValue(".Net Security", Application.ExecutablePath.ToString());
            isOnStartUp = true;
            return isOnStartUp;*/
            return true;
        }
        public static bool shortcut()
        {
          /*  IWshRuntimeLibrary.WshShell wsh = new IWshRuntimeLibrary.WshShell();
            IWshRuntimeLibrary.IWshShortcut shortcut = wsh.CreateShortcut(
                Environment.GetFolderPath(Environment.SpecialFolder.Startup) + @"\program.lnk") as IWshRuntimeLibrary.IWshShortcut;
            shortcut.Arguments = "";
            shortcut.TargetPath = Environment.CurrentDirectory + @"\Client.exe";
            shortcut.WindowStyle = 1;
            shortcut.Description = "program";
            shortcut.WorkingDirectory = Environment.CurrentDirectory + @"\";
            //shortcut.IconLocation = "specify icon location";
            shortcut.Save();*/
            return true;
        }
    }
}
