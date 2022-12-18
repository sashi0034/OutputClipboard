using OutputClipboard.Views;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
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
        private const string contextDescription = "クリップボードの画像を保存";
        private const string shortcutKey = "&V";

        public App(){}


        private void app_Startup(object sender, StartupEventArgs e)
        {
            var identity = System.Security.Principal.WindowsIdentity.GetCurrent();
            var principal = new System.Security.Principal.WindowsPrincipal(identity);

            if (principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator))
            {
                try
                {
                    setupExplolerContextMenu();
                    MessageBox.Show($"エクスプローラーのコンテクストメニューに「contextDescription」を追加しました", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                    startShowWindow(new Views.WindowFinishSetup());
                }
                catch (Exception ex)
                {
                    MessageBox.Show("セットアップに失敗しました\n" + ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                var currDir = e.Args.Length > 0 ? e.Args[0] : Directory.GetCurrentDirectory();
                startShowWindow(new Views.WindowOutputClipboard(currDir));
            }
        }

        private static void startShowWindow(Window window)
        {
            window.ShowInTaskbar = false;
            Views.Util.MoveWindowToNearCursor(window);
            window.Show();
        }

        // 参考: https://dobon.net/vb/dotnet/system/explorecontextmenu.html
        private static void setupExplolerContextMenu()
        {
            string commandline = "\"" + $"{GetCurrentAppDir()}\\{appName}.exe" + "\" \"%v\"";
            const string description = $"{contextDescription}({shortcutKey})";

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
