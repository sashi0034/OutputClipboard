using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace OutputClipboard.Views
{
    internal static class Util
    {
        public static void StartPerformPopup(Window window)
        {
            window.Dispatcher.Invoke(() => performPopup(window));
        }

        private static async Task performPopup(Window window)
        {
            MoveWindowToNearCursor(window);
            await CloseWindowDelayed(window);
        }

        public static void MoveWindowToNearCursor(Window window)
        {
            var pos = System.Windows.Forms.Cursor.Position;
            window.Left = pos.X;
            window.Top = pos.Y;
        }

        public static async Task CloseWindowDelayed(Window window)
        {
            await Task.Delay(3000);
            window.Close();
        }

        // 参考: https://gogowaten.hatenablog.com/entry/2019/11/12/201852
        // Autohr: gogowaten
        public static BitmapSource? GetClipboardBitmapDIB()
        {
            var data = Clipboard.GetDataObject();
            if (data == null) return null;

            var ms = data.GetData("DeviceIndependentBitmap") as System.IO.MemoryStream;
            if (ms == null) return null;

            //DeviceIndependentBitmapのbyte配列の15番目がbpp、
            //これが32未満ならBgr32へ変換、これでアルファの値が0でも255扱いになって表示される
            byte[] dib = ms.ToArray();
            if (dib[14] < 32)
            {
                return new FormatConvertedBitmap(Clipboard.GetImage(), PixelFormats.Bgr32, null, 0);
            }
            else
            {
                return Clipboard.GetImage();
            }
        }
    }
}
