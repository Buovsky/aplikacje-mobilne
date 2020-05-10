using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using AirMonitor.Views;

namespace AirMonitor
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            //MainPage = new DetailsPage();
            MainPage = new NavigationPage (new HomePage());
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
