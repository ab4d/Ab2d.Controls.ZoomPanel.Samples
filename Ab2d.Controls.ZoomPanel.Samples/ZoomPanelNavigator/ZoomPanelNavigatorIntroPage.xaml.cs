using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Ab2d.ZoomControlSample.ZoomPanelNavigator
{
    public partial class ZoomPanelNavigatorIntroPage : Page
    {
        public ZoomPanelNavigatorIntroPage()
        {
            InitializeComponent();
        }

        private void link_navigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.ToString());
            e.Handled = true;
        }
    }
}