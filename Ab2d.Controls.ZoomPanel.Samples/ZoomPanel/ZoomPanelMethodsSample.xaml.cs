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
using Ab2d.Controls;

namespace Ab2d.ZoomControlSample.ZoomPanel
{
    /// <summary>
    /// Interaction logic for ZoomPanelMethodsSample.xaml
    /// </summary>
    public partial class ZoomPanelMethodsSample : Page
    {
        public ZoomPanelMethodsSample()
        {
            InitializeComponent();

            ZoomPanel1.ViewboxChanged += new Ab2d.Controls.ZoomPanel.ViewboxChangedRoutedEventHandler(ZoomPanel1_ViewboxChanged);
        }

        void ZoomPanel1_ViewboxChanged(object sender, ViewboxChangedRoutedEventArgs e)
        {
            InfoTextBlock.Text = string.Format(System.Globalization.CultureInfo.InvariantCulture, "Viewbox: {0:0.00} {1:0.00} {2:0.00} {3:0.00}", e.NewViewboxValue.X, e.NewViewboxValue.Y, e.NewViewboxValue.Width, e.NewViewboxValue.Height);
        }

        private void ZoomToFactorButton_Click(object sender, RoutedEventArgs e)
        {
            ZoomPanel1.ZoomToFactor(1.5);
        }

        private void ZoomForFactorButton_Click(object sender, RoutedEventArgs e)
        {
            ZoomPanel1.ZoomForFactor(1.5);
        }

        private void SetViewboxButton_Click(object sender, RoutedEventArgs e)
        {
            ZoomPanel1.SetViewbox(new Rect(0.2, 0.2, 0.6, 0.6));
            // same as: ZoomPanel1.Viewbox = new Rect(0.2, 0.2, 0.6, 0.6);
        }

        private void SetViewboxNowButton_Click(object sender, RoutedEventArgs e)
        {
            ZoomPanel1.SetViewboxNow(new Rect(0.2, 0.2, 0.6, 0.6));
            // same as: ZoomPanel1.Viewbox = new Rect(0.2, 0.2, 0.6, 0.6);
        }
        
        private void TranslateButton_Click(object sender, RoutedEventArgs e)
        {
            ZoomPanel1.Translate(50, 20);
        }

        private void TranslateRelativeButton_Click(object sender, RoutedEventArgs e)
        {
            ZoomPanel1.TranslateRelative(-0.2, -0.1);
        }

        private void TranslateToCenterButton_Click(object sender, RoutedEventArgs e)
        {
            ZoomPanel1.TranslateToCenter(new Point(ZoomPanel1.ActualWidth * 0.7, ZoomPanel1.ActualHeight * 0.2));
        }

        private void TranslateToCenterRelativeButton_Click(object sender, RoutedEventArgs e)
        {
            ZoomPanel1.TranslateToCenterRelative(new Point(0.7, 0.2));
        }

        private void ZoomForRectangleButton_Click(object sender, RoutedEventArgs e)
        {
            ZoomPanel1.ZoomForRectangle(new Rect(ZoomPanel1.ActualWidth * 0.4, ZoomPanel1.ActualHeight * 0.2, ZoomPanel1.ActualWidth * 0.4, ZoomPanel1.ActualHeight * 0.4));
        }

        private void ZoomForRectangle2Button_Click(object sender, RoutedEventArgs e)
        {
            // Should support points in any order (for example for dragging the rectangle to the left and up)
            ZoomPanel1.ZoomForRectangle(new Point(ZoomPanel1.ActualWidth * 0.8, ZoomPanel1.ActualHeight * 0.6), new Point(ZoomPanel1.ActualWidth * 0.4, ZoomPanel1.ActualHeight * 0.2));
        }

        private void ZoomForRectangleRelativeButton_Click(object sender, RoutedEventArgs e)
        {
            ZoomPanel1.ZoomForRectangleRelative(new Rect(0.4, 0.2, 0.4, 0.4));
        }

        private void ZoomAndTranslateToCenterButton_Click(object sender, RoutedEventArgs e)
        {
            ZoomPanel1.ZoomAndTranslateToCenter(1.5, new Point(ZoomPanel1.ActualWidth * 0.3, ZoomPanel1.ActualHeight * 0.5));
        }

        private void ZoomAndTranslateToCenterRelativeButton_Click(object sender, RoutedEventArgs e)
        {
            ZoomPanel1.ZoomAndTranslateToCenterRelative(1.5, new Point(0.3, 0.5));
        }

        private void ZoomAtMousePositionButton_Click(object sender, RoutedEventArgs e)
        {
            ZoomPanel1.ZoomAtMousePosition(1.5, new Point(ZoomPanel1.ActualWidth * 0.25, ZoomPanel1.ActualHeight * 0.25));
        }

        private void ZoomAtRelativeMousePositionButton_Click(object sender, RoutedEventArgs e)
        {
            ZoomPanel1.ZoomAtRelativeMousePosition(1.5, new Point(0.25, 0.25));
        }
        
        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            ZoomPanel1.Reset();
        }

        private void ResetNowButton_Click(object sender, RoutedEventArgs e)
        {
            ZoomPanel1.ResetNow();
        }

        private void ResetToLimitsButton_Click(object sender, RoutedEventArgs e)
        {
            ZoomPanel1.ResetToLimits();
        }

        private void ResetToLimitsNowButton_Click(object sender, RoutedEventArgs e)
        {
            ZoomPanel1.ResetToLimitsNow();
        }
        
        private void FitToWidthButton_Click(object sender, RoutedEventArgs e)
        {
            ZoomPanel1.FitToWidth();
        }
        
        private void FitToHeightButton_Click(object sender, RoutedEventArgs e)
        {
            ZoomPanel1.FitToHeight();
        }
        
        private void FitToLimitsWidthButton_Click(object sender, RoutedEventArgs e)
        {
            ZoomPanel1.FitToLimitsWidth();
        }
        
        private void FitToLimitsHeightButton_Click(object sender, RoutedEventArgs e)
        {
            ZoomPanel1.FitToLimitsHeight();
        }

        private void MouseWheelDownButton_Click(object sender, RoutedEventArgs e)
        {
            ZoomPanel1.MouseWheelDown();
        }

        private void MouseWheelLeftButton_Click(object sender, RoutedEventArgs e)
        {
            ZoomPanel1.MouseWheelLeft();
        }

        private void MouseWheelRightButton_Click(object sender, RoutedEventArgs e)
        {
            ZoomPanel1.MouseWheelRight();
        }

        private void MouseWheelUpButton_Click(object sender, RoutedEventArgs e)
        {
            ZoomPanel1.MouseWheelUp();
        }

        private void LineDownButton_Click(object sender, RoutedEventArgs e)
        {
            ZoomPanel1.LineDown();
        }

        private void LineLeftButton_Click(object sender, RoutedEventArgs e)
        {
            ZoomPanel1.LineLeft();
        }

        private void LineRightButton_Click(object sender, RoutedEventArgs e)
        {
            ZoomPanel1.LineRight();
        }

        private void LineUpButton_Click(object sender, RoutedEventArgs e)
        {
            ZoomPanel1.LineUp();
        }

        private void PageDownButton_Click(object sender, RoutedEventArgs e)
        {
            ZoomPanel1.PageDown();
        }

        private void PageLeftButton_Click(object sender, RoutedEventArgs e)
        {
            ZoomPanel1.PageLeft();
        }

        private void PageRightButton_Click(object sender, RoutedEventArgs e)
        {
            ZoomPanel1.PageRight();
        }

        private void PageUpButton_Click(object sender, RoutedEventArgs e)
        {
            ZoomPanel1.PageUp();
        }

        private void SetHorizontalOffsetButton_Click(object sender, RoutedEventArgs e)
        {
            ZoomPanel1.SetHorizontalOffset(0.2);
        }

        private void SetVerticalOffsetButton_Click(object sender, RoutedEventArgs e)
        {
            ZoomPanel1.SetVerticalOffset(0.2);
        }
    }
}
