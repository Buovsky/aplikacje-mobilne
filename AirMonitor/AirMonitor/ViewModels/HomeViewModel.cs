using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using AirMonitor.Views;
using Xamarin.Forms;
using Xamarin.Essentials;
using System.Globalization;
using System.Web;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using AirMonitor.Models;

namespace AirMonitor.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        private readonly INavigation _navigation;

        public HomeViewModel(INavigation navigation)
        {
            _navigation = navigation;
            Initialize();
        }

        private ICommand _moveToDetailsCommand;
        public ICommand MoveToDetailsCommand => _moveToDetailsCommand ?? (_moveToDetailsCommand = new Command(GoToDetailsPage_Clicked));

        private void GoToDetailsPage_Clicked()
        {
            _navigation.PushAsync(new DetailsPage());
        }

        private async Task Initialize()
        {
            var location = await GetLocation();
            var installations = await GetInstallations(location, maxResults: 3);
            var data = await GetMeasurementsForInstallations(installations);
            Items = new List<Measurement>(data);

            IsBusy = false;
        }

        private List<Measurement> _items;
        public List<Measurement> Items
        {
            get => _items;
            set => SetProperty(ref _items, value);
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        private async Task<IEnumerable<Installation>> GetInstallations(Location location, double maxDistanceInKm = 2, int maxResults = 1)
        {
            if (location == null)
            {
                System.Diagnostics.Debug.WriteLine("No location data.");
                return null;
            }

            var query = GetQuery(new Dictionary<string, object>
            {
                { "lat", location.Latitude },
                { "lng", location.Longitude },
                { "maxDistanceKM", maxDistanceInKm },
                { "maxResults", maxResults }
            });
            var url = GetApiUrl("installations/nearest", query);

            var response = await GetHttpResponseAsync<IEnumerable<Installation>>(url);
            return response;
        }
        private async Task<IEnumerable<Measurement>> GetMeasurementsForInstallations(IEnumerable<Installation> installations)
        {
            if (installations == null)
            {
                System.Diagnostics.Debug.WriteLine("No installations data.");
                return null;
            }

            var measurements = new List<Measurement>();

            foreach (var installation in installations)
            {
                var query = GetQuery(new Dictionary<string, object>
                {
                    { "installationId", installation.Id }
                });
                var url = GetApiUrl("measurements/installation", query);

                var response = await GetHttpResponseAsync<Measurement>(url);

                if (response != null)
                {
                    response.Installation = installation;
                    response.CurrentDisplayValue = (int)Math.Round(response.Current?.Indexes?.FirstOrDefault()?.Value ?? 0);
                    measurements.Add(response);
                }
            }

            return measurements;
        }

        private string GetQuery(IDictionary<string, object> args)
        {
            if (args == null) return null;

            var query = HttpUtility.ParseQueryString(string.Empty);

            foreach (var arg in args)
            {
                if (arg.Value is double number)
                {
                    query[arg.Key] = number.ToString(CultureInfo.InvariantCulture);
                }
                else
                {
                    query[arg.Key] = arg.Value?.ToString();
                }
            }

            return query.ToString();
        }

        private static HttpClient GetHttpClient()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://airapi.airly.eu/v2/");

            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
            client.DefaultRequestHeaders.Add("apikey", "0Uv0eyZK9L8AvDqYYuZlsTpo8w8z2f7K");
            return client;
        }

        private string GetApiUrl(string path, string query)
        {
            var builder = new UriBuilder("https://airapi.airly.eu/v2/");
            builder.Port = -1;
            builder.Path += path;
            builder.Query = query;
            string url = builder.ToString();

            return url;
        }

        private async Task<T> GetHttpResponseAsync<T>(string url)
        {
            try
            {
                var client = GetHttpClient();
                var response = await client.GetAsync(url);

                if (response.Headers.TryGetValues("X-RateLimit-Limit-day", out var dayLimit) &&
                    response.Headers.TryGetValues("X-RateLimit-Remaining-day", out var dayLimitRemaining))
                {
                    Console.WriteLine($"Day limit: {dayLimit?.FirstOrDefault()}, remaining: {dayLimitRemaining?.FirstOrDefault()}");
                }
                switch ((int)response.StatusCode)
                {
                    case 200:
                        var content = await response.Content.ReadAsStringAsync();
                        Console.WriteLine(content);
                        var result = JsonConvert.DeserializeObject<T>(content);
                        return result;
                    case 429:
                        System.Diagnostics.Debug.WriteLine("Too many requests");
                        break;
                    default:
                        var errorContent = await response.Content.ReadAsStringAsync();
                        System.Diagnostics.Debug.WriteLine($"Response error: {errorContent}");
                        return default;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return default;
        }
        private async Task<Location> GetLocation()
        {
            try
            {
                Location location = await Geolocation.GetLastKnownLocationAsync();

                if (location == null)
                {
                    var request = new GeolocationRequest(GeolocationAccuracy.Medium);
                    location = await Geolocation.GetLocationAsync(request);
                }

                if (location != null)
                {
                    Console.WriteLine($"Latitude: {location.Latitude}, Longitude: {location.Longitude}");
                }

                return location;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return null;
        }

        
    }
}

