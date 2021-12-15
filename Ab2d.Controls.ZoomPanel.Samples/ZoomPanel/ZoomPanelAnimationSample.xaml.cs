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
using Ab2d.Animations;
using Ab2d.ZoomControlSample.CustomAnimations;

namespace Ab2d.ZoomControlSample.ZoomPanel
{
    /// <summary>
    /// Interaction logic for ZoomPanelAnimationSample.xaml
    /// </summary>
    public partial class ZoomPanelAnimationSample : Page
    {
        private ZoomPanelLinearAnimator _linearAnimator;
        private ZoomPanelQuinticAnimator _defaultQuinticAnimator;
        private ZoomPanelQuinticAnimator _customQuinticAnimator;

        // ZoomPanelSineAnimator and ZoomPanelZoomOutAnimator are defined in CustomAnimations folder
        private ZoomPanelSineAnimator _customSineAnimator;
        private ZoomPanelZoomOutAnimator _customZoomOutAnimator;

        public ZoomPanelAnimationSample()
        {
            InitializeComponent();

            // A simple linear animation that is also included to the ZoomPanel
            _linearAnimator = new ZoomPanelLinearAnimator();

            // Default quintic animation that adds some easing to animations
            // The description for quintic function can be found at: http://en.wikipedia.org/wiki/Quintic_function
            // The function uses 5 parameters that specify how the function behaves.
            // The default parameter values are: 6, -15, 10, 0, 0
            // To get some other parameters the following page can be used: http://www.timotheegroleau.com/Flash/experiments/easing_function_generator.htm 
            _defaultQuinticAnimator = new ZoomPanelQuinticAnimator();

            // The custom quintic animator uses some custom parameters
            _customQuinticAnimator = new ZoomPanelQuinticAnimator();
            _customQuinticAnimator.SetParameters(4.5, -9, 1, 6, -1.5);

            // Our custom animator class defined below
            _customSineAnimator = new ZoomPanelSineAnimator();

            // Second custom animator is used in the ZoomToObjects sample
            _customZoomOutAnimator = new ZoomPanelZoomOutAnimator();


            // NOTE: By default ZoomPanel is already using ZoomPanelQuinticAnimator
            // Here we just change it to our instance of ZoomPanelQuinticAnimator
            ZoomPanel1.ZoomPanelAnimator = _defaultQuinticAnimator;

            UpdateAnimationGraph();
        }

        private void AnimatorComboxBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!this.IsLoaded)
                return;

            if (AnimatorComboxBox.SelectedItem == NoAnimationItem)
            {
                // It would be also possible to call
                //ZoomPanel1.IsAnimated = false;
                ZoomPanel1.ZoomPanelAnimator = null;
            }
            else if (AnimatorComboxBox.SelectedItem == LinearAnimationItem)
            {
                ZoomPanel1.ZoomPanelAnimator = _linearAnimator;
            }
            else if (AnimatorComboxBox.SelectedItem == DefaultQuinticAnimationItem)
            {
                ZoomPanel1.ZoomPanelAnimator = _defaultQuinticAnimator;
            }
            else if (AnimatorComboxBox.SelectedItem == CustomQuinticAnimationItem)
            {
                ZoomPanel1.ZoomPanelAnimator = _customQuinticAnimator;
            }
            else if (AnimatorComboxBox.SelectedItem == CustomAnimationItem)
            {
                ZoomPanel1.ZoomPanelAnimator = _customSineAnimator;
            }
            else if (AnimatorComboxBox.SelectedItem == Custom2AnimationItem)
            {
                ZoomPanel1.ZoomPanelAnimator = _customZoomOutAnimator;
            }

            UpdateAnimationGraph();
        }

        private void UpdateAnimationGraph()
        {
            AnimationGraphCanvas.Children.Clear();

            if (ZoomPanel1.ZoomPanelAnimator == null)
                return;

            Polyline graph = new Polyline();
            graph.Stroke = Brushes.Blue;
            graph.StrokeThickness = 1;

            for (double progress = 0; progress < 1; progress+=0.025)
            {
                double x, y;

                x = progress * AnimationGraphCanvas.Width;

                y = 1 - ZoomPanel1.ZoomPanelAnimator.CalculateValue(0, 1, progress);
                y *= AnimationGraphCanvas.Height;

                graph.Points.Add(new Point(x, y));
            }

            AnimationGraphCanvas.Children.Add(graph);
        }
    }
}
