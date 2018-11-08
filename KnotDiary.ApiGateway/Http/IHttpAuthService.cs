using KnotDiary.ApiGateway.Models;
using KnotDiary.Common.Web.Models;
using KnotDiary.Models;
using System.Threading.Tasks;

namespace KnotDiary.ApiGateway.Http
{
    public interface IHttpAuthService
    {
        Task<BaseResponse<User>> GetUser(string authToken);
    }
}
