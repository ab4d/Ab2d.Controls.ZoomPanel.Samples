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

namespace Ab2d.ZoomControlSample.ZoomPanel
{
    /// <summary>
    /// Interaction logic for LimitedZooming.xaml
    /// </summary>
    public partial class LimitedZooming : Page
    {
        public LimitedZooming()
        {
            InitializeComponent();
        }

        private void ZoomPanel1_PreviewViewboxChanged(object sender, Ab2d.Controls.ViewboxChangedRoutedEventArgs e)
        {
            double zoomFactor = ZoomPanel1.GetZoomFactor(e.NewViewboxValue);

            // Limit the zoom in and out values to 10% and 500%
            // If outside this limits we will mark the change as handled and this will prevent the change in ZoomPanel
            if (zoomFactor < 0.1 || zoomFactor > 5)
            {
                // First check if we have already limited the zoom in the previous zoom event.
                // In this case just prevent on changes done while zooming (this will not prevent only move events)
                // Without this the user would still be able to move the content of ZoomPanel when zooming (it looks wrong)
                if ((zoomFactor > 5 && e.OldViewboxValue.Width == 0.2) ||
                    (zoomFactor < 0.1 && e.OldViewboxValue.Width == 10))
                {
                    e.Handled = true; // Prevent any change
                }
                else
                {
                    // Here we will adjust the NewViewboxValue.Width and NewViewboxValue.Height
                    // To preserve the location we need to store the new center
                    double centerX = e.OldViewboxValue.X + e.OldViewboxValue.Width / 2;
                    double centerY = e.OldViewboxValue.Y + e.OldViewboxValue.Height / 2;

                    double newWidth, newHeight;

                    if (zoomFactor < 0.1)
                    {
                        newWidth = 10; // 10% = 10 times the whole content is shown
                        newHeight = 10;
                    }
                    else
                    {
                        newWidth = 0.2; // 500% - 1 / 5 of the whole content is shown
                        newHeight = 0.2;
                    }

                    e.NewViewboxValue = new Rect(centerX - newWidth/2, centerY - newHeight/2, newWidth, newHeight);
                }
            }
        }
    }
}
