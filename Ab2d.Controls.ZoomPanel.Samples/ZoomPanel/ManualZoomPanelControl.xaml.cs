using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Interaction logic for ManualZoomPanelControl.xaml
    /// </summary>
    public partial class ManualZoomPanelControl : Page
    {
        private bool _isMouseDown;
        private bool _isTranslating;

        private Point _lastMousePosition;

        public ManualZoomPanelControl()
        {
            InitializeComponent();

            // Disable all zooming and panning of ZoomPanel
            // We will do that with
            ZoomPanel1.ZoomMode = Controls.ZoomPanel.ZoomModeType.None;

            ZoomPanel1.MouseLeftButtonDown += delegate(object sender, MouseButtonEventArgs e)
            {
                // Start with storing the current mouse position
                _lastMousePosition = e.GetPosition(ZoomPanel1);
                _isMouseDown = true;
            };

            ZoomPanel1.MouseLeftButtonUp += delegate(object sender, MouseButtonEventArgs e)
            {
                if (!_isMouseDown)
                    return; // This can happen is user pressed mouse down on some other control and released the button on ZoomPanel

                _isMouseDown = false;

                if (_isTranslating)
                {
                    // If we were translating, stop translating and release mouse capture
                    _isTranslating = false;
                    ZoomPanel1.ReleaseMouseCapture();
                }
                else
                {
                    // If we were not translating then this is a Click (a MouseLeftButtonDown was hit before - _isMouseDown was true)
                    OnClick(e);
                }
            };

            ZoomPanel1.MouseMove += delegate(object sender, MouseEventArgs e)
            {
                if (e.LeftButton != MouseButtonState.Pressed) // Skip this code if mouse button is not pressed
                {
                    _isMouseDown = false;
                    return;
                }

                var currentMousePosition = e.GetPosition(ZoomPanel1);

                // Get a vector from the previous mouse position to the current mouse position
                var mouseMoveVector = currentMousePosition - _lastMousePosition;

                // IF we are not translating and the mouse has moved for more than 1 pixel,
                // then we start translating
                if (!_isTranslating && mouseMoveVector.LengthSquared > 1)
                {
                    _isTranslating = true;

                    // Capture mouse so that we get mouse events also in cases when mouse leaves our application
                    ZoomPanel1.CaptureMouse();
                }

                if (_isTranslating)
                {
                    // Call TranslateNow that translates the content of ZoomPanel for the x and y difference
                    ZoomPanel1.TranslateNow(mouseMoveVector.X, mouseMoveVector.Y);

                    // Save the current position
                    _lastMousePosition = currentMousePosition;
                }
            };

            // Manually handle Mouse Wheel zoom in and out
            ZoomPanel1.MouseWheel += delegate(object sender, MouseWheelEventArgs e)
            {
                double zoomFactor;

                if (e.Delta > 0)
                    zoomFactor = ZoomPanel1.MouseWheelZoomFactor; // Zoom in
                else
                    zoomFactor = 1 / ZoomPanel1.MouseWheelZoomFactor; // Zoom out


                // If IsZoomPositionPreserved is true then we zoom in and out into the current mouse position
                if (ZoomPanel1.IsZoomPositionPreserved)
                {
                    var currentMousePosition = e.GetPosition(ZoomPanel1);
                    ZoomPanel1.ZoomAtMousePosition(zoomFactor, currentMousePosition);
                }
                else
                {
                    // Just zoom in or out into the center of the ZoomPanel's content
                    ZoomPanel1.ZoomForFactor(zoomFactor);
                }
            };
        }

        private void OnClick(MouseEventArgs e)
        {
            var currentMousePosition = e.GetPosition(ZoomPanel1);

            MessageBox.Show(string.Format("Mouse clicked at:\r\n{0:0}", currentMousePosition));
        }
    }
}
