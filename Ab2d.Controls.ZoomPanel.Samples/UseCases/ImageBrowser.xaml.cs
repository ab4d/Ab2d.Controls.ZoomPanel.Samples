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
    /// Interaction logic for ImageBrowser.xaml
    /// </summary>
    public partial class ImageBrowser : Page
    {
        private const int MAX_IMAGE_FILE_NAMES = 100;

        private List<string> _imagesFolders;
        private List<string> _imageFileNames;
        private int _imageIndex;

        public ImageBrowser()
        {
            InitializeComponent();

            _imageIndex = -1;

            _imagesFolders = new List<string>();
            _imagesFolders.Add(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures));
            _imagesFolders.Add(@"C:\Users\Public\Pictures\Sample Pictures");
            _imagesFolders.Add(@"C:\Documents and Settings\All Users\Documents\My Pictures\Sample Pictures");
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            CollectImageFileNames();

            if (_imageFileNames == null || _imageFileNames.Count == 0) // No images found in the samples pictures
                SelectImagesFolder();

            UpateImageInfo();
            ShowCurrentImage();
        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            MoveToPrevious();
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            MoveToNext();
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            SelectImagesFolder();
        }

        private void SelectImagesFolder()
        {
            string selectedFolder;

            selectedFolder = SelectDirectory();

            _imagesFolders = new List<string>();
            _imagesFolders.Add(selectedFolder);

            CollectImageFileNames();
        }

        private void MoveToPrevious()
        {
            if (_imageIndex > 0)
            {
                _imageIndex--;
                UpateImageInfo();
                ShowCurrentImage();
            }
        }

        private void MoveToNext()
        {
            if (_imageIndex < _imageFileNames.Count - 1)
            {
                _imageIndex++;
                UpateImageInfo();
                ShowCurrentImage();
            }
        }

        private void UpateImageInfo()
        {
            if (_imageIndex == -1)
            {
                PreviousButton.IsEnabled = false;
                NextButton.IsEnabled = false;

                InfoTextBlock.Text = "No images";
            }
            else
            {
                if (_imageIndex == 0)
                {
                    PreviousButton.IsEnabled = false;
                    NextButton.IsEnabled = true;
                }
                else if (_imageIndex == _imageFileNames.Count - 1)
                {
                    PreviousButton.IsEnabled = true;
                    NextButton.IsEnabled = false;
                }
                else
                {
                    PreviousButton.IsEnabled = true;
                    NextButton.IsEnabled = true;
                }

                InfoTextBlock.Text = string.Format("Image {0:#,##0} / {1:#,##0}", _imageIndex + 1, _imageFileNames.Count);
            }
        }

        private void ShowCurrentImage()
        {
            string imageFileName;

            if (_imageIndex == -1)
            {
                Image1.Source = null;
                return;
            }

            ZoomPanel1.ResetNow(); // No animation

            imageFileName = _imageFileNames[_imageIndex];
            Image1.Source = new BitmapImage(new Uri(imageFileName));
        }

        private void CollectImageFileNames()
        {
            _imageFileNames = new List<string>(); // create an empty array

            foreach (string imagesFolder in _imagesFolders)
            {
                CollectImageFileNames(imagesFolder);

                if (_imageFileNames.Count > MAX_IMAGE_FILE_NAMES)
                    break;
            }

            OpenButton.IsEnabled = true;
            MessageGrid.Visibility = Visibility.Collapsed;

            if (_imageFileNames.Count > 0)
                _imageIndex = 0;
            else
                _imageIndex = -1;

            UpateImageInfo();
            ShowCurrentImage();
        }

        private void CollectImageFileNames(string directoryName)
        {
            string[] allNames;

            if (!System.IO.Directory.Exists(directoryName)) // if folder does not exits skip it
                return;

            allNames = System.IO.Directory.GetFiles(directoryName, "*.jpg");

            _imageFileNames.AddRange(allNames);

            if (_imageFileNames.Count < MAX_IMAGE_FILE_NAMES)
            {
                allNames = System.IO.Directory.GetDirectories(directoryName);

                foreach (string oneDirectoryName in allNames)
                {
                    CollectImageFileNames(oneDirectoryName);

                    if (_imageFileNames.Count > MAX_IMAGE_FILE_NAMES)
                        break;
                }
            }
        }

        private string SelectDirectory()
        {
            string folder;

            // Uh. No Folder Browser Dialog in WPF - use WinForms instead
            System.Windows.Forms.FolderBrowserDialog folderDialog;
            System.Windows.Forms.DialogResult result;

            folderDialog = new System.Windows.Forms.FolderBrowserDialog();
            folderDialog.Description = "Select images folder";

            result = folderDialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
                folder = folderDialog.SelectedPath;
            else
                folder = null;

            return folder;
        }
    }
}
