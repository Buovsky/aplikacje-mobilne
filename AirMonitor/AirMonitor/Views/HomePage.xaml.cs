using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using AirMonitor.ViewModels;
using Xamarin.Essentials;


namespace AirMonitor.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePage : ContentPage
    {
        public HomePage()
        {
            InitializeComponent();

            BindingContext = new HomeViewModel(Navigation);


        }

        private async void Localization_Clicked(object sender, EventArgs e)
        {
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.High);
                var location = await Geolocation.GetLastKnownLocationAsync();


                await DisplayAlert("Lokalizacja graficzna", $"Szerokość graficzna: {location.Latitude}\nDługość graficzna: {location.Longitude}\nWysokość graiczna: {location.Altitude}", "Zamknij");


            }
            catch
            {
                // Unable to get location
            }

        }
    }
}