using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace Ab2d.ZoomControlSample.ZoomPanel
{
    /// <summary>
    /// Interaction logic for ZoomPanelSetZoomMethodSample.xaml
    /// </summary>
    public partial class ZoomPanelSetZoomMethodSample : Page
    {
        private bool _isInternalChange = false;

        private TypeConverter _rectConverter;

        public ZoomPanelSetZoomMethodSample()
        {
            InitializeComponent();

            _rectConverter = TypeDescriptor.GetConverter(typeof(Rect));

            this.Loaded += new RoutedEventHandler(ZoomPanelSetZoomMethodSample_Loaded);

            // ViewboxChanged event happens every time the zoom area is changed
            ZoomPanel1.ViewboxChanged += new Controls.ZoomPanel.ViewboxChangedRoutedEventHandler(ZoomPanel1_ViewboxChanged);
        }

        void ZoomPanelSetZoomMethodSample_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateShowValues(ZoomPanel1.Viewbox);

            //ZoomPanel1.Stretch = Stretch.None;
        }

        void ZoomPanel1_ViewboxChanged(object sender, Controls.ViewboxChangedRoutedEventArgs e)
        {
            // Because the CenterPosition and ZoomFactor values can be animated, 
            // it is not sure that their final values are already set in the ZoomPanel.
            // Therefore we use the NewViewboxValue that is passed to this event handler - 
            // it contain the final value (after the animation will be complete) of the viewbox.
            // From the viewbox we can get the CenterPosition and ZoomFactor

            UpdateShowValues(e.NewViewboxValue);
        }

        private void UpdateShowValues(Rect viewbox)
        {
            Point centerPosition;
            double zoomFactor;

            centerPosition = ZoomPanel1.GetCenterPosition(viewbox, GetSelectedUnits());
            zoomFactor = ZoomPanel1.GetZoomFactor(viewbox);


            _isInternalChange = true;

            if (GetSelectedUnits() == Controls.ZoomPanel.CenterPositionUnitsType.Absolute) // Absolute
                CenterPositionTextBox.Text = string.Format("{0:0}", centerPosition);
            else
                CenterPositionTextBox.Text = string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0:0.00} {1:0.00}", centerPosition.X, centerPosition.Y);

            ZoomFactorTextBox.Text = string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0:0.00}", zoomFactor);

            ViewboxTextBox.Text = string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0:0.00} {1:0.00} {2:0.00} {3:0.00}", viewbox.X, viewbox.Y, viewbox.Width, viewbox.Height);

            _isInternalChange = false;
        }

        private Ab2d.Controls.ZoomPanel.CenterPositionUnitsType GetSelectedUnits()
        {
            if (CenterPositionUnitsComboBox.SelectedIndex == 0)
                return Ab2d.Controls.ZoomPanel.CenterPositionUnitsType.Relative;
            else
                return Ab2d.Controls.ZoomPanel.CenterPositionUnitsType.Absolute;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!this.IsLoaded || _isInternalChange)
                return;

            UpdateShowValues(ZoomPanel1.Viewbox);
        }

        private void SetZoomButton_Click(object sender, RoutedEventArgs e)
        {
            SetZoom(false);
        }

        private void SetZoomNowButton_Click(object sender, RoutedEventArgs e)
        {
            SetZoom(true);
        }


        private void SetViewboxButton_Click(object sender, RoutedEventArgs e)
        {
            SetViewbox(false);
        }

        private void SetViewboxNowButton_Click(object sender, RoutedEventArgs e)
        {
            SetViewbox(true);
        }

        private void SetViewbox(bool immediateChange)
        {
            Rect newViewbox;

            try
            {
                newViewbox = (Rect)_rectConverter.ConvertFromInvariantString(ViewboxTextBox.Text);

                if (immediateChange)
                {
                    ZoomPanel1.SetZoomNow(newViewbox);

                    // SetViewbox method can be also used:
                    // ZoomPanel1.SetViewbox(newViewbox);
                }
                else
                {
                    ZoomPanel1.SetZoom(newViewbox);

                    // SetViewbox method or setting Viewbox property can be also used:
                    //ZoomPanel1.SetViewboxNow(newViewbox);
                    //ZoomPanel1.Viewbox = newViewbox;
                }
            }
            catch
            {
            }
        }

        private void SetZoom(bool immediateChange)
        {
            bool hasErrors;
            Point newCenterPosition;
            double newZoomFactor;
            Ab2d.Controls.ZoomPanel.CenterPositionUnitsType units;

            hasErrors = false;
            newCenterPosition = new Point(); // to make compiler happy

            try
            {
                string[] parts;

                parts = CenterPositionTextBox.Text.Split(new char [] {' ', ','});

                if (parts.Length != 2)
                {
                    hasErrors = true;
                }
                else
                {
                    double x,y;

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


            if (!double.TryParse(ZoomFactorTextBox.Text, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out newZoomFactor))
            {
                hasErrors = true;
                ZoomFactorTextBox.Foreground = Brushes.Red;
            }
            else
            {
                if (ZoomFactorTextBox.Foreground == Brushes.Red)
                    ZoomFactorTextBox.Foreground = Brushes.Black;
            }

            if (CenterPositionUnitsComboBox.SelectedIndex == 0)
                units = Controls.ZoomPanel.CenterPositionUnitsType.Relative;
            else
                units = Controls.ZoomPanel.CenterPositionUnitsType.Absolute;

            if (!hasErrors)
            {
                if (immediateChange)
                {
                    ZoomPanel1.SetZoomNow(newCenterPosition, units, newZoomFactor);

                    // The zoom can be also set by setting the properties
                    //ZoomPanel1.CenterPosition = newCenterPosition;
                    //ZoomPanel1.ZoomFactor = newZoomFactor;
                }
                else
                {
                    ZoomPanel1.SetZoom(newCenterPosition, units, newZoomFactor);
                }
            }
        }

        private void StretchComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                string stretchText;

                stretchText = (string)((ComboBoxItem)StretchComboBox.SelectedValue).Content;
                TypeConverter stretchConverter = TypeDescriptor.GetConverter(typeof(Stretch));
                ZoomPanel1.Stretch = (Stretch)stretchConverter.ConvertFromString(stretchText);
            }
            catch
            {
                // Not expected but just in case
            }
        }

        private void GridSizeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!this.IsLoaded)
                return;

            SampleGrid1.Width = double.Parse(((ComboBoxItem)GridWidthComboBox.SelectedItem).Content.ToString());
            SampleGrid1.Height = double.Parse(((ComboBoxItem)GridHeightComboBox.SelectedItem).Content.ToString());
            SampleGrid1.Recreate();
        }
    }
}
