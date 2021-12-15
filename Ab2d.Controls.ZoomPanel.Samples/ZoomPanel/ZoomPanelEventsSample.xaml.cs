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
    /// Interaction logic for ZoomPanelEventsSample.xaml
    /// </summary>
    public partial class ZoomPanelEventsSample : Page
    {
        public ZoomPanelEventsSample()
        {
            InitializeComponent();

            AddMessage("Events log:\n");
        }

        private void RotateRightButton_Click(object sender, RoutedEventArgs e)
        {
            // Animate the rotation
            ZoomPanel1.SetZoom(ZoomPanel1.Viewbox, ZoomPanel1.RotationAngle - 30);

            //ZoomPanel1.RotationAngle -= 30; // this change is not animated
        }

        private void RotateLeftButton_Click(object sender, RoutedEventArgs e)
        {
            // Animate the rotation
            ZoomPanel1.SetZoom(ZoomPanel1.Viewbox, ZoomPanel1.RotationAngle + 30);

            //ZoomPanel1.RotationAngle += 30; // this change is not animated
        }

        private void ZoomPanel1_ZoomModeChanged(object sender, Ab2d.Controls.ZoomModeChangedRoutedEventArgs e)
        {
            AddMessage(string.Format("ZoomMode changed from {0} to {1}.\n", e.OldZoomMode, e.NewZoomMode));
        }

        private void ZoomPanel1_PreviewViewboxChanged(object sender, Ab2d.Controls.ViewboxChangedRoutedEventArgs e)
        {
            PreviewViewboxChangedDialog previewDialog;

            AddMessage(string.Format("Preview Viewbox changed from {0} to {1}.\n", FormatRect(e.OldViewboxValue), FormatRect(e.NewViewboxValue)));

            if (HandlePreviewCheckBox.IsChecked ?? false)
            {
                previewDialog = new PreviewViewboxChangedDialog();
                previewDialog.OldViewbox = e.OldViewboxValue;
                previewDialog.NewViewbox = e.NewViewboxValue;
                previewDialog.OldRotationAngle = e.OldRotationAngle;
                previewDialog.NewRotationAngle = e.NewRotationAngle;

                previewDialog.ShowDialog();

                e.NewViewboxValue = previewDialog.NewViewbox;
                e.NewRotationAngle = previewDialog.NewRotationAngle;

                e.Handled = !previewDialog.AllowChange;

                if (!previewDialog.AllowChange)
                    AddMessage("  Change denied!\n");
            }
        }

        private void ZoomPanel1_ViewboxChanged(object sender, Ab2d.Controls.ViewboxChangedRoutedEventArgs e)
        {
            AddMessage(string.Format("Viewbox changed from {0} to {1}.\n", FormatRect(e.OldViewboxValue), FormatRect(e.NewViewboxValue)));
        }


        private void AddMessage(string message)
        {
            InfoTextBox.Text += message;
            InfoScrollViewer.ScrollToBottom();
        }

        public static string FormatRect(Rect rect)
        {
            return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0:0.00} {1:0.00} {2:0.00} {3:0.00}", rect.X, rect.Y, rect.Width, rect.Height);
        }
    }
}
