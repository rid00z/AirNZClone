using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace AirNZClone
{
    public partial class MyTripsCollection : ContentPage
    {
        public MyTripsCollection()
        {
            InitializeComponent();
            BindingContext = new MyTripsViewModel();
        }
    }
}
