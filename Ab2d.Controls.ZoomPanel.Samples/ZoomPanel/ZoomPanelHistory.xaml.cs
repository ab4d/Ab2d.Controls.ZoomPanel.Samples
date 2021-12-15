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

namespace Ab2d.ZoomControlSample.ZoomPanel
{
    /// <summary>
    /// Interaction logic for ZoomPanelHistory.xaml
    /// </summary>
    public partial class ZoomPanelHistory : Page
    {
        public ZoomPanelHistory()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(ZoomPanelHistory_Loaded);

            ZoomPanel1.ViewboxChanged += new Controls.ZoomPanel.ViewboxChangedRoutedEventHandler(ZoomPanel1_ViewboxChanged);
            ZoomPanel1.HistoryItems.CurrentHistoryItemChanged += new Controls.ZoomPanel.HistoryItemsChangedEventHandler(HistoryItems_CurrentHistoryItemChanged);
        }

        void HistoryItems_CurrentHistoryItemChanged(object sender, Controls.ZoomPanel.HistoryItemsChangedEventArgs e)
        {
            UpdateHistoryList();
        }

        void ZoomPanelHistory_Loaded(object sender, RoutedEventArgs e)
        {
            MaxItemsTextBlock.Text = "MaxItems: " + ZoomPanel1.HistoryItems.MaxItems.ToString();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            ZoomPanel1.HistoryItems.MoveBack();
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            ZoomPanel1.HistoryItems.MoveNext();
        }

        private void ZoomPanel1_ViewboxChanged(object sender, Controls.ViewboxChangedRoutedEventArgs e)
        {
            // NOTE:
            // Because the zooming is animated the current Viewbox and RotationAngle are not yet set to the final values
            // Therefore values from ViewboxChangedRoutedEventArgs should be used
            UpdateCurrentViewbox(e.NewViewboxValue, e.NewRotationAngle);
        }

        private void RotateRightButton_Click(object sender, RoutedEventArgs e)
        {
            // Animate the rotation
            ZoomPanel1.SetZoom(ZoomPanel1.Viewbox, ZoomPanel1.RotationAngle - 30);
            
            //ZoomPanel1.RotationAngle -= 30; // this change is not animated
        }

        private void RotateLeftButton_Click(object sender, RoutedEventArgs e)
        {
            // Animate the rotation
            ZoomPanel1.SetZoom(ZoomPanel1.Viewbox, ZoomPanel1.RotationAngle + 30);

            //ZoomPanel1.RotationAngle += 30; // this change is not animated
        }

        private void CleanButton_Click(object sender, RoutedEventArgs e)
        {
            ZoomPanel1.HistoryItems.Clear();
            UpdateHistoryList();
        }

        private void UpdateHistoryList()
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < ZoomPanel1.HistoryItems.ItemsCount; i++)
            {
                Ab2d.Controls.ZoomPanel.ZoomPanelHistoryItem item = ZoomPanel1.HistoryItems.GetItem(i);

                sb.AppendFormat("{0} {1} ({2}) {3:0}\r\n", 
                    (ZoomPanel1.HistoryItems.CurrentIndex == i) ? "->" : "  ",
                    i,
                    FormatRect(item.Viewbox), 
                    item.RotationAngle);
            }

            ItemsTextBox.Text = sb.ToString();
        }

        private void UpdateCurrentViewbox(Rect newViewbox, double newRotationAngle)
        {
            CurrentViewboxTextBlock.Text = FormatRect(newViewbox);
            CurrentRotationAngleTextBlock.Text = string.Format("{0:0}", newRotationAngle);
        }

        private string FormatRect(Rect rect)
        {
            return string.Format("{0:0.0} {1:0.0}  {2:0.0} {3:0.0}", rect.X, rect.Y, rect.Width, rect.Height);
        }
    }
}
