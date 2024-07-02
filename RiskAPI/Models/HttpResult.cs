using System.Net;

namespace RiskAPI.Models
{
    public class HttpResult<T>
    {
        public HttpStatusCode Code { get; set;}
        public T? Root { get; set;}
        public string Message { get; set;}

    }
}
