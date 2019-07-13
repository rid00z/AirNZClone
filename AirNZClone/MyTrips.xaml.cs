using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xamarin.Forms;

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
    }

    public partial class MyTrips : ContentPage
    {
        List<TripOffset> _trips;
        TripOffset _currentTrip;

        public MyTrips()
        {
            InitializeComponent();
            //BindingContext = new MyTripsViewModel();
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            _trips = new List<TripOffset>()
            {
                new TripOffset { Trip = view1, Offset = 0 },
                new TripOffset { Trip = view2, Offset = width },
                new TripOffset { Trip = view3, Offset = width * 2 }
            };
            _currentTrip = _trips[0];
            view1.TranslationX = 0;
            view2.TranslationX = width;
            view3.TranslationX = width * 2;

            view1.BindingContext = new TripViewModel { CityImage = "auckland.jpg" };
            view2.BindingContext = new TripViewModel { CityImage = "housten.jpeg" };
            view3.BindingContext = new TripViewModel { CityImage = "sydney.jpg" };
        }

        double _lastX;
        public void PanUpdated(object sender, PanUpdatedEventArgs args)
        {
            var container = sender as View;

            if (args.StatusType == GestureStatus.Running
                || args.StatusType == GestureStatus.Started)
            {
                _lastX = args.TotalX;
                foreach (var trip in _trips)
                {
                    trip.Trip.TranslationX = trip.Offset + args.TotalX;
                }
            }
            else if (args.StatusType == GestureStatus.Completed)
            {                
                Debug.WriteLine($"Completed {container.Width} {_lastX} {_currentTrip.Trip.TranslationX}");
                if ((Math.Abs(_lastX)+20) > (container.Width / 2))
                {
                    Debug.WriteLine($"Snap");
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
                        if (_trips[_trips.Count-1] != _currentTrip)
                        {
                            foreach (var trip in _trips)
                            {
                                trip.Offset = trip.Offset - container.Width;
                            }
                        }
                    }
                }
                 
                foreach (var trip in _trips)
                {
                    if (trip.Offset == 0)
                        _currentTrip = trip;
                    trip.Trip.TranslateTo(trip.Offset, 0, 250, Easing.SinOut);
                }
            }
            else if (args.StatusType == GestureStatus.Canceled)
            {
                foreach (var trip in _trips)
                {
                    trip.Trip.TranslationX = trip.Offset;
                }
            }
        }

        public void Swiped(object sender, SwipedEventArgs args)
        {
            Debug.Write($"Swiped {args.Direction}");
        }

        //public void Scrolled(object sender, ScrolledEventArgs args)
        //{
        //    Debug.WriteLine($"Scrolled {args.ScrollX} {args.ScrollY} ");
        //}

    }
}
