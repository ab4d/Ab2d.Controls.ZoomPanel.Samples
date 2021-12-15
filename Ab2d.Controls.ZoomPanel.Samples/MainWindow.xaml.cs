using System.Diagnostics;
using System.Windows.Controls;
using System.Windows;
using System;
using System.Linq;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;

namespace Ab2d.ZoomControlSample
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            // When using the ZoomPanel from NuGet and when you have purchase a commercial version,
            // then uncomment the following line to activate the license uncomment (see your User Account web page for more info):
            //Ab2d.Licensing.ZoomPanel.LicenseHelper.SetLicense(licenseOwner: "[CompanyName]", 
            //                                                  licenseType: "[LicenseType]", 
            //                                                  license: "[LicenseText]");

            InitializeComponent();

            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
        }

        public void ShowSupportPage()
        {
            var supportPageElement = SampleList.Items.OfType<System.Xml.XmlElement>()
                                                     .First(x => x.Attributes["Page"] != null && x.Attributes["Page"].Value == "Other/SupportPage.xaml");

            SampleList.SelectedItem = supportPageElement;
            SampleListScrollViewer.ScrollToEnd();
        }

        private void TextBlock_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            System.Xml.XmlNode node = e.NewValue as System.Xml.XmlNode;

            System.Xml.XmlAttribute attribute = node.Attributes["Description"];

            if (attribute == null)
            {
                DescriptionTextBlock.Text = "";
                return;
            }

            string description = attribute.Value;

            if (!string.IsNullOrEmpty(description))
            {
                DescriptionTextBlock.BeginInit();
                DescriptionTextBlock.Inlines.Clear();

                int pos1, pos2;
                bool isBold = false;
                string part = "";
                char command;

                pos1 = 0;
                while (pos1 != -1 || pos1 > description.Length - 1)
                {
                    pos2 = description.IndexOf('\\', pos1);

                    if (pos2 == -1)
                    {
                        part = description.Substring(pos1);
                        break;
                    }

                    part = description.Substring(pos1, pos2 - pos1);
                    command = description[pos2 + 1]; // char after '\'

                    Run run = new Run(part);
                    if (isBold)
                        run.FontWeight = FontWeights.Bold;

                    DescriptionTextBlock.Inlines.Add(run);

                    if (command == 'n') // NewLine
                        DescriptionTextBlock.Inlines.Add(new System.Windows.Documents.LineBreak());
                    else if (command == 'b') // Toggle bold
                        isBold = !isBold;

                    pos1 = pos2 + 2;
                }

                if (!string.IsNullOrEmpty(part))
                    DescriptionTextBlock.Inlines.Add(part);

                DescriptionTextBlock.EndInit();
            }
            else
            {
                DescriptionTextBlock.Text = "";
            }
        }

        private void LogoImage_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            Process.Start("https://www.ab4d.com");
        }

        private void ContentFrame_OnNavigated(object sender, NavigationEventArgs e)
        {
            // Prevent navigation (for example clicking back button) because our ListBox is not updated when this navigation occurs
            // We prevent navigation with clearing the navigation history each time navigation item changes
            ContentFrame.NavigationService.RemoveBackEntry();
        }
    }
}