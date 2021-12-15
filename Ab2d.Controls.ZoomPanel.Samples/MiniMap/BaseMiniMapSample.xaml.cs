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

namespace Ab2d.ZoomControlSample.MiniMap
{
    /// <summary>
    /// Interaction logic for BaseMiniMapSample.xaml
    /// </summary>
    public partial class BaseMiniMapSample : Page
    {
        private TypeConverter _rectConverter;
        private Brush _viewboxTextBoxBorderBrush;

        private bool _isChangedInternally;

        public BaseMiniMapSample()
        {
            InitializeComponent();

            MiniMap1.ViewboxChanged += new Ab2d.Controls.BaseMiniMap.ViewboxChangedRoutedEventHandler(MiniMap1_ViewboxChanged);
            MiniMap1.PreviewViewboxChanged += new Ab2d.Controls.BaseMiniMap.ViewboxChangedRoutedEventHandler(MiniMap1_PreviewViewboxChanged);

            this.Loaded += new RoutedEventHandler(BaseMiniMapSample_Loaded);
            this.SizeChanged += new SizeChangedEventHandler(BaseMiniMapSample_SizeChanged);

            PreviewCanvas.SizeChanged += new SizeChangedEventHandler(PreviewCanvas_SizeChanged);
                        
        }

        void PreviewCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            
        }

        void BaseMiniMapSample_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            PreviewCanvas.Width = PreviewBorder.ActualWidth - 4;
            PreviewCanvas.Height = PreviewBorder.ActualHeight - 4;
        }

        void BaseMiniMapSample_Loaded(object sender, RoutedEventArgs e)
        {
            _viewboxTextBoxBorderBrush = ViewboxTextBox.BorderBrush;

            Update();
        }

        void MiniMap1_ViewboxChanged(object sender, Ab2d.Controls.ViewboxChangedRoutedEventArgs e)
        {
            _isChangedInternally = true;

            ViewboxTextBox.Text = string.Format("{0:0.0}", e.NewViewboxValue);

            // add new Viewbox value to ViewboxChangedTextBlock
            ViewboxChangedTextBlock.Text = string.Format("{0:0.00}", e.NewViewboxValue) + Environment.NewLine + ViewboxChangedTextBlock.Text;

            _isChangedInternally = false;
        }

        void MiniMap1_PreviewViewboxChanged(object sender, Ab2d.Controls.ViewboxChangedRoutedEventArgs e)
        {
            if (PreventViewboxEnabledCheckbox.IsChecked ?? false)
            {
                ViewboxChangedTextBlock.Text = "Change prevented" + Environment.NewLine + ViewboxChangedTextBlock.Text;

                e.Handled = true; // with setting Handled to true the change is prevented
            }

            // Here we could also change the e.NewViewboxValue to some other value
        }

        private void ImageCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!this.IsLoaded)
                return;

            Update();
        }

        private void RectangleStrokeCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!this.IsLoaded)
                return;

            switch (RectangleStrokeCombobox.SelectedIndex)
            { 
                case 0:
                    MiniMap1.RectangleStroke = Brushes.Red;
                    break;

                case 1:
                    MiniMap1.RectangleStroke = Brushes.Blue;
                    break;

                case 2:
                    MiniMap1.RectangleStroke = Brushes.Black;
                    break;
            }
        }

        private void RectangleStrokeThicknessCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!this.IsLoaded)
                return;

            switch (RectangleStrokeThicknessCombobox.SelectedIndex)
            {
                case 0:
                    MiniMap1.RectangleStrokeThickness = 0;
                    break;

                case 1:
                    MiniMap1.RectangleStrokeThickness = 1;
                    break;

                case 2:
                    MiniMap1.RectangleStrokeThickness = 4;
                    break;
            }
        }

        private void MaskBrushCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!this.IsLoaded)
                return;

            switch (MaskBrushCombobox.SelectedIndex)
            {
                case 0:
                    MiniMap1.MaskBrush = Brushes.Transparent; // Transparent (none)
                    break;

                case 1:
                    MiniMap1.MaskBrush = new SolidColorBrush(Color.FromArgb(5 * 16 + 5, 0, 0, 0)); // #55000000 (darken)
                    break;

                case 2:
                    MiniMap1.MaskBrush = new SolidColorBrush(Color.FromArgb(2 * 16 + 2, 255, 255, 255)); // #22FFFFFF (lighten)
                    break;

                case 3:
                    MiniMap1.MaskBrush = new SolidColorBrush(Color.FromArgb(5 * 16 + 5, 255, 0, 0)); // #55FF0000 (red)
                    break;
            }
        }

        private void ViewboxTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isChangedInternally)
                return;

            UpdateViewbox();
        }

        private void Update()
        {
            Visual mapVisual = null;

            PreviewCanvas.Children.Clear();

            switch (ImageCombobox.SelectedIndex)
            { 
                case 0:
                    AddSampleButtons();
                    mapVisual = PreviewCanvas;
                    break;

                case 1:
                    AddSampleButtons();

                    PreviewCanvas.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    PreviewCanvas.Arrange(new Rect(0,0,PreviewCanvas.ActualWidth, PreviewCanvas.ActualHeight));

                    RenderTargetBitmap renderBitmap;

                    renderBitmap = new RenderTargetBitmap((int)PreviewCanvas.ActualWidth, (int)PreviewCanvas.ActualHeight, 96, 96 /* standard WPF dpi setting 96 */, PixelFormats.Pbgra32);
                    renderBitmap.Render(PreviewCanvas);

                    Image renderImage = new Image();
                    renderImage.Source = renderBitmap;

                    mapVisual = renderImage;
                    break;

                case 2:
                    Image image;
                    BitmapImage bitmapImage;

                    bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = Application.GetResourceStream(new Uri(@"Resources\earthatnight-2048.jpg", UriKind.Relative)).Stream;
                    bitmapImage.EndInit();

                    image = new Image();
                    image.Width = PreviewCanvas.ActualWidth;
                    image.Source = bitmapImage;

                    PreviewCanvas.Children.Add(image);
                    mapVisual = PreviewCanvas;

                    break;
            }

            MiniMap1.MapImageElement = mapVisual;
        }

        private void AddSampleButtons()
        {
            Random rnd;
            Button oneButton;

            rnd = new Random();

            for (int i = 0; i < 20; i++)
            {
                oneButton = new Button();
                oneButton.FontSize = 20;
                oneButton.Content = "Button " + i.ToString();
                oneButton.MouseEnter += new MouseEventHandler(oneButton_MouseEnter);
                oneButton.MouseLeave += new MouseEventHandler(oneButton_MouseLeave);

                Canvas.SetLeft(oneButton, rnd.Next(Convert.ToInt32(PreviewCanvas.Width - 100)));
                Canvas.SetTop(oneButton, rnd.Next(Convert.ToInt32(PreviewCanvas.Height - 40)));

                PreviewCanvas.Children.Add(oneButton);
            }
        }

        private Brush _savedBrush;

        void oneButton_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Button)sender).Background = _savedBrush;
        }

        void oneButton_MouseEnter(object sender, MouseEventArgs e)
        {
            _savedBrush = ((Button)sender).Background;
            ((Button)sender).Background = Brushes.Blue;
        }

        private BitmapSource RenderElement(FrameworkElement element, bool resize)
        {
            RenderTargetBitmap renderBitmap;

            int width, height;
            double aspect;

            if (resize)
            {
                aspect = element.ActualWidth / element.ActualHeight;

                if (aspect > 1)
                {
                    width = Convert.ToInt32(MiniMap1.Width);
                    height = Convert.ToInt32(MiniMap1.Width / aspect);
                }
                else
                {
                    width = Convert.ToInt32(MiniMap1.Width * aspect);
                    height = Convert.ToInt32(MiniMap1.Height);
                }
            }
            else
            {
                width = Convert.ToInt32(element.ActualWidth);
                height = Convert.ToInt32(element.ActualHeight);
            }


            renderBitmap = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);

            DrawingVisual visual = new DrawingVisual();
            using (DrawingContext context = visual.RenderOpen())
            {
                VisualBrush brush = new VisualBrush(element);
                context.DrawRectangle(brush,
                                      null,
                                      new Rect(new Point(), new Size(element.ActualWidth, element.ActualHeight)));
            }

            visual.Transform = new ScaleTransform(width / element.ActualWidth, height / element.ActualHeight);

            renderBitmap.Render(visual);

            //BitmapEncoder encoder = new TiffBitmapEncoder();
            //encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
            //encoder.Save(fs);
            //fs.Close();


            //renderBitmap = new RenderTargetBitmap(width, height, 96, 96 /* standard WPF dpi setting 96 */, PixelFormats.Pbgra32);
            //renderBitmap.Render(OptionsStackPanel);

            return renderBitmap;
        }

        private BitmapSource RenderElement2(FrameworkElement element, bool resize)
        {
            RenderTargetBitmap renderBitmap;

            int width, height;
            double aspect;

            if (resize)
            {
                aspect = element.ActualWidth / element.ActualHeight;

                if (aspect > 1)
                {
                    width = Convert.ToInt32(MiniMap1.Width);
                    height = Convert.ToInt32(MiniMap1.Width / aspect);
                }
                else
                {
                    width = Convert.ToInt32(MiniMap1.Width * aspect);
                    height = Convert.ToInt32(MiniMap1.Height);
                }
            }
            else
            {
                width = Convert.ToInt32(element.ActualWidth);
                height = Convert.ToInt32(element.ActualHeight);
            }

            renderBitmap = new RenderTargetBitmap(width, height, 96, 96 /* standard WPF dpi setting 96 */, PixelFormats.Pbgra32);
            renderBitmap.Render(OptionsStackPanel);

            return renderBitmap;
        }

        private void UpdateViewbox()
        {
            if (!this.IsLoaded)
                return;

            try
            {
                if (_rectConverter == null)
                    _rectConverter = TypeDescriptor.GetConverter(typeof(Rect));

                MiniMap1.Viewbox = (Rect)_rectConverter.ConvertFromInvariantString(ViewboxTextBox.Text);

                if (ViewboxTextBox.BorderBrush != _viewboxTextBoxBorderBrush)
                    ViewboxTextBox.BorderBrush = _viewboxTextBoxBorderBrush;
            }
            catch
            {
                ViewboxTextBox.BorderBrush = new SolidColorBrush(Colors.Red);
            }
        }
    }
}
