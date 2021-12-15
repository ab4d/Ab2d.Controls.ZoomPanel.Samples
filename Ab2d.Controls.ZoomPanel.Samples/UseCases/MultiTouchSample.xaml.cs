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

namespace Ab2d.ZoomControlSample.UseCases
{
    /// <summary>
    /// Interaction logic for MultiTouchSample.xaml
    /// </summary>
    public partial class MultiTouchSample : Page
    {
        public MultiTouchSample()
        {
            InitializeComponent();

            // Touch control is supported only in .Net 4 build of ZoomPanel (or newer) and is not supported in .Net 3.5 build
            // We can check which build we are using with checking the default value of IsTouchEnabled property - if true, than we are using .Net 4 build, else .Net 3.5 build is used
            if (!ZoomPanel1.IsTouchEnabled)
            {
                // Using .Net 3.5 build
                // Show info TextBlock and disable CheckBoxes
                NotNet4TextBlock.Visibility = Visibility.Visible;

                IsEnabledCheckbox.IsChecked = false;
                IsEnabledCheckbox.IsEnabled = false;
            }
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            ZoomPanel1.Reset();
        }
    }
}
