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
using System.ComponentModel;
using System.IO;

namespace Ab2d.ZoomControlSample.ZoomPanel
{
    /// <summary>
    /// Interaction logic for ZoomPanelCustomContent.xaml
    /// </summary>
    public partial class ZoomPanelCustomContent : Page
    {
        // Earth by night image from NASA's Visible Earth
        // http://visibleearth.nasa.gov/view_rec.php?id=13872
        // Note: Image was converted from png into jpg to reduce its size

        private BitmapImage _originalImage;

        private const double EARTH_CIRCUMFERENCE = 40075.02; // from http://en.wikipedia.org/wiki/Earth
       
        public ZoomPanelCustomContent()
        {
            InitializeComponent();

            _originalImage = new BitmapImage();
            _originalImage.BeginInit();
            _originalImage.StreamSource = Application.GetResourceStream(new Uri(@"Resources\earthatnight-2048.jpg", UriKind.Relative)).Stream;
            _originalImage.EndInit();

            // Register the MyCustomContentProvider method as the custom content provider
            // The size of custom content is set in the km (the size of the shown map)
            // This way the absoluteCustomContentViewbox parameter to MyCustomContentProvider with be also in the same units (scale)
            ZoomPanel1.RegisterCustomContentProvider(MyCustomContentProvider, new Size(EARTH_CIRCUMFERENCE, EARTH_CIRCUMFERENCE / 2));
            //ZoomPanel1.RegisterCustomContentProvider(MyCustomContentProvider, new Size(_originalImage.PixelWidth, _originalImage.PixelHeight));
        }

        // NOTES:
        // MyCustomContentProvider provides the custom content as UIElement for the ZoomPanel - the area that will be shown
        //
        // The relativeCustomContentViewbox, absoluteCustomContentViewbox, zoomLevel and availableSize are the parameters for the shown area 
        //
        // Parameters:
        // relativeCustomContentViewbox:
        //   Rect that in relative units tells which part of the custom content needs to be shown
        //   For example (0,0,1,1) tells that the method should return the whole custom content
        //   NOTE:
        //     Because the aspect ratio of the ZoomPanel can be different that the custom content aspect ratio,
        //     it is possible that the relativeCustomContentViewbox would have x or y negative and width or height bigger than 1.
        //     This means that there should be some blank space returned.
        //     For example:
        //         ZoomPanel size: width = 200, height = 300
        //         CustomContent size: width = 2000, height = 1000
        //         To show the whole custom content in 200 x 300 area,
        //         the original 2000 x 1000 image will be shown in the center as 200 x 100 image.
        //         Because the returned image should be 200 x 300 in size, the returned image should have empty space above and below the centered image.
        //
        // absoluteCustomContentViewbox:
        //   Same as relativeCustomContentViewbox, but in absolute coordinates (set with RegisterCustomContentProvider)
        //      For example relative point (1,1) is in absolute units the same as (customContentSize.Width, customContentSite.Height)
        //
        // zoomFactor:
        //    ZoomFactor that is used to scale the content. It is the same as the ZoomFactor in the ZoomPanel. Its value calculated by the following formula:
        //    zoomFactor = 2 / (viewbox.Width + viewbox.Height)
        //
        //    The calculation can be changed to override the GetZoomFactor method in a derived class.
        //    NOTE: In version before version 3, the zoomFactor (at that time called zoomLevel) was get with the following formula:
        //       ZoomLevel is calculated from 1 / (relativeCustomContentViewbox.Width * relativeCustomContentViewbox.Height)
        //
        // availableSize:
        //   The available size in ZoomPanel that is available for the custom content. 
        //   In other words the relativeCustomContentViewbox / absoluteCustomContentViewbox will be shown in the availableSize area.
        //   If the returned UIElement is bigger or smaller than the availableSize, it will be scaled to full the availableSize.
        private UIElement MyCustomContentProvider(Rect relativeCustomContentViewbox, Rect absoluteCustomContentViewbox, double zoomFactor, Size availableSize)
        {
            Grid rootGrid;


            rootGrid = new Grid();
            rootGrid.Width = availableSize.Width;
            rootGrid.Height = availableSize.Height;

            ImageBrush imageBrush;

            imageBrush = new ImageBrush();
            imageBrush.ImageSource = _originalImage;
            imageBrush.Viewbox = relativeCustomContentViewbox;
            imageBrush.ViewboxUnits = BrushMappingMode.RelativeToBoundingBox;

            Rectangle imageRectangle;
            imageRectangle = new Rectangle();
            imageRectangle.Fill = imageBrush;

            rootGrid.Children.Add(imageRectangle);


            //CroppedBitmap croppedBitmap;
            //croppedBitmap = new CroppedBitmap();
            //croppedBitmap.BeginInit();
            //croppedBitmap.Source = _originalImage;
            //croppedBitmap.SourceRect = new Int32Rect(Convert.ToInt32(_originalImage.PixelWidth * viewbox.X),
            //                                         Convert.ToInt32(_originalImage.PixelHeight * viewbox.Y),
            //                                         Convert.ToInt32(_originalImage.PixelWidth * viewbox.Width),
            //                                         Convert.ToInt32(_originalImage.PixelHeight * viewbox.Height));
            //croppedBitmap.EndInit();

            //Image croppedImage;
            //croppedImage = new Image();
            //croppedImage.Width = contentSize.Width;
            //croppedImage.Height = contentSize.Height;
            //croppedImage.Source = croppedBitmap;

            //rootGrid.Children.Add(croppedImage);

            Rectangle rectangle;

            rectangle = new Rectangle();
            rectangle.Fill = new SolidColorBrush(Color.FromArgb(70, 70, 70, 70));
            rectangle.Height = 20;
            rectangle.VerticalAlignment = VerticalAlignment.Bottom;
            rectangle.Margin = new Thickness(0, 0, 0, 5);

            rootGrid.Children.Add(rectangle);

            double shownWidth, shownHeight;

            shownWidth = relativeCustomContentViewbox.Width > 1 ? 1 : relativeCustomContentViewbox.Width;
            shownHeight = relativeCustomContentViewbox.Height > 1 ? 1 : relativeCustomContentViewbox.Height;

            TextBlock infoTextBlock;
            infoTextBlock = new TextBlock();
            infoTextBlock.Foreground = Brushes.White;
            infoTextBlock.FontWeight = FontWeights.Bold;
            infoTextBlock.Margin = new Thickness(10, 8, 10, 8);
            infoTextBlock.VerticalAlignment = VerticalAlignment.Bottom;
            infoTextBlock.HorizontalAlignment = HorizontalAlignment.Left;
            infoTextBlock.Text = string.Format("Shown area: {0:#,##0}km x {1:#,##00}km", absoluteCustomContentViewbox.Width, absoluteCustomContentViewbox.Height);

            rootGrid.Children.Add(infoTextBlock);


            infoTextBlock = new TextBlock();
            infoTextBlock.Foreground = Brushes.White;
            infoTextBlock.Margin = new Thickness(10, 8, 10, 8);
            infoTextBlock.VerticalAlignment = VerticalAlignment.Bottom;
            infoTextBlock.HorizontalAlignment = HorizontalAlignment.Right;
            infoTextBlock.Text = "Image source: NASA's Visible Earth";

            rootGrid.Children.Add(infoTextBlock);


            // NOTE:
            // InfoTextBlock is not send as custom content
            Rect relativeVisibleArea;
            Rect absoluteVisibleArea;

            // Gets the whole visible area - the area that would be shown if the Viewbox would be "0 0 1 1"
            // This value is different from the custom content size because of the different aspect ration of ZoomPanel and custom content
            ZoomPanel1.CalculateVisibleArea(out relativeVisibleArea, out absoluteVisibleArea);

            InfoTextBlock.Text = string.Format("relativeCustomContentViewbox: {0}\nabsoluteCustomContentViewbox: {1}\nzoomFactor: {2:0.00}\ncontentSize: {3}",
                Dump(relativeCustomContentViewbox),
                DumpInt(absoluteCustomContentViewbox),
                zoomFactor,
                DumpInt(availableSize));


            // Uncomment to save the image that is send to the ZoomPanel to disk
            //BitmapSource bitmap;

            //rootGrid.Arrange(new Rect(0, 0, availableSize.Width, availableSize.Height));
            //bitmap = RenderImage(rootGrid, availableSize.Width, availableSize.Height);
            //SaveResultImage(bitmap, @"c:\temp\ZoomContent.png");

            return rootGrid;
        }


        #region Helper methods
        private string Dump(Rect rect)
        {
            return string.Format("x:{0:0.00} y:{1:0.00} w:{2:0.00} h:{3:0.00}", rect.X, rect.Y, rect.Width, rect.Height);
        }

        private string Dump(Size size)
        {
            return string.Format("w:{0:0.00} h:{1:0.00}", size.Width, size.Height);
        }

        private string DumpInt(Rect rect)
        {
            return string.Format("x:{0:0} y:{1:0} w:{2:0} h:{3:0}", rect.X, rect.Y, rect.Width, rect.Height);
        }

        private string DumpInt(Size size)
        {
            return string.Format("w:{0:0} h:{1:0}", size.Width, size.Height);
        }

        public static void SaveResultImage(BitmapSource image, string resulImageFileName)
        {
            // write the bitmap to a file
            using (FileStream fs = new FileStream(resulImageFileName, FileMode.Create))
            {
                //JpegBitmapEncoder enc = new JpegBitmapEncoder();
                PngBitmapEncoder enc = new PngBitmapEncoder();
                BitmapFrame bitmapImage = BitmapFrame.Create(image);
                enc.Frames.Add(bitmapImage);
                enc.Save(fs);
            }
        }

        public static BitmapSource RenderImage(System.Windows.Media.Visual objectToRender, double width, double height)
        {
            return RenderImage(objectToRender, Convert.ToInt32(width), Convert.ToInt32(height));
        }

        public static BitmapSource RenderImage(System.Windows.Media.Visual objectToRender, int width, int height)
        {
            Rectangle backgroundRect = new Rectangle();
            backgroundRect.Width = width;
            backgroundRect.Height = height;
            backgroundRect.Fill = Brushes.Red;
            backgroundRect.Arrange(new Rect(0, 0, width, backgroundRect.Height));

            RenderTargetBitmap bmp = new RenderTargetBitmap(width, height, 96, 96 /* standard WPF dpi setting 96 */, PixelFormats.Pbgra32);
            bmp.Render(backgroundRect);
            bmp.Render(objectToRender);

            return bmp;
        }
        #endregion
    }
}
