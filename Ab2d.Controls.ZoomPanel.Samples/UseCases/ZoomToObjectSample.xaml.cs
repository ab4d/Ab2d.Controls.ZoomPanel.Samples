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
using Ab2d.Animations;
using Ab2d.ZoomControlSample.CustomAnimations;

namespace Ab2d.ZoomControlSample.UseCases
{
    /// <summary>
    /// Interaction logic for ZoomToObjectSample.xaml
    /// </summary>
    public partial class ZoomToObjectSample : Page
    {
        private FrameworkElement _selectedObject;

        private BaseZoomPanelAnimator _defaultAnimator;
        private ZoomPanelZoomOutAnimator _customZoomOutAnimator;

        public ZoomToObjectSample()
        {
            InitializeComponent();

            // This sample use custom animator that when going from one object to another it zooms out
            _defaultAnimator = ZoomPanel1.ZoomPanelAnimator;
            _customZoomOutAnimator = new ZoomPanelZoomOutAnimator();

            this.Loaded += new RoutedEventHandler(ZoomToObjectSample_Loaded);
        }

        void ZoomToObjectSample_Loaded(object sender, RoutedEventArgs e)
        {
            FillObjects();
            _customZoomOutAnimator.ZoomOutFactor = GetComboBoxValue(ZoomOutComboBox);
        }

        private void ObjectsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FrameworkElement targetObject;

            if (!this.IsLoaded)
                return;

            if (ObjectsListBox.SelectedIndex == 0)
                targetObject = null; // Show all objects
            else
                targetObject = ((ListBoxItem)ObjectsListBox.SelectedItem).Tag as FrameworkElement;

            ZoomToObject(targetObject);
        }

        private void MarginComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_selectedObject == null)
                return;

            // Zoom to the same object with changed margin
            ZoomToObject(_selectedObject);
        }


        private void NewShape_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Because this event handler is subscribed to our shape, we should handle this event so it does not get passed on to ZoomPanel (we do not want that two actions occur on one mouse event)
            e.Handled = true;
        }

        void NewShape_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // Because shapes do not support click event, we just subscribed to MouseLeftButtonUp
            // To get better reproduction of click event, we would have to subscribe to MouseLeftButtonDown too (here we should mark the selected object and check in the MouseLeftButtonUp if it is the same). 
            // But for this sample the MouseLeftButtonUp is enough

            Shape clickedShape = sender as Shape;
            FrameworkElement newSelectedShape;

            if (clickedShape == null)
                return;

            if (clickedShape == _selectedObject)
            {
                // Click on the selected object again - show all objects
                newSelectedShape = null;
            }
            else
            {
                newSelectedShape = clickedShape;
            }

            // ZoomToObject will be called when the ListBox item is changed
            // ZoomToObject(newSelectedShape);

            SelectListBoxItem(newSelectedShape);

            // Because this event handler is subscribed to our shape, we should handle this event so it does not get passed on to ZoomPanel (we do not want that two actions occur on one mouse event)
            e.Handled = true;
        }

        private void ZoomOutComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!this.IsLoaded)
                return;

            _customZoomOutAnimator.ZoomOutFactor = GetComboBoxValue(ZoomOutComboBox);
        }



        private void ZoomToObject(FrameworkElement targetObject)
        {
            if (targetObject == null)
            {
                // When zooming from all object to one object we use default animation
                // Default animation is also used when zooming from one object to all objects
                ZoomPanel1.ZoomPanelAnimator = _defaultAnimator;

                // Show all objects
                ZoomPanel1.Reset();
                _selectedObject = null;

                return;
            }


            // First get the position of the selected object
            // For our simple sample we can also use Canvas.GetLeft and Canvas.GetTop
            // But for more complex scenarios where the object is inside many parents with transformations we need to use TransformToAncestor
            GeneralTransform transform = targetObject.TransformToAncestor(ContentCanvas);
            Point objectsPosition = transform.Transform(new Point(0, 0));

            // Get the size of object
            Size objectsSize = new Size(targetObject.ActualWidth, targetObject.ActualHeight);

            // We will zoom to the center of the object
            Point centerPosition = new Point(objectsPosition.X + objectsSize.Width / 2, objectsPosition.Y + objectsSize.Height / 2);

            // Now get the required zoom level (1 = no zoom, 2 = 200% zoom, etc.)
            double zoomX, zoomY;
            double zoomFactor;

            double margin = GetComboBoxValue(MarginComboBox);

            // get zoom factor for width and height of the object
            zoomX = ContentCanvas.ActualWidth / (objectsSize.Width + margin);
            zoomY = ContentCanvas.ActualHeight / (objectsSize.Height + margin);

            // Get min value (so the whole object is shown) and adjust it for the selected margin
            zoomFactor = Math.Min(zoomX, zoomY);

            // Set the new zoom position and zoom factor
            ZoomPanel1.SetZoom(centerPosition, Controls.ZoomPanel.CenterPositionUnitsType.Absolute, zoomFactor);

            if (_selectedObject != null)
            {
                // When zooming from one object to another object we use custom animation (if UseCustomAnimaitonCheckBox is checked)
                if (UseCustomAnimaitonCheckBox.IsChecked ?? false)
                    ZoomPanel1.ZoomPanelAnimator = _customZoomOutAnimator;
                else
                    ZoomPanel1.ZoomPanelAnimator = _defaultAnimator;
            }

            _selectedObject = targetObject;
        }

        private double GetComboBoxValue(ComboBox comboBox)
        {
            double comboBoxValue;

            ComboBoxItem item = comboBox.SelectedItem as ComboBoxItem;

            if (item == null)
                comboBoxValue = 0;
            else
                comboBoxValue = double.Parse((string)item.Content);

            return comboBoxValue;
        }

        // Randomly add some objects
        private void FillObjects()
        {
            Shape newShape;
            string shapeName;
            ListBoxItem item;
            Random rnd = new Random();

            ContentCanvas.Children.Clear();
            ObjectsListBox.Items.Clear();

            // Add special object to show all objects (zoom out)
            item = new ListBoxItem();
            item.Content = "Show all objects";
            item.IsSelected = true;
            ObjectsListBox.Items.Add(item);


            Color[] colors = new Color[] { Colors.Red, Colors.Blue, Colors.Green, Colors.Orange, Colors.Yellow };
            string[] colorNames = new string[] { "Red", "Blue", "Green", "Orange", "Yellow" };

            for (int c = 0; c < colors.Length; c++ )
            {
                Color color = colors[c];

                for (int i = 0; i < 3; i++)
                {
                    double x, y;
                    double width, height;

                    // Get random position and size
                    width = 50 + rnd.NextDouble() * 100;
                    height = 20 + rnd.NextDouble() * 50;

                    // Use ZoomPanel1's actual size because Canvas size is set at the end of this method
                    x = 20 + rnd.NextDouble() * (ZoomPanel1.ActualWidth - 40 - width);
                    y = 20 + rnd.NextDouble() * (ZoomPanel1.ActualHeight - 40 - height);

                    switch (i)
                    {
                        case 0:
                        default:
                            newShape = new Rectangle();
                            shapeName = "Rectangle";
                            break;

                        case 1:
                            Rectangle newRectangle = new Rectangle();
                            shapeName = "Rounded Rectangle";
                            newRectangle.RadiusX = 5;
                            newRectangle.RadiusY = 5;
                            newShape = newRectangle;
                            break;

                        case 2:
                            newShape = new Ellipse();
                            shapeName = "Ellipse";
                            break;
                    }

                    newShape.Width = width;
                    newShape.Height = height;
                    newShape.Opacity = 0.8;
                    newShape.Fill = new SolidColorBrush(color);
                    newShape.Stroke = Brushes.Black;
                    newShape.StrokeThickness = 1;

                    newShape.Cursor = Cursors.Hand;

                    // Because ZoomPanel is in Move mode, it is subscribed to MouseLeftButtonDown and MouseLeftButtonUp (and some other events).
                    // This means that in mouse events will be handled by ZoomPanel and we will not be notified if we would also be subscribed to those events.
                    // But we can be notified about mouse event before ZoomPanel if you use PreviewMouse...events.
                    // In case of using Preview events, we need to set the Handled property of the MouseButtonEventArgs (get as parameter into event handler) to true
                    // in case when you handled the event by yourself - this means that the mouse down, up or some other event was used by your code
                    // and should not be used by some other subscriber - in this case ZoomPanel.
                    // This way, you can control which mouse down and up events are passed through ZoomPanel - those that are not handled by your code.
                    newShape.PreviewMouseLeftButtonDown += NewShape_MouseLeftButtonDown;
                    newShape.PreviewMouseLeftButtonUp += NewShape_MouseLeftButtonUp;

                    Canvas.SetLeft(newShape, x);
                    Canvas.SetTop(newShape, y);

                    ContentCanvas.Children.Add(newShape);

                    item = new ListBoxItem();
                    item.Tag = newShape;
                    item.Content = string.Format("{0} {1}", colorNames[c], shapeName);

                    ObjectsListBox.Items.Add(item);
                }
            }

            // Without setting the size of the Canvas its content would not be visible
            // Use ZoomPanel1's actual size because it is already set in Loaded event
            ContentCanvas.Width = ZoomPanel1.ActualWidth;
            ContentCanvas.Height = ZoomPanel1.ActualHeight;

            ObjectsListBox.Focus();
        }

        private void SelectListBoxItem(FrameworkElement selectedShape)
        {
            int selectedIndex = 0;

            if (selectedShape == null)
            {
                // Show all objects
                selectedIndex = 0;
            }
            else
            {
                for (int i = 0; i < ObjectsListBox.Items.Count; i++)
                {
                    if (((ListBoxItem)ObjectsListBox.Items[i]).Tag == selectedShape)
                    {
                        selectedIndex = i;
                        break;
                    }
                }
            }

            ObjectsListBox.SelectedIndex = selectedIndex;
        }
    }
}
