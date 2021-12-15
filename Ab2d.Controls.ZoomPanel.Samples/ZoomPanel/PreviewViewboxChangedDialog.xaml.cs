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
using System.ComponentModel;

namespace Ab2d.ZoomControlSample.ZoomPanel
{
    /// <summary>
    /// Interaction logic for PreviewViewboxChangedDialog.xaml
    /// </summary>
    public partial class PreviewViewboxChangedDialog : Window
    {
        public Rect OldViewbox { get; set; }
        public Rect NewViewbox { get; set; }
        
        public double OldRotationAngle { get; set; }
        public double NewRotationAngle { get; set; }

        public bool AllowChange { get; set; }

        public PreviewViewboxChangedDialog()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            OldViewboxTextBox.Text = ZoomPanelEventsSample.FormatRect(OldViewbox);
            NewViewboxTextBox.Text = ZoomPanelEventsSample.FormatRect(NewViewbox);


            OldRotationAngleTextBox.Text = string.Format("{0:0}", OldRotationAngle);
            NewRotationAngleTextBox.Text = string.Format("{0:0}", NewRotationAngle);
        }

        private void DenyChangeButton_Click(object sender, RoutedEventArgs e)
        {
            if (UpdateData())
            {
                AllowChange = false;
                this.Close();
            }
        }

        private void AllowChangeButton_Click(object sender, RoutedEventArgs e)
        {
            if (UpdateData())
            {
                AllowChange = true;
                this.Close();
            }
        }

        private bool UpdateData()
        {
            bool success;
            TypeConverter rectConverter;

            rectConverter = TypeDescriptor.GetConverter(typeof(Rect));

            success = true;

            try
            {
                NewViewbox = (Rect)rectConverter.ConvertFromInvariantString(NewViewboxTextBox.Text);
                NewViewboxTextBox.BorderBrush = null;
            }
            catch
            {
                NewViewboxTextBox.BorderBrush = Brushes.Red;
                success = false;
            }

            try
            {
                NewRotationAngle = double.Parse(NewRotationAngleTextBox.Text);
                NewRotationAngleTextBox.BorderBrush = null;
            }
            catch
            {
                NewRotationAngleTextBox.BorderBrush = Brushes.Red;
                success = false;
            }

            return success;
        }

    }
}
