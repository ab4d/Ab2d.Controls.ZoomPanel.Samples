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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace Ab2d.ZoomControlSample.ZoomPanel
{
    /// <summary>
    /// Interaction logic for ViewboxSample.xaml
    /// </summary>
    public partial class ViewboxSample : Page
    {
        private TypeConverter _rectConverter;

        public ViewboxSample()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(ViewboxSample_Loaded);
        }

        void ViewboxSample_Loaded(object sender, RoutedEventArgs e)
        {
            ViewboxTextBox.Focus();
        }

        private void ViewboxTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (_rectConverter == null)
                    _rectConverter = TypeDescriptor.GetConverter(typeof(Rect));

                Rect newViewbox = (Rect)_rectConverter.ConvertFromInvariantString(ViewboxTextBox.Text);

                if (newViewbox.Width <= 0 || newViewbox.Height <= 0)
                {
                    // Not valid
                    ViewboxTextBox.Foreground = Brushes.Red;
                }
                else
                {
                    // Valid
                    CustomZoomPanel.Viewbox = newViewbox;

                    if (ViewboxTextBox.Foreground != Brushes.Black)
                        ViewboxTextBox.Foreground = Brushes.Black;
                }
            }
            catch
            {
                ViewboxTextBox.Foreground = Brushes.Red;
            }
        }
    }
}
