using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace AirNZClone
{
    public partial class MyTripsCarousel : ContentPage
    {
        public MyTripsCarousel()
        {
            InitializeComponent();
            BindingContext = new MyTripsViewModel();
        }
    }
}
