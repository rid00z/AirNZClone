using System;
using System.Collections.Generic;

namespace AirNZClone
{
    public class MyTripsViewModel
    {
        public MyTripsViewModel()
        {
        }

        public List<TripViewModel> Trips { get; set; } = new List<TripViewModel>()
        {
            new TripViewModel { CityImage = "auckland.jpg" },
            new TripViewModel { CityImage = "housten.jpeg" },
            new TripViewModel { CityImage = "sydney.jpg" }
        };

    }
}
