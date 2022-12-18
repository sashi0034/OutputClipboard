using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace OutputClipboard.Views
{
    internal static class Util
    {
        public static void CloseWindowDelayed(Window window)
        {
            window.Dispatcher.Invoke(async () =>
            {
                await Task.Delay(2000);
                window.Close();
            });
        }
    }
}
