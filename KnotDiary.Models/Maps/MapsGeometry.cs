using Newtonsoft.Json;

namespace KnotDiary.Models.Maps
{
    public class MapsGeometry
    {
        [JsonProperty("location_type")]
        public string LocationType { get; set; }

        public MapsCoordinates Location { get; set; }
    }
}
