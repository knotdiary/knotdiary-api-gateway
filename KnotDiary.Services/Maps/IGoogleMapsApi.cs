using KnotDiary.Models.Maps;
using System.Threading.Tasks;

namespace KnotDiary.Services.Maps
{
    public interface IGoogleMapsApi
    {
        Task<MapsReverseGeoCodeItem> GetAddressByCoordinates(double lat, double lng);
    }
}
