using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace MifuminSoft.Sukeru
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MouseLeftButtonDown += (sender, e) => { DragMove(); };

            if (App.CommandLineArgs?.Length > 0)
            {
                var path = App.CommandLineArgs[0];
                if (File.Exists(path))
                {
                    try
                    {
                        image.Source = new BitmapImage(new Uri(path));
                    }
                    catch (Exception)
                    {
                    }

                }
            }
        }

        private void image_DragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.All;
        }

        private void image_Drop(object sender, DragEventArgs e)
        {
            var files = e.Data.GetData(DataFormats.FileDrop) as string[];
            if (files != null && files.Length > 0)
            {
                try
                {
                    image.Source = new BitmapImage(new Uri(files[0]));
                }
                catch (Exception)
                {
                }
            }
        }

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
