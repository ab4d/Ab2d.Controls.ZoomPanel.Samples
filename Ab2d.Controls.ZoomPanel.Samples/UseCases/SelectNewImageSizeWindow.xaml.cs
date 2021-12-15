using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Ab2d.ZoomControlSample.UseCases
{
    /// <summary>
    /// Interaction logic for SelectNewImageSizeWindow.xaml
    /// </summary>
    public partial class SelectNewImageSizeWindow : Window
    {
        private double _initialWidth;
        private double _initialHeight;

        private bool _isInitialized;

        public double ImageWidth { get; set; }
        public double ImageHeight { get; set; }

        public string TitleText
        {
            get
            {
                return TitleTextBlock.Text;
            }

            set
            {
                TitleTextBlock.Text = value;
            }
        }

        public bool ShowUseSampleGridCheckBox
        {
            get
            {
                return UseSampleGridCheckBox.Visibility == System.Windows.Visibility.Visible;
            }

            set
            {
                UseSampleGridCheckBox.Visibility = value ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            }
        }

        public bool ShowPreserveAspectRatioCheckBox
        {
            get
            {
                return PreserveAspectRatioCheckBox.Visibility == System.Windows.Visibility.Visible;
            }

            set
            {
                PreserveAspectRatioCheckBox.Visibility = value ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            }
        }
        

        public bool UseSampleGrid
        {
            get
            {
                return UseSampleGridCheckBox.IsChecked ?? false;
            }
        }

        public SelectNewImageSizeWindow()
        {
            InitializeComponent();

            ImageWidth = 400;
            ImageHeight = 600;

            this.Loaded += new RoutedEventHandler(SelectNewImageSizeWindow_Loaded);
        }

        void SelectNewImageSizeWindow_Loaded(object sender, RoutedEventArgs e)
        {
            WidthTextBox.Text = string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0:0}", ImageWidth);
            HeightTextBox.Text = string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0:0}", ImageHeight);

            _initialWidth = ImageWidth;
            _initialHeight = ImageHeight;

            _isInitialized = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            ImageWidth = 0;
            ImageHeight = 0;

            this.Close();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            ImageWidth = Convert.ToDouble(int.Parse(WidthTextBox.Text, System.Globalization.CultureInfo.InvariantCulture));
            ImageHeight = Convert.ToDouble(int.Parse(HeightTextBox.Text, System.Globalization.CultureInfo.InvariantCulture));

            this.Close();
        }


        private void WidthTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            int width;

            if (!_isInitialized)
                return;

            if (!int.TryParse(WidthTextBox.Text, out width))
            {
                WidthTextBox.Foreground = Brushes.Red;
                OkButton.IsEnabled = false;
            }
            else
            {
                WidthTextBox.Foreground = Brushes.Black;
                OkButton.IsEnabled = true;

                if (PreserveAspectRatioCheckBox.Visibility == System.Windows.Visibility.Visible && (PreserveAspectRatioCheckBox.IsChecked ?? false))
                    HeightTextBox.Text = string.Format("{0:0}", width * _initialHeight / _initialWidth);
            }
        }

        private void HeightTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            int height;

            if (!_isInitialized)
                return;

            if (!int.TryParse(HeightTextBox.Text, out height))
            {
                HeightTextBox.Foreground = Brushes.Red;
                OkButton.IsEnabled = false;
            }
            else
            {
                HeightTextBox.Foreground = Brushes.Black;
                OkButton.IsEnabled = true;

                if (PreserveAspectRatioCheckBox.Visibility == System.Windows.Visibility.Visible && (PreserveAspectRatioCheckBox.IsChecked ?? false))
                    WidthTextBox.Text = string.Format("{0:0}", height * _initialWidth / _initialHeight);
            }
        }
    }
}
