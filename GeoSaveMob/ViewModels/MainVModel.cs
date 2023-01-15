using GeoSaveMob.Classes;
using LCadLau2.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSaveMob.ViewModels
{
    internal class MainVModel:BasicViewModel
    {
        private string _location;
        private RelayCommand getLocation;

        private CancellationTokenSource _cancelTokenSource;
        private bool _isCheckingLocation;

        public MainVModel()
        {
            Location = "Нет данных";
        }
        public string Location { get { return _location; } set { Set(ref _location, value, nameof(Location)); } }

        public RelayCommand GetLocation 
        {
            get { return getLocation ?? (getLocation = new RelayCommand(async () =>
            {
                await GetCurrentLocation();
            })); }
        }


        private async Task GetCurrentLocation()
        {
            try
            {
                _isCheckingLocation = true;

                GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(10));

                _cancelTokenSource = new CancellationTokenSource();

                Location location = await Geolocation.Default.GetLocationAsync(request, _cancelTokenSource.Token);

                if (location != null)
                {
                    Location = $"Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}";
                }
                    
            }
            // Catch one of the following exceptions:
            //   FeatureNotSupportedException
            //   FeatureNotEnabledException
            //   PermissionException
            catch (Exception ex)
            {
                // Unable to get location
            }
            finally
            {
                _isCheckingLocation = false;
            }
        }

        public void CancelRequest()
        {
            if (_isCheckingLocation && _cancelTokenSource != null && _cancelTokenSource.IsCancellationRequested == false)
                _cancelTokenSource.Cancel();
        }

    }
}
