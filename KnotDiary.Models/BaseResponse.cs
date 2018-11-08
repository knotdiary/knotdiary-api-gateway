namespace KnotDiary.Models
{
    public class BaseResponse<T>
    {
        public BaseResponse() { }

        public BaseResponse(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public BaseResponse(bool isSuccess, string message)
        {
            IsSuccess = isSuccess;
            Message = message;
        }

        public BaseResponse(bool isSuccess, string message, T data)
        {
            IsSuccess = isSuccess;
            Message = message;
            Data = data;
        }

        public bool IsSuccess { get; set; }
        
        public string Message { get; set; }

        public T Data { get; set; }
    }
}
