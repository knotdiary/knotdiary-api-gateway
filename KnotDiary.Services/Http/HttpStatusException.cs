using System;
using System.Net;

namespace KnotDiary.Services.Http
{
    public class HttpStatusException : Exception
    {
        public HttpStatusException(HttpStatusCode statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
        }

        public HttpStatusCode StatusCode { get; }
    }
}
