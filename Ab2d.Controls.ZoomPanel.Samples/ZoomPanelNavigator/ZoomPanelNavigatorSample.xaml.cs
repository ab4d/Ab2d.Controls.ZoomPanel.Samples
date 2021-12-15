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
    /// Interaction logic for ZoomPanelNavigatorSample.xaml
    /// </summary>
    public partial class ZoomPanelNavigatorSample : Page
    {
        public ZoomPanelNavigatorSample()
        {
            InitializeComponent();

            IsLimitedCheckBox.ToolTip = ((string)IsLimitedCheckBox.ToolTip).Replace("\\n", Environment.NewLine);

            this.Loaded += new RoutedEventHandler(ZoomPanelNavigatorSample_Loaded);
        }

        void ZoomPanelNavigatorSample_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateMinZoomFactor();
            UpdateMaxZoomFactor();
        }

        private void MinComboxBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!this.IsLoaded)
                return;

            UpdateMinZoomFactor();
        }

        private void MaxComboxBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!this.IsLoaded)
                return;

            UpdateMaxZoomFactor();
        }

        private void UpdateMinZoomFactor()
        {
            ComboBoxItem item = (ComboBoxItem)MinComboxBox.SelectedItem;

            double min = double.Parse((string)item.Content, System.Globalization.CultureInfo.InvariantCulture);

            ZoomPanelNavigator1.MinZoomFactor = min;
        }

        private void UpdateMaxZoomFactor()
        {
            ComboBoxItem item = (ComboBoxItem)MaxComboxBox.SelectedItem;

            double max = double.Parse((string)item.Content, System.Globalization.CultureInfo.InvariantCulture);

            ZoomPanelNavigator1.MaxZoomFactor = max;
        }
    }
}
