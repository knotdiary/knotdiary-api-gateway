using Newtonsoft.Json;
using System.Collections.Generic;

namespace KnotDiary.Models.Maps
{
    public class MapsAddressComponent
    {
        [JsonProperty("long_name")]
        public string LongName { get; set; }

        [JsonProperty("short_name")]
        public string ShortName { get; set; }

        public List<string> Types { get; set; }
    }
}
