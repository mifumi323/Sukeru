using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MifuminSoft.Sukeru
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private Color selectedColor;

        public Color SelectedColor
        {
            get { return selectedColor; }
            set
            {
                selectedColor = value;
                Background = new SolidColorBrush(value);
            }
        }


        public MainWindow()
        {
            InitializeComponent();
            MouseLeftButtonDown += (sender, e) => { DragMove(); };

            if (App.CommandLineArgs?.Length > 0)
            {
                var path = App.CommandLineArgs[0];
                if (File.Exists(path))
                {
                    LoadImageFromFile(path);
                }
            }
        }

        private void LoadImageFromFile(string path)
        {
            try
            {
                image.Source = new BitmapImage(new Uri(path));
            }
            catch (Exception)
            {
            }
        }

        private void Image_DragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.All;
        }

        private void Image_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetData(DataFormats.FileDrop) is string[] files && files.Length > 0)
            {
                LoadImageFromFile(files[0]);
            }
        }

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ColorMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SelectedColor = (Color)((MenuItem)sender).Tag;
            foreach (MenuItem menuItem in colorMenuItem.Items)
            {
                menuItem.IsChecked = ((Color)menuItem.Tag) == SelectedColor;
            }
        }

        private void Window_ContextMenuOpening(object sender, System.Windows.Controls.ContextMenuEventArgs e)
        {
            if (colorMenuItem.Items.Count == 0)
            {
                var type = typeof(Colors);
                var properties = type.GetProperties();
                foreach (var p in properties)
                {
                    var color = (Color)p.GetValue(null, null);
                    if (color.A < 255)
                    {
                        continue;
                    }

                    var menuItem = new MenuItem()
                    {
                        Header = p.Name,
                        Icon = new Rectangle()
                        {
                            Fill = new SolidColorBrush(color),
                        },
                        Tag = color,
                        IsCheckable = true,
                        IsChecked = color == SelectedColor,
                    };
                    menuItem.Click += ColorMenuItem_Click;
                    colorMenuItem.Items.Add(menuItem);
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SelectedColor = Colors.White;
        }

        private void TopMostMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Topmost = ((MenuItem)sender).IsChecked;
        }
    }
}
