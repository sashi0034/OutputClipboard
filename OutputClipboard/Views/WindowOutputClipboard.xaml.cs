using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Printing;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Clipboard = System.Windows.Clipboard;
using MessageBox = System.Windows.MessageBox;

namespace OutputClipboard.Views
{
    public enum WindowOutputClipboardShowing
    {
        InputFileName,
        Text
    }


    /// <summary>
    /// WindowOutputClipboard.xaml の相互作用ロジック
    /// </summary>
    public partial class WindowOutputClipboard : Window
    {
        private readonly BitmapSource? clipboardImage;
        private readonly string currDir;

        private bool _hasSaved = false;

        private string fileName
        {
            get { return inputFileName.Text; }
            set { inputFileName.Text = value; }
        }


        public WindowOutputClipboard(string currDir)
        {
            InitializeComponent();

            this.currDir = currDir;

            if ((clipboardImage = Util.GetClipboardBitmapDIB()) == null)
            {
                exitWindow("クリップボードに画像が入っていません");
                return;
            }

            fileName = $"{getTimeStampNow()}.png";

            inputFileName.Focus();
            this.Dispatcher.InvokeAsync(async () =>
            {
                await Task.Delay(0);
                inputFileName.Select(0, fileName.Length - 4);
            });
        }

        private void exitWindow(string messsage)
        {
            changeShow(WindowOutputClipboardShowing.Text);
            textResult.Text = messsage;
            Dispatcher.Invoke(() => Util.CloseWindowDelayed(this));
        }

        private string getTimeStampNow()
        {
            return DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
        }

        private void changeShow(WindowOutputClipboardShowing showing)
        {
            setVisible(inputFileName, showing == WindowOutputClipboardShowing.InputFileName);
            setVisible(textResult, showing == WindowOutputClipboardShowing.Text);
        }

        private void setVisible(UIElement element, bool isVisible)
        {
            element.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
        }



        private void inputFileName_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                saveImageAndExit();
            }
            else if (e.Key == Key.Escape)
            {
                _hasSaved = true;
                exitWindow("保存をキャンセルしました");
            }
        }

        private void window_Deactivated(object sender, EventArgs e)
        {
            if (clipboardImage==null) return;
            saveImageAndExit();
        }

        private void saveImageAndExit()
        {
            Debug.Assert(clipboardImage != null);
            if (_hasSaved) return;
            _hasSaved = false;


            if (fileName.Length < 4 || fileName.Substring(fileName.Length - 4) != ".png")
            {
                fileName += ".png";
            }

            string path = $"{currDir}\\{fileName}";
            Debug.WriteLine(path);

            try
            {
                saveImage(path, clipboardImage);
                exitWindow("画像を保存しました");
            }
            catch (Exception ex)
            {
                MessageBox.Show("セットアップに失敗しました\n" + ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                exitWindow("画像の保存に失敗しました");
            }
        }

        private static void saveImage(string path, BitmapSource image)
        {
            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                ImageSource source = image;
                BmpBitmapEncoder encoder = new BmpBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create((BitmapSource)source));
                encoder.Save(stream);
            }
        }

    }
}
