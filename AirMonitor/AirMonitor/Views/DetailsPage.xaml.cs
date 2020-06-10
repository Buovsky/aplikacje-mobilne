using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using AirMonitor.Models;
using AirMonitor.ViewModels;

namespace AirMonitor.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DetailsPage : ContentPage
    {
        public DetailsPage(Measurement item)
        {
            InitializeComponent();

            var viewModel = BindingContext as DetailsViewModel;
            viewModel.Item = item;
        }

        private void Help_Clicked(object sender, EventArgs e)
        {
            DisplayAlert("Czym jest CAQI?", "CAQI (Wspólny Indeks Jakości Powietrza) jest liczbą w skali od 1 do 100, " +
                "gdzie niska wartość oznacza dobrą jakość powietrza oraz wysoka wartość oznacza złą jakość powietrza.", "Zamknij");
            //DisplayActionSheet("Tytul", "Anuluj", "destruction", "jeden", "dwa", "trzy");
        }
    }
}