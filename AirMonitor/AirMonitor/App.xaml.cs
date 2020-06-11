using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using AirMonitor.Views;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System.Linq;


namespace AirMonitor
{
    public partial class App : Application
    {
        public static string ApiKey { get; private set; }
        public static string ApiUrl { get; private set; }
        public static string ApiMeasurementUrl { get; private set; }
        public static string ApiInstallationUrl { get; private set; }

        public App()
        {
            InitializeComponent();

            //MainPage = new NavigationPage (new TabbedPage1());

            InitializeApp();

        }

        private async Task InitializeApp()
        {
            await LoadConfig();

            MainPage = new NavigationPage(new TabbedPage1());
        }
        private static async Task LoadConfig()
        {
            var assembly = Assembly.GetAssembly(typeof(App));
            var resourceNames = assembly.GetManifestResourceNames();
            var jsonName = resourceNames.FirstOrDefault(s => s.Contains("config.json"));

            using (var stream = assembly.GetManifestResourceStream(jsonName))
            {
                using (var reader = new StreamReader(stream))
                {
                    var json = await reader.ReadToEndAsync();
                    var parsingJson = JObject.Parse(json);

                    ApiKey = parsingJson["ApiKey"].Value<string>();
                    ApiUrl = parsingJson["ApiUrl"].Value<string>();
                    ApiMeasurementUrl = parsingJson["ApiMeasurementUrl"].Value<string>();
                    ApiInstallationUrl = parsingJson["ApiInstallationUrl"].Value<string>();
                }
            }
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
