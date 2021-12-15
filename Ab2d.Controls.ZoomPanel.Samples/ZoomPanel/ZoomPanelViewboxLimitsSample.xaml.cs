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
    /// Interaction logic for ZoomPanelViewboxLimitsSample.xaml
    /// </summary>
    public partial class ZoomPanelViewboxLimitsSample : Page
    {
        public ZoomPanelViewboxLimitsSample()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ZoomPanel1.ZoomMode = Ab2d.Controls.ZoomPanel.ZoomModeType.Move;
        }
    }
}
