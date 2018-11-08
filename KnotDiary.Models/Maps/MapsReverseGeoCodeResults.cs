using System.Collections.Generic;

namespace KnotDiary.Models.Maps
{
    public class MapsReverseGeoCodeResults
    {
        public List<MapsReverseGeoCodeItem> Results { get; set; }

        public string Status { get; set; }
    }
}
