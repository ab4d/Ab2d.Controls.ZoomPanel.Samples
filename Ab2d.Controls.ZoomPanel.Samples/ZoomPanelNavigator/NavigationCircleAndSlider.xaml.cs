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

namespace Ab2d.ZoomControlSample.ZoomPanelNavigator
{
    /// <summary>
    /// Interaction logic for NavigationCircleAndSlider.xaml
    /// </summary>
    public partial class NavigationCircleAndSlider : Page
    {
        public NavigationCircleAndSlider()
        {
            InitializeComponent();
        }

        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true; // enable all commands
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CircleTextBox.Text = ((RoutedUICommand)e.Command).Name + Environment.NewLine + CircleTextBox.Text;
        }

        private void VerticalSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            VerticalSliderTextBox.Text = String.Format("Slider value: {0:0.00}\r\n{1}", e.NewValue, VerticalSliderTextBox.Text);
        }

        private void HorizontalSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            HorizontalSliderTextBox.Text = String.Format("Slider value: {0:0.00}\r\n{1}", e.NewValue, HorizontalSliderTextBox.Text);
        }
    }
}
