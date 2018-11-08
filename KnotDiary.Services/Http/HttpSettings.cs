namespace KnotDiary.Services.Http
{
    public abstract class HttpSettings
    {
        public string BaseUrl { get; set; }

        public int TimeoutInMilliseconds { get; set; } = 2000;

        public string AuthKeyName { get; set; }
    }
}
