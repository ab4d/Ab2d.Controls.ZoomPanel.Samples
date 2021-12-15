using System;
using System.Collections.Generic;
using System.Text;
using Ab2d.Animations;
using System.Windows;

namespace Ab2d.ZoomControlSample.CustomAnimations
{
    public class ZoomPanelZoomOutAnimator : ZoomPanelQuinticAnimator
    {
        public double ZoomOutFactor { get; set; }

        public ZoomPanelZoomOutAnimator()
        {
            ZoomOutFactor = 0.5;
        }

        /// <summary>
        /// Calculates the new Viewbox and RotationAngle values from the properties.
        /// </summary>
        /// <param name="startViewbox">animation start Viewbox value as Rect</param>
        /// <param name="endViewbox">animation end Viewbox value as Rect</param>
        /// <param name="startRotationAngle">animation start RotationAngle</param>
        /// <param name="endRotationAngle">animation end RotationAngle</param>
        /// <param name="progress">progress of animation from 0 to 1</param>
        /// <param name="newViewbox">new Viewbox value</param>
        /// <param name="newRotationAngle">new RotationAngle value</param>
        public override void CalculateViewboxAndRotationAngle(Rect startViewbox, Rect endViewbox, double startRotationAngle, double endRotationAngle, double progress, out Rect newViewbox, out double newRotationAngle)
        {
            if (startViewbox == endViewbox)
            {
                // Viewbox is not animated
                newViewbox = startViewbox;
            }
            else
            {
                // Interpolate the Viewbox
                double x, y, w, h;

                x = CalculateValue(startViewbox.X, endViewbox.X, progress);
                y = CalculateValue(startViewbox.Y, endViewbox.Y, progress);
                w = CalculateValue(startViewbox.Width, endViewbox.Width, progress);
                h = CalculateValue(startViewbox.Height, endViewbox.Height, progress);

                double averageWidth = (startViewbox.Width + endViewbox.Width) / 2;
                double averageHeight = (startViewbox.Height + endViewbox.Height) / 2;

                double sizeAdjustFactor;

                // we need sin values from -PI/2 to 3*PI/2
                // Normalize the sin function:
                // at progress = 0   => sizeAdjustFactor = 0 
                // at progress = 0.5 => sizeAdjustFactor = 1
                // at progress = 1   => sizeAdjustFactor = 0
                sizeAdjustFactor = ((Math.Sin((progress * Math.PI * 2) - (Math.PI / 2))) + 1) / 2;

                double widthAdjust = sizeAdjustFactor * averageWidth * ZoomOutFactor;
                double heightAdjust = sizeAdjustFactor * averageHeight * ZoomOutFactor;

                x -= widthAdjust / 2;
                y -= heightAdjust / 2;
                w += widthAdjust;
                h += heightAdjust;

                // IMPORTANT:
                // We need to prevent setting negative width and height to Rect!
                // In this case we need to set Width or Height to 0.
                // Because 0 is also not a correct value for Width and Height (as it can lead to devide by zero),
                // the code in ZoomPanel will check for 0 values in Width and Height and in this case use the current value for Width and Height.
                if (w < 0)
                    w = 0;

                if (h < 0)
                    h = 0;

                newViewbox = new Rect(x, y, w, h);
            }

            if (startRotationAngle == endRotationAngle)
            {
                // RotationAngle is not animated
                newRotationAngle = startRotationAngle;
            }
            else
            {
                // Interpolate the RotationAngle
                newRotationAngle = CalculateValue(startRotationAngle, endRotationAngle, progress);
            }
        }

        private double GetSin(double progress)
        {
            // we need sin values from -PI/2 to 3*PI/2

            return Math.Sin((progress * Math.PI * 2) - (Math.PI / 2));
        }
    }
}
