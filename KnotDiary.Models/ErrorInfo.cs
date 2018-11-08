using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace KnotDiary.Models
{
    public class ErrorInfo<TErrorType> where TErrorType : struct
    {
        public ErrorInfo(TErrorType errorType)
        {
            if (!typeof(TErrorType).IsEnum)
            {
                throw new ArgumentException("Argument must be an enum type");
            }

            ErrorType = errorType;
        }

        public ErrorInfo(TErrorType errorType, string message)
        {
            if (!typeof(TErrorType).IsEnum)
            {
                throw new ArgumentException("Argument must be an enum type");
            }

            ErrorType = errorType;
            Message = message;
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public TErrorType ErrorType { get; set; }

        public string Message { get; set; }
    }
}
