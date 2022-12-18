using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace OutputClipboard
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private const string appName = "OutputClipboard";

        public App()
        {
            var identity = System.Security.Principal.WindowsIdentity.GetCurrent();
            var principal = new System.Security.Principal.WindowsPrincipal(identity);

            if (principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator))
            {
                setupExplolerContextMenu();

                new Views.WindowFinishSetup().Show();
            }
            else
            {
                new Views.WindowOutputClipboard().Show();
            }
        }

        // 参考: https://dobon.net/vb/dotnet/system/explorecontextmenu.html
        private static void setupExplolerContextMenu()
        {
            string commandline = "\"" + $"{GetCurrentAppDir()}\\{appName}.exe" + "\" \"%1\"";
            const string description = "クリップボードを保存(&V)";

            string path = $"Directory\\background\\shell\\{appName}";

            //フォルダへの関連付けを行う
            Microsoft.Win32.RegistryKey cmdkey =
                Microsoft.Win32.Registry.ClassesRoot.CreateSubKey(
                    $"{path}\\command");
            cmdkey.SetValue("", commandline);
            cmdkey.Close();

            //説明を書き込む
            Microsoft.Win32.RegistryKey verbkey =
                Microsoft.Win32.Registry.ClassesRoot.CreateSubKey($"{path}");
            verbkey.SetValue("", description);
            verbkey.Close();
        }

        public static string? GetCurrentAppDir()
        {
            return System.IO.Path.GetDirectoryName(
                System.Reflection.Assembly.GetExecutingAssembly().Location);
        }
    }
}
