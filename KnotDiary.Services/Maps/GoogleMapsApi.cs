using KnotDiary.Common;
using KnotDiary.Models.Maps;
using KnotDiary.Services.Http;
using Serilog;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace KnotDiary.Services.Maps
{
    public class GoogleMapsApi : IGoogleMapsApi
    {
        private readonly ILogger _logger;
        private readonly HttpClientWrapper _httpClient;
        private readonly GoogleMapsHttpSettings _settings;
        private readonly ICacheManager _cacheManager;
        
        public GoogleMapsApi(ILogger logger, GoogleMapsHttpSettings httpSettings, ICacheManager cacheManager)
        {
            _logger = logger;
            _settings = httpSettings;
            _httpClient = new HttpClientWrapper(_logger, httpSettings.BaseUrl, httpSettings.TimeoutInMilliseconds);
            _cacheManager = cacheManager;
        }

        public async Task<MapsReverseGeoCodeItem> GetAddressByCoordinates(double lat, double lng)
        {
            try
            {
                var response = await _httpClient.GetAsync<MapsReverseGeoCodeResults>($"maps/api/geocode/json?latlng={lat},{lng}");
                if (response == null)
                {
                    throw new Exception($"Google Maps response is null - lat: {lat} - lng: {lng}");
                }

                var premiseResult = response.Results.FirstOrDefault(a => a.Types.Contains("premise"));
                if (premiseResult != null)
                {
                    return premiseResult;
                }

                var poiResult = response.Results.FirstOrDefault(a => a.Types.Contains("point_of_interest"));
                if (poiResult != null)
                {
                    return poiResult;
                }

                var establishmentResult = response.Results.FirstOrDefault(a => a.Types.Contains("establishment"));
                if (establishmentResult != null)
                {
                    return establishmentResult;
                }

                var streetAddressResult = response.Results.FirstOrDefault(a => a.Types.Contains("street_address"));
                if (streetAddressResult == null)
                {
                    throw new Exception($"No address result found for coordinates - lat: {lat} - lng: {lng}");
                }

                return streetAddressResult;
            }
            catch (Exception e)
            {
                _logger.Error(e, "Google Maps API - GetAddressByCoordinates - Failed to get address");
                return null;
            }
        }
    }
}
