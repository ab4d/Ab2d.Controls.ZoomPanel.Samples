using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Ab2d.ZoomControlSample.Common
{
    public class SampleGrid : Canvas
    {
        public int WidthCellsCount { get; set; }
        public int HeightCellsCount { get; set; }

        public SampleGrid()
            : base()
        {
            this.Width = 100;
            this.Height = 100;

            this.WidthCellsCount = 10;
            this.HeightCellsCount = 10;
        }

        protected override System.Windows.Size MeasureOverride(System.Windows.Size constraint)
        {
            if (this.Children.Count == 0)
                CreateChildControls();
        
            return base.MeasureOverride(constraint);
        }

        public void Recreate()
        {
            CreateChildControls();
        }

        private void CreateChildControls()
        {
            TextBlock textBlock;

            this.Children.Clear();

            double halfLineSize = ((this.Width + this.Height) / 2) / 60;

            Border border = new Border();
            border.BorderThickness = new System.Windows.Thickness(halfLineSize);
            border.BorderBrush = Brushes.Black;
            border.Width = this.Width;
            border.Height = this.Height;

            this.Children.Add(border);


            textBlock = new TextBlock();
            textBlock.FontSize = Math.Min(this.Width, this.Height) / 10;
            textBlock.FontFamily = new FontFamily("Arial Black");
            textBlock.Foreground = Brushes.LightGray;
            textBlock.Text = string.Format("Size: {0}x{1}", this.Width, this.Height);
            Canvas.SetLeft(textBlock, halfLineSize * 2);
            Canvas.SetTop(textBlock, halfLineSize);
            this.Children.Add(textBlock);


            if (WidthCellsCount < 2 || HeightCellsCount < 2)
                return;

            double xStep = this.Width / WidthCellsCount;
            double yStep = this.Height / HeightCellsCount;
            

            double xPos, yPos;

            xPos = xStep;
            for (int x = 1; x < WidthCellsCount; x++)
            {
                yPos = yStep;
                for (int y = 1; y < HeightCellsCount; y++)
                {
                    Line line;

                    line = new Line();
                    line.X1 = xPos;
                    line.X2 = xPos;
                    line.Y1 = yPos - halfLineSize;
                    line.Y2 = yPos + halfLineSize;
                    line.StrokeThickness = halfLineSize / 4;
                    line.Stroke = Brushes.Black;
                    this.Children.Add(line);

                    line = new Line();
                    line.X1 = xPos - halfLineSize;
                    line.X2 = xPos + halfLineSize;
                    line.Y1 = yPos;
                    line.Y2 = yPos;
                    line.StrokeThickness = halfLineSize / 4;
                    line.Stroke = Brushes.Black;
                    this.Children.Add(line);

                    textBlock = new TextBlock();
                    textBlock.FontSize = halfLineSize * 1.5;
                    textBlock.FontFamily = new FontFamily("Arial");
                    textBlock.Text = string.Format("{0} {1}", x, y);
                    Canvas.SetLeft(textBlock, xPos + halfLineSize / 2);
                    Canvas.SetTop(textBlock, yPos - 2 * halfLineSize);
                    this.Children.Add(textBlock);


                    yPos += yStep;
                }

                xPos += xStep;
            }
        }
    }
}
