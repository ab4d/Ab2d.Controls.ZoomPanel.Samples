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
using System.Reflection;

namespace Ab2d.ZoomControlSample.ZoomPanelNavigator
{
    /// <summary>
    /// Interaction logic for NavigatorCustomization.xaml
    /// </summary>
    public partial class NavigatorCustomization : Page
    {
        public NavigatorCustomization()
        {
            InitializeComponent();

            this.Unloaded += new RoutedEventHandler(NavigatorCustomization_Unloaded);
        }

        void NavigatorCustomization_Unloaded(object sender, RoutedEventArgs e)
        {
            ResetSettings();
        }

        private void SetSelectedColorsButton_Click(object sender, RoutedEventArgs e)
        {
            Color selectedColor, selectedContentColor;

            selectedColor        = GetColorFromComboBox(SelectedFillColorComboBox);
            selectedContentColor = GetColorFromComboBox(SelectedContentColorComboBox);

            Ab2d.Common.ZoomPanel.CommonNavigationResources.Instance.SetSelectedColor(selectedColor, selectedContentColor);
        }

        private void SetAllColorsButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateAllColors();
        }

        private void ResetColorsButton_Click(object sender, RoutedEventArgs e)
        {
            ResetSettings();
        }

        private void CustomDictButton_Click(object sender, RoutedEventArgs e)
        {
            ResourceDictionary customResourceDictionary = new ResourceDictionary();
            customResourceDictionary.Source = new Uri(@"pack://application:,,,/Ab2d.ZoomControlSample;component/Resources/CustomNavigationResourceDictionary.xaml", UriKind.Absolute);

            // COMMENTED:
            // Adding ResourceDictionary to ZoomPanelNavigator does not work
            // Therefore it is needed to set the resources on UsedNavigationSlider and UsedNavigationCircle
            //ZoomPanelNavigator1.Resources.MergedDictionaries.Clear();
            //ZoomPanelNavigator1.Resources.MergedDictionaries.Add(customResourceDictionary);

            // Remove custom resource dictionaries if they were added before
            ClearCustomResources();

            ZoomPanelNavigator1.UsedNavigationSlider.Resources.MergedDictionaries.Add(customResourceDictionary);
            ZoomPanelNavigator1.UsedNavigationCircle.Resources.MergedDictionaries.Add(customResourceDictionary);
        }

        private void ResetSettings()
        {
            LightColorComboBox.SelectedIndex = 0;
            DarkColorComboBox.SelectedIndex = 0;
            SelectedFillColorComboBox.SelectedIndex = 0;
            SelectedContentColorComboBox.SelectedIndex = 0;

            // If custom resource dictionaries were used removed them
            ClearCustomResources();

            UpdateAllColors();
        }

        private void ClearCustomResources()
        {
            if (ZoomPanelNavigator1.UsedNavigationSlider.Resources.MergedDictionaries.Count == 2)
            {
                ZoomPanelNavigator1.UsedNavigationSlider.Resources.MergedDictionaries.RemoveAt(1);
                ZoomPanelNavigator1.UsedNavigationCircle.Resources.MergedDictionaries.RemoveAt(1);
            }
        }

        private void UpdateAllColors()
        {
            Color lightColor, darkColor, selectedColor, selectedContentColor;

            lightColor           = GetColorFromComboBox(LightColorComboBox);
            darkColor            = GetColorFromComboBox(DarkColorComboBox);
            selectedColor        = GetColorFromComboBox(SelectedFillColorComboBox);
            selectedContentColor = GetColorFromComboBox(SelectedContentColorComboBox);

            Ab2d.Common.ZoomPanel.CommonNavigationResources.Instance.SetMainColors(lightColor, darkColor, selectedColor, selectedContentColor);
        }

        private Color GetColorFromComboBox(ComboBox comboBox)
        {
            ComboBoxItem selectedComboBoxItem;
            Rectangle selectedRectangle;
            SolidColorBrush selectedBrush;

            selectedComboBoxItem = comboBox.SelectedItem as ComboBoxItem;

            selectedRectangle = selectedComboBoxItem.Content as Rectangle;

            selectedBrush = selectedRectangle.Fill as SolidColorBrush;

            return selectedBrush.Color;
        }

        private void ScaleComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            NavigatorScale.ScaleX = ScaleComboBox.SelectedIndex + 1;
            NavigatorScale.ScaleY = ScaleComboBox.SelectedIndex + 1;
        }

        private void HeightComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateNavigatorSize();
        }

        private void CurrentColorsButton_Click(object sender, RoutedEventArgs e)
        {
            if (ZoomPanelNavigator1.UsedNavigationSlider.Resources.MergedDictionaries.Count > 1)
            {
                MessageBox.Show("Custom resource dictionary is used");
            }
            else
            {
                string message;

                message = "Currently used colors:\r\n\r\n";

                FieldInfo[] constants = typeof(Ab2d.Common.ZoomPanel.CommonNavigationResources).GetFields(BindingFlags.Public | BindingFlags.Static);

                foreach (FieldInfo constant in constants)
                {
                    string colorKeyName;

                    colorKeyName = constant.GetValue(Ab2d.Common.ZoomPanel.CommonNavigationResources.Instance) as string;

                    message += string.Format("{0}: {1}\r\n", colorKeyName, Ab2d.Common.ZoomPanel.CommonNavigationResources.Instance.GetResourceColor(colorKeyName));
                }

                MessageBox.Show(message);
            }
        }

        private void OrientationRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (!this.IsLoaded)
                return;

            if (VerticalRadioButton.IsChecked ?? false)
            {
                HeightTextBlock.Text = "Navigator height:";

                UpdateNavigatorSize();
                ZoomPanelNavigator1.Orientation = Orientation.Vertical;
            }
            else
            {
                HeightTextBlock.Text = "Navigator width:";

                UpdateNavigatorSize();
                ZoomPanelNavigator1.Orientation = Orientation.Horizontal;
            }
        }

        private void UpdateNavigatorSize()
        {
            double newSize;

            if (HeightComboBox.SelectedIndex == 0)
            {
                newSize = double.NaN;
            }
            else
            {
                ComboBoxItem selectedComboBoxItem = HeightComboBox.SelectedItem as ComboBoxItem;
                newSize = double.Parse((string)selectedComboBoxItem.Content);
            }

            if (VerticalRadioButton.IsChecked ?? false)
            {
                ZoomPanelNavigator1.Height = newSize;
                ZoomPanelNavigator1.Width = double.NaN;
            }
            else
            {
                ZoomPanelNavigator1.Width = newSize;
                ZoomPanelNavigator1.Height = double.NaN;
            }
        }
    }
}
