using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AirMonitor.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void Help_Clicked(object sender, EventArgs e)
        {
            DisplayAlert("Tytuł dialogu", "elo", "elo");
            DisplayActionSheet("Tytul", "Anuluj", "destruction", "jeden", "dwa", "trzy");
        }

    }
}