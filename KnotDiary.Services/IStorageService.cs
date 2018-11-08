using KnotDiary.Models;
using System.Threading.Tasks;

namespace KnotDiary.Services
{
    public interface IStorageService
    {
        Task<string> UploadStreamAsync(string containerPath, MediaUpload media);
    }
}
