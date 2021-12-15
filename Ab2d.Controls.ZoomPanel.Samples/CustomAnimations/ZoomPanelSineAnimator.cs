using System;
using System.Collections.Generic;
using System.Text;
using Ab2d.Animations;

namespace Ab2d.ZoomControlSample.CustomAnimations
{
    public class ZoomPanelSineAnimator : BaseZoomPanelAnimator
    {
        // When creating a custom animator the CalculateValue method must be overriden.
        // It provides the logic to calculate the values between start and end value.
        // If additional customization is needed it is possible to override the CalculateViewboxAndRotationAngle method.

        /// <summary>
        /// Gets a value that is calculated from startValue, endValue and progress.
        /// </summary>
        /// <param name="startValue">start value</param>
        /// <param name="endValue">end value</param>
        /// <param name="progress">progress value from 0 to 1</param>
        /// <returns>calculated value</returns>
        public override double CalculateValue(double startValue, double endValue, double progress)
        {
            double middleValue = (endValue - startValue) / 2;

            if (progress < 0.5)
                return (1 - Math.Cos(progress * Math.PI)) * middleValue + startValue;
            else
                return Math.Sin((progress - 0.5) * Math.PI) * middleValue + startValue + middleValue;
        }
    }
}
