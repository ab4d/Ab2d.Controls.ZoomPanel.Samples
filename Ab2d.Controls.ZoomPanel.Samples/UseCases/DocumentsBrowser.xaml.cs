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
using System.IO;
using System.ComponentModel;

namespace Ab2d.ZoomControlSample.UseCases
{
    /// <summary>
    /// Interaction logic for DocumentsBrowser.xaml
    /// </summary>
    public partial class DocumentsBrowser : Page
    {
        private bool _internalChange;
        private TextBox _editTextBox;

        public DocumentsBrowser()
        {
            InitializeComponent();

            this.ZoomPanel1.ViewboxChanged += new Ab2d.Controls.ZoomPanel.ViewboxChangedRoutedEventHandler(ZoomPanel1_ViewboxChanged);

            this.Loaded += new RoutedEventHandler(DocumentsBrowser_Loaded);
        }

        void DocumentsBrowser_Loaded(object sender, RoutedEventArgs e)
        {
            //ZoomCombobox.SelectedValue = FitWidthComboBoxItem;
            ZoomCombobox.SelectedIndex = 8;

            // Unfortunately, the first version of WPF does not provide a TextChanged event for a ComboBox like the one available for a TextBox. Workaround:
            _editTextBox = ZoomCombobox.Template.FindName("PART_EditableTextBox", ZoomCombobox) as TextBox;
            if (_editTextBox != null)
                _editTextBox.TextChanged += new TextChangedEventHandler(editTextBox_TextChanged);           
        }

        void ZoomPanel1_ViewboxChanged(object sender, Ab2d.Controls.ViewboxChangedRoutedEventArgs e)
        {
            double percentage;

            if (_internalChange || (e.NewViewboxValue.Width == e.OldViewboxValue.Width && e.NewViewboxValue.Height == e.OldViewboxValue.Height))
            {
                // if internal change or zoom factor is the same (only the Viewbox position is changed)
                return;
            }

            percentage = 1 / ((e.NewViewboxValue.Width + e.NewViewboxValue.Height) / 2); // get an average zoom percentage

            _internalChange = true;
            ZoomCombobox.Text = string.Format("{0:#0.#}%", percentage * 100);
            _internalChange = false;
        }

        void editTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            double percentage;
            string percentageText;
            ComboBoxItem selectedComboBoxItem;

            if (_internalChange)
                return;

            selectedComboBoxItem = ZoomCombobox.SelectedValue as ComboBoxItem;

            if (selectedComboBoxItem != null) // The change will be handled in ZoomCombobox_SelectionChanged
                return;

            percentageText = _editTextBox.Text.Replace("%", ""); // remove the '%'

            if (double.TryParse(percentageText, out percentage))
            {
                percentage = double.Parse(percentageText) / 100;

                _internalChange = true;
                ZoomPanel1.ZoomToFactor(percentage);
                _internalChange = false;
            }
        }

        private void ZoomCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedText;

            if (!ZoomCombobox.IsInitialized || _internalChange)
                return;


            ComboBoxItem selectedComboBoxItem;

            selectedComboBoxItem = ZoomCombobox.SelectedValue as ComboBoxItem;

            if (selectedComboBoxItem == null)
                return;


            selectedText = selectedComboBoxItem.Content as string;

            if (string.IsNullOrEmpty(selectedText))
                return;

            _internalChange = true; // Do not update the TextBox inside Combobox

            // If fist char is digit, than user selected one of the percent values
            if (char.IsDigit(selectedText[0]))
            {
                double percentage;
                string percentageText;

                percentageText = selectedText.Replace("%", ""); // remove the '%'
                percentage = double.Parse(percentageText) / 100;

                ZoomPanel1.ZoomToFactor(percentage);
            }
            else if (selectedComboBoxItem == FitWidthComboBoxItem)
            {
                if (ZoomPanel1.IsViewboxLimited)
                    ZoomPanel1.FitToLimitsWidth();
                else
                    ZoomPanel1.FitToWidth();
            }
            else if (selectedComboBoxItem == FitHeightComboBoxItem)
            {
                if (ZoomPanel1.IsViewboxLimited)
                    ZoomPanel1.FitToLimitsHeight();
                else
                    ZoomPanel1.FitToHeight();
            }
            else
            {
                ZoomPanel1.Reset();
            }

            _internalChange = false;
        }
    }
}
