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
    /// Interaction logic for ZoomPanelMultiMouseButtonSample.xaml
    /// </summary>
    public partial class ZoomPanelMultiMouseButtonSample : Page
    {
        private Point _lastMoveMousePosition;

        public ZoomPanelMultiMouseButtonSample()
        {
            InitializeComponent();

            this.PreviewMouseRightButtonDown += new MouseButtonEventHandler(ZoomPanelMultiMouseButtonSample_PreviewMouseRightButtonDown);
            this.PreviewMouseRightButtonUp += new MouseButtonEventHandler(ZoomPanelMultiMouseButtonSample_PreviewMouseRightButtonUp);
            this.MouseMove += new MouseEventHandler(ZoomPanelMultiMouseButtonSample_MouseMove);
        }

        void ZoomPanelMultiMouseButtonSample_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            ZoomPanel1.ZoomMode = Controls.ZoomPanel.ZoomModeType.Rectangle;
        }

        void ZoomPanelMultiMouseButtonSample_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            ZoomPanel1.ZoomMode = Controls.ZoomPanel.ZoomModeType.Move;

            _lastMoveMousePosition = e.GetPosition(ZoomPanel1);
        }

        void ZoomPanelMultiMouseButtonSample_MouseMove(object sender, MouseEventArgs e)
        {
            // Only process mouse move when we are moving the content with right mouse button
            if (ZoomPanel1.ZoomMode != Ab2d.Controls.ZoomPanel.ZoomModeType.Move || e.RightButton != MouseButtonState.Pressed)
                return;

            Point newMousePosition = e.GetPosition(ZoomPanel1);

            // Calculate dx and dy and take the current zoom ration in account
            double dx = (newMousePosition.X - _lastMoveMousePosition.X);
            double dy = (newMousePosition.Y - _lastMoveMousePosition.Y);


            // Disable animation and make the transition
            ZoomPanel1.IsAnimated = false;
            ZoomPanel1.Translate(dx, dy);
            ZoomPanel1.IsAnimated = true;


            _lastMoveMousePosition = newMousePosition;

            e.Handled = true;
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            ZoomPanel1.Reset();
        }
    }
}
