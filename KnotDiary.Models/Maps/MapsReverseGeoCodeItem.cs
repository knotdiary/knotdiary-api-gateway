using Newtonsoft.Json;
using System.Collections.Generic;

namespace KnotDiary.Models.Maps
{
    public class MapsReverseGeoCodeItem
    {
        [JsonProperty("address_components")]
        public List<MapsAddressComponent> AddressComponents { get; set; }

        [JsonProperty("formatted_address")]
        public string FormattedAddress { get; set; }
        
        public MapsGeometry Geometry { get; set; }

        public List<string> Types { get; set; }
    }
}
