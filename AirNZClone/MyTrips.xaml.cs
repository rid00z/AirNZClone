using System;
using System.Collections.Generic;
using System.Diagnostics;
using MR.Gestures;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace AirNZClone
{
    /*
     *
     * How I solved the problem?
     * -Complete understanding of the controls
     * -Especially around snaps
     * 
     * Ok so how does this work? 
     *
     * Initial State (assume width = 100):
     * Trip1 Offset = 0
     * Trip2 Offset = 100
     * Trip3 Offset = 200
     *
     * On a pan, we move the translations by the X.
     * Eg, the pan X becomes -50
     * This means everything is minus 50
     * 
     * If we get to the edge on the left we cycle and snap. On a snap
     * then offsets are changed to the snap.
     *
     * After snap State (assume width = 100):
     * Trip1 Offset = -100
     * Trip2 Offset = 0
     * Trip3 Offset = 100
     * 
     *
     * Problem: only want this to work for directions that have data.
     * How to snap to next one.
     * 
     */

    public class TripOffset
    {
        public Trip Trip { get; set; }
        public double Offset { get; set; }
        public Xamarin.Forms.View TripBackgroundImage { get; set; }
    }

    public partial class MyTrips : Xamarin.Forms.ContentPage
    {
        List<TripOffset> _trips;
        TripOffset _currentTrip;

        public MyTrips()
        {
            InitializeComponent();
            view2.BindingContext = new TripViewModel { CityImage = "housten.jpeg" };
            view1.BindingContext = new TripViewModel { CityImage = "auckland.jpg" };
            view3.BindingContext = new TripViewModel { CityImage = "sydney.jpg" };
            bgImage0.Source = "auckland.jpg";
            bgImage2.Source = "housten.jpeg";
            bgImage3.Source = "sydney.jpg"; 
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            //Set all defaults for Trip views
            _trips = new List<TripOffset>()
            {
                new TripOffset { Trip = view1, TripBackgroundImage = view0Image, Offset = 0 },
                new TripOffset { Trip = view2, TripBackgroundImage = view2Image, Offset = width },
                new TripOffset { Trip = view3, TripBackgroundImage = view3Image, Offset = width * 2 }
            };            
            _currentTrip = _trips[0];
            view1.CalculateOffsets(0, width, 0, false);
            bgImage0.Opacity = 1;
            view2.CalculateOffsets(width, width, width, false);
            bgImage2.Opacity = 1;
            view3.CalculateOffsets(width * 2, width, width * 2, false);
            bgImage3.Opacity = 1;
        }

        double _lastX;
        int counter = 0;
        bool disabled = false;

        public void PanUpdatedMR(object sender, PanEventArgs panEventArgs)
        {
            if (disabled) //If were vertical scrolling then ignore pan
                return;

            //Calculate percentage of pan and then set opacity on overlayt
            var percentage = Math.Abs(panEventArgs.TotalDistance.X / this.Width);
            whiteoverlay.Opacity = (percentage + .4).Clamp(.5, .9);

            counter++;
            if (counter == 3)
            {
                var totalDis = panEventArgs.TotalDistance;
                var isVertical = Math.Abs(totalDis.Y) > Math.Abs(totalDis.X);
                Debug.Write($"{isVertical}");
                if (isVertical)
                    disabled = true;
                else
                    scrollingContainer.IsEnabled = false;
            }

            _lastX = panEventArgs.TotalDistance.X;
            foreach (var trip in _trips)
            {
                trip.Trip.CalculateOffsets(trip.Offset + panEventArgs.TotalDistance.X, scrollingContainer.Width, trip.Offset, false);
            }
        }

        public void PanCompleted(object sender, PanEventArgs panEventArgs)
        {
            whiteoverlay.Opacity = .5; 

            scrollingContainer.IsEnabled = true;
            disabled = false;
            counter = 0;

            if (panEventArgs.Cancelled)
            {
                foreach (var trip in _trips)
                {
                    trip.Trip.CalculateOffsets(trip.Offset, scrollingContainer.Width, trip.Offset, false);
                }
            }

            var container = sender as View;

            var snapFromDistance = (Math.Abs(_lastX) + 20) > (container.Width / 2);
            var snapFromSpeed = Math.Abs(panEventArgs.Velocity.X) > 800;

            if (snapFromSpeed || snapFromDistance)
            {
                if (_lastX > 0) //show the one to the left
                {
                    if (_trips[0] != _currentTrip)
                    {
                        foreach (var trip in _trips)
                        {
                            trip.Offset = trip.Offset + container.Width;
                        }
                    }
                }
                else
                {
                    if (_trips[_trips.Count - 1] != _currentTrip)
                    {
                        foreach (var trip in _trips)
                        {
                            trip.Offset = trip.Offset - container.Width;
                        }
                    }
                }
            }

            //Go through the trips and move into default positions. 
            foreach (var trip in _trips)
            {
                if (trip.Offset == 0)
                {
                    _currentTrip = trip;
                    trip.TripBackgroundImage.Opacity = 1;
                }
                else
                {
                    trip.TripBackgroundImage.Opacity = 0;
                }

                trip.Trip.CalculateOffsets(trip.Offset, scrollingContainer.Width, trip.Offset, true);
            }
        }

    }
}
