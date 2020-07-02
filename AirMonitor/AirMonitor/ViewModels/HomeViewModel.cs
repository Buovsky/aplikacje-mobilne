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
            Initialize(false);
        }

        private ICommand _moveToDetailsCommand;
        public ICommand MoveToDetailsCommand => _moveToDetailsCommand ?? (_moveToDetailsCommand = new Command<Measurement>(GoToDetailsPage_Clicked));

        private void GoToDetailsPage_Clicked(Measurement item)
        {
            _navigation.PushAsync(new DetailsPage(item));
        }

        private async Task Initialize(bool forceRefresh)
        {
            IsBusy = true;

            await LoadData(forceRefresh);

            IsBusy = false;
        }

        private async Task LoadData(bool forceRefresh)
        {
            var location = await GetLocation();
            var data = await Task.Run(async () =>
            {
                var installations = await GetInstallations(location, forceRefresh, maxResults: 3);
                return await GetMeasurementsForInstallations(installations, forceRefresh);
            });

            Items = new List<Measurement>(data);
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

        private async Task<IEnumerable<Installation>> GetInstallations(Location location, bool forceRefresh, double maxDistanceInKm = 2, int maxResults = 1)
        {
            if (location == null)
            {
                System.Diagnostics.Debug.WriteLine("No location data.");
                return null;
            }

            IEnumerable<Installation> result;

            var savedMeasurements = App.DbHelper.GetMeasurements();

            if (forceRefresh || ShouldUpdateData(savedMeasurements))
            {

                var query = GetQuery(new Dictionary<string, object>
                {
                    { "lat", location.Latitude },
                    { "lng", location.Longitude },
                    { "maxDistanceKM", maxDistanceInKm },
                    { "maxResults", maxResults }
                });
                var url = GetApiUrl(App.ApiInstallationUrl, query);

                result = await GetHttpResponseAsync<IEnumerable<Installation>>(url);
                App.DbHelper.SaveInstallations(result);
            }
            else
            {
                result = App.DbHelper.GetInstallations();
            }

            return result;

        }
        private async Task<IEnumerable<Measurement>> GetMeasurementsForInstallations(IEnumerable<Installation> installations, bool forceRefresh)
        {
            if (installations == null)
            {
                System.Diagnostics.Debug.WriteLine("No installations data.");
                return null;
            }

            var measurements = new List<Measurement>();
            var savedMeasurements = App.DbHelper.GetMeasurements();

            if (forceRefresh || ShouldUpdateData(savedMeasurements))
            {
                foreach (var installation in installations)
                {
                    var query = GetQuery(new Dictionary<string, object>
                    {
                        { "installationId", installation.Id }
                    });
                    var url = GetApiUrl(App.ApiMeasurementUrl, query);

                    var response = await GetHttpResponseAsync<Measurement>(url);

                    if (response != null)
                    {
                        response.Installation = installation;
                        response.CurrentDisplayValue = (int)Math.Round(response.Current?.Indexes?.FirstOrDefault()?.Value ?? 0);
                        measurements.Add(response);
                    }
                }

                App.DbHelper.SaveMeasurements(measurements);
            }
            else
            {
                measurements = savedMeasurements.ToList();
            }

            foreach (var measurement in measurements)
            {
                measurement.CurrentDisplayValue = (int)Math.Round(measurement.Current?.Indexes?.FirstOrDefault()?.Value ?? 0);
            }


            return measurements;
        }

        private bool ShouldUpdateData(IEnumerable<Measurement> savedMeasurements)
        {
            var isAnyMeasurementOld = savedMeasurements.Any(s => s.Current.TillDateTime.AddMinutes(60) < DateTime.UtcNow);

            return savedMeasurements.Count() == 0 || isAnyMeasurementOld;
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
            client.BaseAddress = new Uri(App.ApiUrl);

            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
            client.DefaultRequestHeaders.Add("apikey", App.ApiKey);
            return client;
        }

        private string GetApiUrl(string path, string query)
        {
            var builder = new UriBuilder(App.ApiUrl);
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

