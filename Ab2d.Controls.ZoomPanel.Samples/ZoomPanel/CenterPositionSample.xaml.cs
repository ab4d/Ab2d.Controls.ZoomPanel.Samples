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

namespace Ab2d.ZoomControlSample.ZoomPanel
{
    /// <summary>
    /// Interaction logic for CenterPositionSample.xaml
    /// </summary>
    public partial class CenterPositionSample : Page
    {
        public CenterPositionSample()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(CenterPositionSample_Loaded);
        }

        void CenterPositionSample_Loaded(object sender, RoutedEventArgs e)
        {
            CenterPositionTextBox.Focus();
        }

        private void RelativeRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            CustomZoomPanel.CenterPositionUnits = Controls.ZoomPanel.CenterPositionUnitsType.Relative;
        }

        private void AbsoluteRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            CustomZoomPanel.CenterPositionUnits = Controls.ZoomPanel.CenterPositionUnitsType.Absolute;
        }

        private void CenterPositionTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            bool hasErrors = false;
            Point newCenterPosition = new Point(); // to make compiler happy

            try
            {
                string[] parts;

                parts = CenterPositionTextBox.Text.Split(new char[] { ' ', ',' });

                if (parts.Length != 2)
                {
                    hasErrors = true;
                }
                else
                {
                    double x, y;

                    if (!double.TryParse(parts[0], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out x))
                    {
                        hasErrors = true;
                        CenterPositionTextBox.Foreground = Brushes.Red;
                    }
                    else
                    {
                        if (!double.TryParse(parts[1], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out y))
                        {
                            hasErrors = true;
                            CenterPositionTextBox.Foreground = Brushes.Red;
                        }
                        else
                        {
                            newCenterPosition = new Point(x, y);

                            if (CenterPositionTextBox.Foreground == Brushes.Red)
                                CenterPositionTextBox.Foreground = Brushes.Black;
                        }
                    }
                }
            }
            catch
            {
                hasErrors = true;
                CenterPositionTextBox.Foreground = Brushes.Red;
            }

            if (!hasErrors)
                CustomZoomPanel.CenterPosition = newCenterPosition;
        }
    }
}
