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

namespace Ab2d.ZoomControlSample.DampeningTimer
{
    /// <summary>
    /// Interaction logic for DampeningTimerTester.xaml
    /// </summary>
    public partial class DampeningTimerTester : Page
    {
        private const double X_MARGIN = 15;

        private List<Point> _points;
        private DateTime _startTime;
        private Ab2d.Common.DampeningTimer _dampeningTimer;

        private double _minLimit, _maxLimit;

        private double _pointsPerSecond;

        public DampeningTimerTester()
        {
            InitializeComponent();

            _pointsPerSecond = 200;

            _points = new List<Point>();

            _dampeningTimer = new Ab2d.Common.DampeningTimer();
            _dampeningTimer.Tick += new Ab2d.Common.DampeningTimerTickEventHandler(_dampeningTimer_Tick);
            _dampeningTimer.Completed += new EventHandler(_dampeningTimer_Completed);
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            _dampeningTimer.StartValue             = double.Parse(StartValueTextBox.Text, System.Globalization.CultureInfo.InvariantCulture);
            _dampeningTimer.EndValue               = double.Parse(EndValueTextBox.Text, System.Globalization.CultureInfo.InvariantCulture);
            _dampeningTimer.Dampening              = double.Parse(DampeningTextBox.Text, System.Globalization.CultureInfo.InvariantCulture);
            _dampeningTimer.Attraction             = double.Parse(AttractionTextBox.Text, System.Globalization.CultureInfo.InvariantCulture);
            _dampeningTimer.TerminalVelocity       = double.Parse(TerminalVelocityTextBox.Text, System.Globalization.CultureInfo.InvariantCulture);
            _dampeningTimer.AnimationMinDifference = double.Parse(AnimationMinDifferenceTextBox.Text, System.Globalization.CultureInfo.InvariantCulture);
            _dampeningTimer.DesiredTicksPerSecond  = Int32.Parse(DesiredTicksPerSecondTextBox.Text, System.Globalization.CultureInfo.InvariantCulture);

            SetGraphLimits(_dampeningTimer.StartValue, _dampeningTimer.EndValue);
            DrawAxis(_dampeningTimer.StartValue, _dampeningTimer.EndValue);

            _lastPoint = new Point(X_MARGIN, NormalizeValue(_dampeningTimer.StartValue));
            _points = new List<Point>();
            ResultsTextBox1.Text = string.Format(" TIME   VALUE\n{0:0.000}s {1:0.00}\n", 0, _dampeningTimer.StartValue);

            _startTime = DateTime.Now;
            _dampeningTimer.Start();

            StartButton.IsEnabled = false;
            StopButton.IsEnabled = true;
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            _dampeningTimer.Stop();

            StopButton.IsEnabled = false;
            StartButton.IsEnabled = true;
        }

        public void StopAnimation()
        {
            _dampeningTimer.Stop();
        }

        void _dampeningTimer_Tick(object sender, Ab2d.Common.DampeningTimerTickEventArgs e)
        {
            ResultsTextBox1.Text += string.Format("{0:0.000}s {1:0.00}\n", (DateTime.Now - _startTime).TotalSeconds, e.CurrentValue);
            AddValue(e.CurrentValue);
        }

        void _dampeningTimer_Completed(object sender, EventArgs e)
        {
            ResultsTextBox1.Text += string.Format("{0:0.000}s {1:0.00}\n", (DateTime.Now - _startTime).TotalSeconds, _dampeningTimer.CurrentValue);

            StartButton.IsEnabled = true;
        }

        private Point _lastPoint;

        private void AddValue(double newValue)
        {
            double x, y;

            if (newValue > _maxLimit || newValue < _minLimit)
                return; // out of bounds

            x = X_MARGIN + (DateTime.Now - _startTime).TotalSeconds * _pointsPerSecond;
            y = NormalizeValue(newValue);

            if (PreviewCanvas.Width < x) // resize PreviewCanvas if needed
            {
                PreviewCanvas.Width = x;
                PreviewScrollViewer.ScrollToRightEnd();
            }

            Line line;
            line = new Line();
            line.X1 = _lastPoint.X;
            line.Y1 = _lastPoint.Y;
            line.X2 = x;
            line.Y2 = y;
            line.Stroke = Brushes.Blue;
            line.StrokeThickness = 2;

            PreviewCanvas.Children.Add(line);

            _lastPoint = new Point(x, y);

            //_points.Add(new Point(x, y));

            //RedrawPoints();
        }

        private void RedrawPoints()
        {
            Point startPoint;

            startPoint = new Point(X_MARGIN, NormalizeValue(_dampeningTimer.StartValue));

            StreamGeometry streamGeometry;

            streamGeometry = new StreamGeometry();

            using (StreamGeometryContext context = streamGeometry.Open())
            {
                context.BeginFigure(startPoint, false, false);
                context.PolyLineTo(_points, true, true);
            }
        }


        // Puts the value inside the PreviewCanvas limits
        private double NormalizeValue(double originalValue)
        {
            double percent;

            if (originalValue > _maxLimit)
                percent = 1; // top
            else if (originalValue < _minLimit)
                percent = 0; // bottom
            else
                percent = (originalValue - _minLimit) / (_maxLimit - _minLimit);

            return (1 - percent) * PreviewCanvas.Height; // flip y
        }

        private void SetGraphLimits(double startValue, double endValue)
        {
            double end1, end2;
            double start1, start2;
            double diff;

            diff = Math.Abs(endValue - startValue);

            start1 = startValue + diff * 0.2; // 20% space around start value
            start2 = startValue - diff * 0.2;

            end1 = endValue + diff * 0.8; // 80% space around end value
            end2 = endValue - diff * 0.8;

            _maxLimit = Math.Max(start1, end1);
            _minLimit = Math.Min(start2, end2);
        }

        private void DrawAxis(double startValue, double endValue)
        {
            double pos;
            Line line;

            PreviewCanvas.Children.Clear();

            pos = NormalizeValue(startValue);

            // first horizontal line
            line = new Line();
            line.Stroke = Brushes.Gray;
            line.X1 = X_MARGIN / 2;
            line.Y1 = pos;
            line.X2 = PreviewCanvas.Width - X_MARGIN;
            line.Y2 = pos;

            PreviewCanvas.Children.Add(line);


            // second horizontal line
            pos = NormalizeValue(endValue);

            line = new Line();
            line.Stroke = Brushes.Gray;
            line.X1 = X_MARGIN / 2;
            line.Y1 = pos;
            line.X2 = PreviewCanvas.Width - X_MARGIN;
            line.Y2 = pos;

            PreviewCanvas.Children.Add(line);


            // vertical line
            line = new Line();
            line.Stroke = Brushes.Gray;
            line.X1 = X_MARGIN;
            line.Y1 = 10;
            line.X2 = X_MARGIN;
            line.Y2 = PreviewCanvas.Height - 10;

            PreviewCanvas.Children.Add(line);
        }
    }
}
