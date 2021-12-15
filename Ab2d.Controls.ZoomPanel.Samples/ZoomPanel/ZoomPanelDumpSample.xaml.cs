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

namespace Ab2d.ZoomControlSample.ZoomPanel
{
    /// <summary>
    /// Interaction logic for ZoomPanelDumpSample.xaml
    /// </summary>
    public partial class ZoomPanelDumpSample : Page
    {
        public ZoomPanelDumpSample()
        {
            InitializeComponent();
        }

        private void ShowAdditionalInfoCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (!this.IsLoaded)
                return;

            ZoomPanelDump1.ShowAdditionalInfo = true;
        }

        private void ShowAdditionalInfoCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!this.IsLoaded)
                return;

            ZoomPanelDump1.ShowAdditionalInfo = false;
        }
    }
}
