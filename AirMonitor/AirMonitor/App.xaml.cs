using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using AirMonitor.Views;
using System.Runtime.Serialization;

namespace AirMonitor
{
    public partial class App : Application
    {
        public static SerializationInfo AirlyApiUrl { get; internal set; }

        public App()
        {
            InitializeComponent();

            //MainPage = new DetailsPage();
            //MainPage = new NavigationPage (new HomePage());
            MainPage = new NavigationPage (new TabbedPage1());

            
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
