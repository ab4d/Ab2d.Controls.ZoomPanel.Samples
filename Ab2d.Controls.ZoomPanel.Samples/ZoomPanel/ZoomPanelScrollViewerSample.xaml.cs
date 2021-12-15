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
    /// Interaction logic for ZoomPanelScrollViewerSample.xaml
    /// </summary>
    public partial class ZoomPanelScrollViewerSample : Page
    {
        private string[] _allSamples;

        public ZoomPanelScrollViewerSample()
        {
            InitializeComponent();

            _allSamples = new string[] { "tiger.xaml", "conferenze_2d_5.xaml", "airbus.xaml", "hangar_1.xaml", "airport_1.xaml", "palazz_sport.xaml" };
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ZoomPanel1.ZoomMode = Ab2d.Controls.ZoomPanel.ZoomModeType.Move;
        }

        private void SceneListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Uri newSource;

            if (!ZoomPanelContentFrame.IsLoaded || SceneListBox.SelectedIndex < 0 || SceneListBox.SelectedIndex > _allSamples.Length)
                return;

            newSource = new Uri("/Resources/" + _allSamples[SceneListBox.SelectedIndex], UriKind.Relative);

            ZoomPanelContentFrame.Source = newSource;
        }
    }
}
