using Microsoft.Extensions.Logging;
using System.Net.Http;

namespace TwitterCodeChallenge.Lib
{
    public abstract class TwitterServiceBase
    {
        public TwitterServiceBase(ILogger<TwitterServiceBase> logger, HttpClient httpClient)
        {
            Logger = logger;
            HttpClient = httpClient;
        }
        protected ILogger<TwitterServiceBase> Logger { get; }
        protected HttpClient HttpClient { get; set; }
    }
}
