using KnotDiary.Models.Maps;
using System.Collections.Generic;
using System.Linq;

namespace KnotDiary.Models
{
    public class Location
    {
        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string Premise { get; set; }

        public string Name { get; set; }

        public string Number { get; set; }
        
        public string Street { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Country { get; set; }

        public static Location ToLocation(MapsReverseGeoCodeItem result)
        {
            if (result == null || result.AddressComponents?.Count == 0 || result?.Geometry?.Location == null)
            {
                return null;
            }

            var premise = GetAddressPremise(result.AddressComponents);
            var bldgNumber = result.AddressComponents.FirstOrDefault(a => a.Types.Contains("street_number"));
            var street = result.AddressComponents.FirstOrDefault(a => a.Types.Contains("route"));
            var city = result.AddressComponents.FirstOrDefault(a => a.Types.Contains("locality"));
            var state = result.AddressComponents.FirstOrDefault(a => a.Types.FirstOrDefault(t => t.Contains("administrative_area_level")) != null);
            var country = result.AddressComponents.FirstOrDefault(a => a.Types.Contains("country"));

            return new Location
            {
                Premise = premise?.LongName,
                Number = bldgNumber?.LongName,
                State = state?.LongName,
                Street = street?.LongName,
                City = city?.LongName,
                Country = country?.LongName,
                Latitude = result.Geometry.Location.Lat,
                Longitude = result.Geometry.Location.Lng,
            };
        }

        private static MapsAddressComponent GetAddressPremise(List<MapsAddressComponent> addressComponents)
        {
            var premiseResult = addressComponents.FirstOrDefault(a => a.Types.Contains("premise"));
            if (premiseResult != null)
            {
                return premiseResult;
            }

            var poiResult = addressComponents.FirstOrDefault(a => a.Types.Contains("point_of_interest"));
            if (poiResult != null)
            {
                return poiResult;
            }

            var establishmentResult = addressComponents.FirstOrDefault(a => a.Types.Contains("establishment"));
            if (establishmentResult != null)
            {
                return establishmentResult;
            }

            return null;
        }
    }
}
