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

        private bool processing = true;

        public MainWindow()
        {
            InitializeComponent();
            MouseLeftButtonDown += (sender, e) => { DragMove(); };

            widthTextBox.Text = Width.ToString();
            heightTextBox.Text = Height.ToString();
            UpdateSizePanels();
            processing = false;

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
                processing = true;
                image.Source = new BitmapImage(new Uri(path));
                if (unitComboBox.Text == "%")
                {
                    UpdateSizeByImage();
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                processing = false;
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

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (processing)
            {
                return;
            }
            try
            {
                processing = true;
                widthTextBox.Text = Width.ToString();
                heightTextBox.Text = Height.ToString();
            }
            finally
            {
                processing = false;
            }
        }

        private void WidthTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (processing)
            {
                return;
            }
            try
            {
                processing = true;
                if (double.TryParse(widthTextBox.Text, out var width) && width > 0)
                {
                    Width = width;
                }
            }
            finally
            {
                processing = false;
            }
        }

        private void HeightTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (processing)
            {
                return;
            }
            try
            {
                processing = true;
                if (double.TryParse(heightTextBox.Text, out var height) && height > 0)
                {
                    Height = height;
                }
            }
            finally
            {
                processing = false;
            }
        }

        private void PercentTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (processing)
            {
                return;
            }
            try
            {
                UpdateSizeByImage();
            }
            finally
            {
                processing = false;
            }
        }

        private void UpdateSizeByImage()
        {
            if (image.Source is not BitmapSource bitmap)
            {
                return;
            }
            var width = bitmap.Width;
            var height = bitmap.Height;
            if (double.TryParse(percentTextBox.Text, out var percent) && percent > 0)
            {
                width = bitmap.Width * percent / 100;
                height = bitmap.Height * percent / 100;
            }
            Width = width;
            Height = height;
            widthTextBox.Text = width.ToString();
            heightTextBox.Text = height.ToString();
        }

        private void UnitComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (processing)
            {
                return;
            }
            try
            {
                processing = true;
                UpdateSizePanels();
            }
            finally
            {
                processing = false;
            }
        }

        private void UpdateSizePanels()
        {
            var selected = (unitComboBox.SelectedItem as ComboBoxItem).Content;
            absoluteSizePanel.Visibility = selected.Equals("px") ? Visibility.Visible : Visibility.Collapsed;
            percentSizePanel.Visibility = selected.Equals("%") ? Visibility.Visible : Visibility.Collapsed;
        }

        private void SpecifySizeMenuItemMenuItem_Click(object sender, RoutedEventArgs e)
        {
            specifySizeMenuItem.IsChecked = !specifySizeMenuItem.IsChecked;
            sizePanel.Visibility = specifySizeMenuItem.IsChecked ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
