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

namespace Ab2d.ZoomControlSample.ViewboxEx
{
    /// <summary>
    /// Interaction logic for ViewboxExTester.xaml
    /// </summary>
    public partial class ViewboxExTester : Page
    {
        private TypeConverter _rectConverter;

        public ViewboxExTester()
        {
            InitializeComponent();

            //MiniMap1.MapImageElement = Viewbox1.conten
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //Viewbox1.LayoutUpdated += new EventHandler(Viewbox1_LayoutUpdated);
            Viewbox1.ActualViewboxChanged += new Ab2d.Controls.ViewboxEx.ActualViewboxChangedEventHandler(Viewbox1_ActualViewboxChanged);

            //MiniMap1.MapImageElement = Viewbox1.Child;
            
            Refresh();
        }

        private void Refresh()
        {
            // The controls are not loaded yet
            if (!this.IsLoaded) return;

            if (RelativeViewboxUnitsCheckBox.IsChecked ?? false)
                Viewbox1.ViewboxUnits = BrushMappingMode.RelativeToBoundingBox;
            else
                Viewbox1.ViewboxUnits = BrushMappingMode.Absolute;

            ContentTextBlock.Text = ContentTextBox.Text;

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
                    Viewbox1.Viewbox = newViewbox;

                    if (ViewboxTextBox.Foreground != Brushes.Black)
                        ViewboxTextBox.Foreground = Brushes.Black;
                }

                //RefreshRectangles();
            }
            catch
            {
                ViewboxTextBox.Foreground = Brushes.Red;
            }

            try
            {
                string stretchText;

                stretchText = (string)((ComboBoxItem)StretchComboBox.SelectedValue).Content;
                TypeConverter stretchConverter = TypeDescriptor.GetConverter(typeof(Stretch));
                Viewbox1.Stretch = (Stretch)stretchConverter.ConvertFromString(stretchText);
            }
            catch
            {
                // Not expected but just in case
            }

            //MiniMap1.Refresh();

            // ContentSize and ActualContentBounds info are shown in LayoutUpdated event handler - after the Viewbox is repainted the data are also updated
        }


        void Viewbox1_ActualViewboxChanged(object sender, Ab2d.Controls.ActualViewboxChangedEventArgs e)
        {
            // ContentSize and ActualContentBounds info are shown in LayoutUpdated event handler - after the Viewbox is repainted the data are also updated

            try
            {
                ContentSizeLabel.Text = string.Format("w:{0:0.00} h:{1:0.00}", Viewbox1.ContentSize.Width, Viewbox1.ContentSize.Height);
                ActualContentBoundsXYLabel.Text = string.Format("x:{0:0.00} y:{1:0.00}", Viewbox1.ActualContentBounds.X, Viewbox1.ActualContentBounds.Y);
                ActualContentBoundsWHLabel.Text = string.Format("w:{0:0.00} h:{1:0.00}", Viewbox1.ActualContentBounds.Width, Viewbox1.ActualContentBounds.Height);

                Rect actualViewbox;

                actualViewbox = Viewbox1.ActualViewbox;

                ActualViewboxXYLabel.Text = string.Format("x:{0:0.00} y:{1:0.00}", actualViewbox.X, actualViewbox.Y);
                ActualViewboxWHLabel.Text = string.Format("w:{0:0.00} h:{1:0.00}", actualViewbox.Width, actualViewbox.Height);
            }
            catch
            { }
        }

        //private void RefreshRectangles()
        //{
        //    double x, y;
        //    double width, height;
        //    Rect relativeViewbox;

        //    relativeViewbox = Viewbox1.GetRelativeViewbox();

        //    // calculate width and height (aprox. 1/4 of the containg canvas)
        //    width = RectanglesCanvas.Width / 4;
        //    height = width * Viewbox1.ActualHeight / Viewbox1.ActualWidth;

        //    // center it
        //    x = (RectanglesCanvas.Width - ContentRectangle.Width) / 2;
        //    y = (RectanglesCanvas.Height - ContentRectangle.Height) / 2;

        //    // Set values
        //    Canvas.SetLeft(ContentRectangle, x);
        //    Canvas.SetTop(ContentRectangle, y);
        //    ContentRectangle.Width = width;
        //    ContentRectangle.Height = height;


        //    Canvas.SetLeft(ViewboxRectangle, x + relativeViewbox.X * width);
        //    Canvas.SetTop(ViewboxRectangle, y + relativeViewbox.Y * height);
        //    ViewboxRectangle.Width = relativeViewbox.Width * width;
        //    ViewboxRectangle.Height = relativeViewbox.Height * height;
        //}

        private void ViewboxTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Refresh();
        }

        private void ContentTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Refresh();
        }

        private void StretchComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Refresh();
        }

        private void RelativeViewboxUnitsCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Refresh();
        }
    }
}
