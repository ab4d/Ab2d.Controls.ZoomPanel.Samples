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
using System.IO;
using System.ComponentModel;
using System.Xml;

namespace Ab2d.ZoomControlSample.UseCases
{
    /// <summary>
    /// Interaction logic for PainterSample.xaml
    /// </summary>
    public partial class PainterSample : Page
    {
        private PointCollection _currentPolylinePoints;
        private bool _isDrawing;

        private TypeConverter _stringToBrushConverter;

        private PainterSettings _settings;
        private bool _isSpaceKeyDown;

        private bool _isZoomModeChanged;
        private Ab2d.Controls.ZoomPanel.ZoomModeType _savedZoomMode;



        public PainterSample()
        {
            InitializeComponent();


            this.CommandBindings.Add(new CommandBinding(PainterSettings.SetStrokeThicknessCommand, SetStrokeThicknessCommandExecuted));
            this.CommandBindings.Add(new CommandBinding(PainterSettings.SetStrokeColorCommand, SetStrokeColorCommandExecuted));

            _settings = new PainterSettings();
            _settings.CurrentStrokeBrush = Brushes.Red;
            _settings.CurrentStrokeThickness = 10;

            ColorsStackPanel.DataContext = _settings;
            ThicknessStackPanel.DataContext = _settings;


            ZoomPanel1.ViewboxChanged += ZoomPanel1_ViewboxChanged;
            ZoomPanel1.PreviewMouseWheel += new MouseWheelEventHandler(ZoomPanel1_PreviewMouseWheel);

            // Using Preview events to handle the space before it can be used to click the currently focused control (for example button in ZoomController)
            this.PreviewKeyDown += new KeyEventHandler(PainterSample_PreviewKeyDown);
            this.PreviewKeyUp += new KeyEventHandler(PainterSample_PreviewKeyUp);

            this.Loaded += new RoutedEventHandler(PainterSample_Loaded);
        }

        void PainterSample_Loaded(object sender, RoutedEventArgs e)
        {
            // Set the content to be a litte bit smaller than the height of the ZoomPanel
            SetNewContent(null, new Size(ZoomPanel1.ActualHeight * 0.7, ZoomPanel1.ActualHeight - 30));
        }

        void ZoomPanel1_ViewboxChanged(object sender, Controls.ViewboxChangedRoutedEventArgs e)
        {
            double newZoomFactor = ZoomPanel1.GetZoomFactor(e.NewViewboxValue);

            ZoomValueTextBlock.Text = string.Format("{0:0}%", newZoomFactor * 100);         
        }

        void PainterSample_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            ProcessKeyEvent(e);
        }
        
        void PainterSample_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            ProcessKeyEvent(e);
        }

        void ZoomPanel1_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            double zoomFactor;

            // Handle the mouse wheel when the ZoomPanel is not active
            if (ZoomPanel1.ZoomMode == Ab2d.Controls.ZoomPanel.ZoomModeType.None)
            {
                if (e.Delta > 0)
                    zoomFactor = ZoomPanel1.MouseWheelZoomFactor;
                else
                    zoomFactor = 1 / ZoomPanel1.MouseWheelZoomFactor;

                if (ZoomPanel1.IsZoomPositionPreserved)
                    ZoomPanel1.ZoomAtMousePosition(zoomFactor, e.GetPosition(ZoomPanel1));
                else
                    ZoomPanel1.ZoomForFactor(zoomFactor);
            }
        }

        private void ProcessKeyEvent(KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                // First handle space
                _isSpaceKeyDown = e.IsDown;

                if (UpdateZoomMode())
                    e.Handled = true;
            }
            else if (_isSpaceKeyDown)
            {
                // While the space is down, the Control os Shift can be pressed or released - check this
                if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl ||
                    e.Key == Key.LeftShift || e.Key == Key.RightShift)
                {
                    if (UpdateZoomMode())
                        e.Handled = true;
                }
            }
            else
            {
                // Handle other keys
                switch (e.Key)
                {
                    case Key.Left:
                        if (e.IsDown || e.IsRepeat) // On key up only set handled to true
                            ZoomPanel1.LineLeft(); // As using the scroll bar

                        e.Handled = true;

                        break;

                    case Key.Right: 
                        if (e.IsDown || e.IsRepeat)
                            ZoomPanel1.LineRight(); // As using the scroll bar

                        e.Handled = true;

                        break;

                    case Key.Up:
                        if (e.IsDown || e.IsRepeat)
                            ZoomPanel1.LineUp(); // As using the scroll bar

                        e.Handled = true;

                        break;

                    case Key.Down:
                        if (e.IsDown || e.IsRepeat)
                            ZoomPanel1.LineDown(); // As using the scroll bar

                        e.Handled = true;

                        break;

                    case Key.Home:
                        if (e.IsDown || e.IsRepeat)
                            ZoomPanel1.ResetToLimits();

                        e.Handled = true;

                        break;

                    case Key.Insert: // ZoomOut
                    case Key.Subtract: // ZoomOut
                        if (e.IsDown || e.IsRepeat)
                            ZoomPanel1.ZoomForFactor(1 / ZoomPanel1.MouseWheelZoomFactor); // ZoomPanel1.MouseWheelZoomFactor == Zoom In factor

                        e.Handled = true;

                        break;

                    case Key.Delete: // ZoomIn
                    case Key.Add: // ZoomIn
                        if (e.IsDown || e.IsRepeat)
                            ZoomPanel1.ZoomForFactor(ZoomPanel1.MouseWheelZoomFactor);

                        e.Handled = true;
                        break;
                }
            }
        }

        private bool UpdateZoomMode()
        {
            bool isHandled;

            isHandled = false;

            if (_isSpaceKeyDown)
            {
                Ab2d.Controls.ZoomPanel.ZoomModeType newZoomMode;

                if (Keyboard.Modifiers == ModifierKeys.None)
                {
                    newZoomMode = Ab2d.Controls.ZoomPanel.ZoomModeType.Move;
                }
                else if (Keyboard.Modifiers == ModifierKeys.Control)
                {
                    newZoomMode = Ab2d.Controls.ZoomPanel.ZoomModeType.ZoomIn;
                }
                else if (Keyboard.Modifiers == ModifierKeys.Shift)
                {
                    newZoomMode = Ab2d.Controls.ZoomPanel.ZoomModeType.ZoomOut;
                }
                else
                {
                    newZoomMode = Ab2d.Controls.ZoomPanel.ZoomModeType.None;
                }


                if (newZoomMode != Ab2d.Controls.ZoomPanel.ZoomModeType.None)
                {
                    ChanageZoomMode(newZoomMode);
                    isHandled = true;
                }
            }
            else
            {
                ResetZoomMode();
                isHandled = true;
            }

            return isHandled;
        }

        private void ChanageZoomMode(Ab2d.Controls.ZoomPanel.ZoomModeType newZoomMode)
        {
            if (ZoomPanel1.ZoomMode != newZoomMode && newZoomMode != Ab2d.Controls.ZoomPanel.ZoomModeType.None)
            {
                if (!_isZoomModeChanged) // if zoom mode was already changed (only modifier - control or shift - was pressed)
                    _savedZoomMode = ZoomPanel1.ZoomMode;

                ZoomPanel1.ZoomMode = newZoomMode;
                _isZoomModeChanged = true;
            }
        }

        private void ResetZoomMode()
        {
            if (_isZoomModeChanged)
            {
                ZoomPanel1.ZoomMode = _savedZoomMode;
                _isZoomModeChanged = false;
            }
        }

        void SetStrokeThicknessCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            _settings.CurrentStrokeThickness = double.Parse((string)e.Parameter);

            // If we were using zoom, now change ZoomMode to None it enable drawing again
            ZoomPanel1.ZoomMode = Ab2d.Controls.ZoomPanel.ZoomModeType.None;
        }

        void SetStrokeColorCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (_stringToBrushConverter == null)
                _stringToBrushConverter = System.ComponentModel.TypeDescriptor.GetConverter(typeof(Brush));

            _settings.CurrentStrokeBrush = (Brush)_stringToBrushConverter.ConvertFromInvariantString((string)e.Parameter);

            // If we were using zoom, now change ZoomMode to None it enable drawing again
            ZoomPanel1.ZoomMode = Ab2d.Controls.ZoomPanel.ZoomModeType.None;
        }


        private void CreateNewButton_Click(object sender, RoutedEventArgs e)
        {
            SelectNewImageSizeWindow selectNewImageSizeWindow = new SelectNewImageSizeWindow()
            {
                ImageWidth = PaintCanvas.ActualWidth,
                ImageHeight = PaintCanvas.ActualHeight,
                TitleText = "Select new image size:",
                ShowUseSampleGridCheckBox = true,
                ShowPreserveAspectRatioCheckBox = false,
                Owner = Application.Current.MainWindow
            };

            selectNewImageSizeWindow.ShowDialog();


            if (selectNewImageSizeWindow.ImageWidth <= 0 || selectNewImageSizeWindow.ImageHeight <= 0)
                return; // Ok was not pressed

            Ab2d.ZoomControlSample.Common.SampleGrid sampleGrid;

            if (selectNewImageSizeWindow.UseSampleGrid)
            {
                sampleGrid = new Common.SampleGrid();
                sampleGrid.Width = selectNewImageSizeWindow.ImageWidth;
                sampleGrid.Height = selectNewImageSizeWindow.ImageHeight;

                sampleGrid.Recreate();
            }
            else
            {
                sampleGrid = null; // this will just clear the PaintCanvas
            }

            SetNewContent(sampleGrid, new Size(selectNewImageSizeWindow.ImageWidth, selectNewImageSizeWindow.ImageHeight));
        }

        private void SetNewContent(FrameworkElement newContent, Size newContentSize)
        {
            PaintCanvas.Children.Clear();

            if (newContent != null)
                PaintCanvas.Children.Add(newContent);

            PaintCanvas.Width = newContentSize.Width;
            PaintCanvas.Height = newContentSize.Height;

            ContextSizeTextBlock.Text = string.Format("Content size: {0:0} x {1:0}", newContentSize.Width, newContentSize.Height);

            ZoomPanel1.Refresh(); // Refresh the slider values, etc.
            ZoomPanel1.ResetNow(); // reset the zoom and position
        }

        private void SaveImageButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog saveFileDialog;

            saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            saveFileDialog.AddExtension = true;
            saveFileDialog.CheckFileExists = false;
            saveFileDialog.CheckPathExists = true;
            saveFileDialog.OverwritePrompt = true;
            saveFileDialog.ValidateNames = true;

            saveFileDialog.FileName = "PainterSample.jpg";
            saveFileDialog.DefaultExt = "jpg";
            saveFileDialog.Filter = "JPEG (*.jpg)|*.jpg";
            saveFileDialog.Title = "Select output file";

            if (saveFileDialog.ShowDialog() ?? false)
            {
                SelectNewImageSizeWindow selectNewImageSizeWindow = new SelectNewImageSizeWindow()
                {
                    ImageWidth = PaintCanvas.ActualWidth,
                    ImageHeight = PaintCanvas.ActualHeight,
                    TitleText = "Select size of the bitmap image to export:",
                    ShowUseSampleGridCheckBox = false,
                    ShowPreserveAspectRatioCheckBox = true,
                    Owner = Application.Current.MainWindow
                };

                selectNewImageSizeWindow.ShowDialog();

                if (selectNewImageSizeWindow.ImageWidth > 0 && selectNewImageSizeWindow.ImageHeight > 0)
                {
                    Visual visualToRender;

                    if (selectNewImageSizeWindow.ImageWidth == PaintCanvas.ActualWidth && selectNewImageSizeWindow.ImageHeight == PaintCanvas.ActualHeight)
                    {
                        visualToRender = PaintCanvas;
                    }
                    else
                    {
                        Rectangle rectangle = new Rectangle();
                        rectangle.Width = selectNewImageSizeWindow.ImageWidth;
                        rectangle.Height = selectNewImageSizeWindow.ImageHeight;

                        rectangle.Fill = new VisualBrush(PaintCanvas);

                        rectangle.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                        rectangle.Arrange(new Rect(0, 0, rectangle.Width, rectangle.Height));

                        visualToRender = rectangle;
                    }

                    BitmapSource image = RenderImage(visualToRender, selectNewImageSizeWindow.ImageWidth, selectNewImageSizeWindow.ImageHeight);
                    SaveResultImage(image, saveFileDialog.FileName);
                }
            }
        }

        private string GetFileToOpen(string filter)
        { 
            string fileName;
            Microsoft.Win32.OpenFileDialog openFileDialog;

            openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = filter;
            openFileDialog.Multiselect = false;
            openFileDialog.Title = "Select file to open";

            if (openFileDialog.ShowDialog() ?? false)
                fileName = openFileDialog.FileName;
            else
                fileName = null;

            return fileName;
        }

        private void OpenImageButton_Click(object sender, RoutedEventArgs e)
        {
            string fileName = GetFileToOpen("All Images|*.jpg;*.png;*.gif");

            if (!string.IsNullOrEmpty(fileName))
            {
                BitmapImage bitmapImage = new BitmapImage(new Uri(fileName));
                Image image = new Image();
                image.Source = bitmapImage;
                image.Width = bitmapImage.Width;
                image.Height = bitmapImage.Height;

                SetNewContent(image, new Size(image.Width, image.Height));
            }
        }

        private void OpenXamlButton_Click(object sender, RoutedEventArgs e)
        {
            string fileName = GetFileToOpen("XAML (*.xaml)|*.xaml");

            if (!string.IsNullOrEmpty(fileName))
            {
                FrameworkElement newContent = null;

                try
                {
                    newContent = System.Windows.Markup.XamlReader.Load(System.IO.File.OpenRead(fileName)) as FrameworkElement;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error importing xaml:\r\n" + ex.Message, "Cannot import xaml", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                if (newContent != null)
                { 
                    newContent.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    SetNewContent(newContent, newContent.DesiredSize);
                }
            }
        }

        private void SaveXamlButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog saveFileDialog;

            saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            saveFileDialog.AddExtension = true;
            saveFileDialog.CheckFileExists = false;
            saveFileDialog.CheckPathExists = true;
            saveFileDialog.OverwritePrompt = true;
            saveFileDialog.ValidateNames = true;

            saveFileDialog.FileName = "PainterSample.xaml";
            saveFileDialog.DefaultExt = "xaml";
            saveFileDialog.Filter = "XAML (*.xaml)|*.xaml";
            saveFileDialog.Title = "Select output file";

            if (saveFileDialog.ShowDialog() ?? false)
            {
                XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
                xmlWriterSettings.Indent = true;

                using (XmlWriter xmlWriter = XmlWriter.Create(saveFileDialog.FileName, xmlWriterSettings))
                {
                    Cursor savedCursor = PaintCanvas.Cursor;
                    PaintCanvas.Cursor = null; // reset the cursor

                    System.Windows.Markup.XamlWriter.Save(PaintCanvas, xmlWriter);

                    PaintCanvas.Cursor = savedCursor;
                }
            }
        }


        private void PaintCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            PaintCanvas.ReleaseMouseCapture();

            _isDrawing = false;
        }

        private void PaintCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point startPoint;

            if (ZoomPanel1.ZoomMode != Ab2d.Controls.ZoomPanel.ZoomModeType.None)
                return;

            startPoint = e.GetPosition(PaintCanvas);

            AddNewPolyline(startPoint);

            PaintCanvas.CaptureMouse();

            _isDrawing = true;
        }

        private void PaintCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            Point currentPoint;

            currentPoint = e.GetPosition(PaintCanvas);

            if (_isDrawing)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                    _currentPolylinePoints.Add(currentPoint);
                else
                    _isDrawing = false; // just in case (with MouseCapture this should not happen)
            }

            if (ZoomPanel1.ZoomMode == Ab2d.Controls.ZoomPanel.ZoomModeType.None)
                PaintCanvas.Cursor = Cursors.Cross;
            else
                PaintCanvas.Cursor = null; // allow ZoomPanel to set its own cursor

            MousePositionTextBlock.Text = string.Format("Position: ({0:0}, {1:0})", currentPoint.X, currentPoint.Y);


            // When mouse button is pressed (we are drawing) and mouse goes outside ZoomPanel,
            // we want to move into the direction of the mouse
            var zoomPanelPosition = e.GetPosition(ZoomPanel1);

            double dx, dy;

            if (zoomPanelPosition.X < 0)
                dx = zoomPanelPosition.X; // left of ZoomPanel
            else if (zoomPanelPosition.X > ZoomPanel1.ActualWidth)
                dx = zoomPanelPosition.X - ZoomPanel1.ActualWidth; // Right of ZoomPanel
            else
                dx = 0; // inside ZoomPanel

            if (zoomPanelPosition.Y < 0)
                dy = zoomPanelPosition.Y; // Above of ZoomPanel
            else if (zoomPanelPosition.Y > ZoomPanel1.ActualHeight) 
                dy = zoomPanelPosition.Y - ZoomPanel1.ActualHeight; // Below of ZoomPanel
            else
                dy = 0; // inside ZoomPanel

            if (dx != 0 || dy != 0)
                ZoomPanel1.TranslateNow(-dx * 0.01, -dy * 0.01); // Reduce the speed of movement so that the user has more control
        }

        private void PaintCanvas_MouseEnter(object sender, MouseEventArgs e)
        {
            MousePositionTextBlock.Visibility = System.Windows.Visibility.Visible;
        }

        private void PaintCanvas_MouseLeave(object sender, MouseEventArgs e)
        {
            MousePositionTextBlock.Visibility = System.Windows.Visibility.Collapsed;
        }


        private void SetStroke(System.Windows.Shapes.Path path)
        {
            path.Stroke = _settings.CurrentStrokeBrush;
            path.StrokeThickness = _settings.CurrentStrokeThickness;
            path.StrokeStartLineCap = PenLineCap.Round;
            path.StrokeEndLineCap = PenLineCap.Round;
            path.StrokeLineJoin = PenLineJoin.Round;
        }

        private void AddNewPolyline(Point startPoint)
        {
            System.Windows.Media.PolyLineSegment polyLineSegment;
            System.Windows.Media.PathFigureCollection pathFigures;
            System.Windows.Media.PathFigure pathFigure;
            System.Windows.Shapes.Path path;
            PathGeometry geometry;


            _currentPolylinePoints = new PointCollection();

            pathFigure = new PathFigure();

            polyLineSegment = new PolyLineSegment();
            polyLineSegment.Points = _currentPolylinePoints;

            pathFigure.StartPoint = startPoint;
            pathFigure.Segments.Add(polyLineSegment);


            pathFigures = new PathFigureCollection();
            pathFigures.Add(pathFigure);

            geometry = new PathGeometry();
            geometry.Figures = pathFigures;

            path = new System.Windows.Shapes.Path();

            SetStroke(path);

            path.Data = geometry;

            PaintCanvas.Children.Add(path);
        }

        public static long SaveResultImage(BitmapSource image, string resulImageFileName)
        {
            long resultImageLength;

            // write the bitmap to a file
            using (FileStream fs = new FileStream(resulImageFileName, FileMode.Create))
            {
                //JpegBitmapEncoder enc = new JpegBitmapEncoder();
                PngBitmapEncoder enc = new PngBitmapEncoder();
                BitmapFrame bitmapImage = BitmapFrame.Create(image);
                enc.Frames.Add(bitmapImage);
                enc.Save(fs);

                resultImageLength = fs.Length;
            }

            return resultImageLength;
        }

        public static BitmapSource RenderImage(System.Windows.Media.Visual objectToRender, double width, double height)
        {
            // Add white background
            Rectangle backgroundRect = new Rectangle();
            backgroundRect.Width = width;
            backgroundRect.Height = height;
            backgroundRect.Fill = Brushes.White;
            backgroundRect.Arrange(new Rect(0, 0, width, height));

            RenderTargetBitmap bmp = new RenderTargetBitmap(Convert.ToInt32(width), Convert.ToInt32(height), 96, 96 /* standard WPF dpi setting 96 */, PixelFormats.Pbgra32);
            bmp.Render(backgroundRect);
            bmp.Render(objectToRender);

            return bmp;
        }
    }

    public class PainterSettings : INotifyPropertyChanged
    {
        public static readonly RoutedCommand SetStrokeThicknessCommand = new RoutedCommand("SetStrokeThicknessCommand", typeof(PainterSettings));
        public static readonly RoutedCommand SetStrokeColorCommand = new RoutedCommand("SetStrokeColorCommand", typeof(PainterSettings));

        private Brush _currentStrokeBrush;
        public Brush CurrentStrokeBrush
        {
            get { return _currentStrokeBrush; }
            set
            {
                _currentStrokeBrush = value;
                NotifyPropertyChanged("CurrentStrokeBrush");
            }
        }

        private double _currentStrokeThickness;
        public double CurrentStrokeThickness
        {
            get { return _currentStrokeThickness; }
            set
            {
                _currentStrokeThickness = value;
                NotifyPropertyChanged("CurrentStrokeThickness");
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
