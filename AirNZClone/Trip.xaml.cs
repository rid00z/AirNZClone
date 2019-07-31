using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace AirNZClone
{
    public partial class Trip : ContentView
    {
        public Trip()
        {
            InitializeComponent();
        }

        public View CircleImageContainer
        {
            get { return circleImageContainer; }
        }

        Easing SpringIn = new Easing((double x) => x* x * (2.7015800476074219 * x - 1.7015800476074219));
		Easing SpringOut = new Easing((double x) => (x - 1.0) * (x - 1.0) * (3.7015800476074219 * (x - 1.0) + 0.7015800476074219) + 1.0);
         
        /// <summary>
        /// So what we want to happen is that if this trip is to the left or to the right of screen
        /// then the circle image seen within the users view.
        ///
        /// Assuming:
        /// the width of screen is 500
        /// the size of the image is 200
        /// current offset of image is (500-200) / 2 = (-)150
        /// 
        /// In the case the circle image to the left
        /// then the offset of full Trip is -500
        /// then image needs to be 50% on the other users screen
        /// (imagewidth/2) + (image offset) = (200/2) + ((500-200)/2) = 250
        /// then as the image moves further onto the screen to change needs to move into
        /// the offset will move to for eg -450 then the image offset need to decrease
        /// 
        /// inputs (
        /// 
        /// Ok considering the width and 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="containerWidth"></param>
        /// <param name="parentCurrentStartingOffset"></param>
        public void CalculateOffsets(double x, double containerWidth, double parentCurrentStartingOffset, bool animate)
        {
            TranslationX = x;
            bool isNeighbourOfCentre = Math.Abs(parentCurrentStartingOffset) == containerWidth;

            if (isNeighbourOfCentre)
            {
                var offsetOfImage = (containerWidth - circleImageContainer.Width) / 2;
                var halfOfImage = circleImageContainer.Width / 2;
                var inverseParentCurrentStartingOffset = parentCurrentStartingOffset * -1;
                double finalOffset = 0;
                double normalizedTime = 0;
                double percentageMoved = 0;
                if (parentCurrentStartingOffset < 0)
                {
                    var offset = Math.Abs(x) - halfOfImage;
                    var maxOffset = (inverseParentCurrentStartingOffset - halfOfImage);
                    finalOffset = offset.Clamp(0, maxOffset);

                    percentageMoved = (finalOffset / maxOffset);
                    normalizedTime = Ease(percentageMoved);
                    finalOffset = normalizedTime * (maxOffset - offsetOfImage);
                    
                    Debug.WriteLine($"normalizedTime{normalizedTime}");
                }
                else
                {
                    var offset = offsetOfImage + halfOfImage - x;
                    finalOffset = offset.Clamp((halfOfImage+offsetOfImage)*-1 + 50, 0);

                    percentageMoved = -1 * (finalOffset / (halfOfImage + offsetOfImage));
                    normalizedTime = Ease(percentageMoved);
                    finalOffset = normalizedTime * (halfOfImage + offsetOfImage) * -1;
                }

                var makeToPointFiveScale = (.5 * (1 - percentageMoved));
                var scale = makeToPointFiveScale + .5;

if (animate)
{
    circleImageContainer.TranslateTo(finalOffset, 0, 250, Easing.SinOut);
    circleImageContainer.ScaleTo(scale, 250, Easing.SinOut);
}
else
{
    circleImageContainer.Scale = scale;
    circleImageContainer.TranslationX = finalOffset;
}
                
            }
            else
            {
                circleImageContainer.Scale = 1;
                circleImageContainer.TranslationX = 0;
            }
        }

        double Ease(double normalizedTime)
        {
            normalizedTime = 1.0 - normalizedTime;
            normalizedTime = Math.Max(0.0, Math.Min(1.0, normalizedTime));
            normalizedTime = 1.0 - Math.Sqrt(1.0 - normalizedTime * normalizedTime);
            return 1.0 - normalizedTime;
        }
    }
}
