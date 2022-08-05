using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;
using Polly;
using Polly.Extensions.Http;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using TwitterCodeChallenge.Core;
using TwitterCodeChallenge.Lib;

namespace TwitterCodeChallenge.ConsoleApp
{
    class Program
    {
        static string consumerKey = "Lcb7poeARRShjQPanmlRfi6S0";
        static readonly string _bearerToken = ConfigurationManager.AppSettings["TWITTER_BEARER_TOKEN"];
        static readonly string _apiUrl = ConfigurationManager.AppSettings["TWITTER_API_URL"];
        static string consumerSecret = "5NQJHdOPTvg7hchuPPZYn2QVyEHUbwpRqJUXn2mYjfePwSzV2d";
        public static int TweetCount { get; set; }
        public static List<string> HashTags { get; set; } = new List<string>();
        private static ITwitterStreamService _twitterStreamService;

        static async Task Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            _twitterStreamService = serviceProvider.GetRequiredService<ITwitterStreamService>();
            int delay = 3000;
            Console.WriteLine("Press any key to exit ...");

            do
            {
                var tweet = await GetTweetAsync();

                if (!tweet.HasError)
                {
                    TweetCount++;
                    HashTags.AddRange(_twitterStreamService.GetHashTags(tweet.Data));
                }
                else
                {
                    Thread.Sleep(delay);
                }

                LogResults();


            } while (!Console.KeyAvailable);

        }

        private static void ConfigureServices(ServiceCollection services)
        {
            services.AddLogging(config =>
            {
                config.AddDebug();
            })
            .Configure<LoggerFilterOptions>(options =>
            {
                options.AddFilter<DebugLoggerProvider>(null, LogLevel.Information);
            })
            .AddTransient<ITwitterStreamService, TwitterStreamService>()
            .AddHttpClient<ITwitterStreamService, TwitterStreamService>(client =>
            {
                client.BaseAddress = new Uri(_apiUrl);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _bearerToken);
                client.DefaultRequestHeaders.Accept.Clear();

            }).AddPolicyHandler(x =>
            {
                return HttpPolicyExtensions.HandleTransientHttpError()
                                           .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                                           .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
            });
        }
        private static void LogResults()
        {
            Console.SetCursorPosition(0, 1);
            string msg = $"Tweet Count: {TweetCount} \r\nTop Hashtags: {string.Join(",", HashTags?.Distinct().Take(10))}";
            Console.Write(msg);
        }

        private static async Task<Tweet> GetTweetAsync()
        {
            var response = await _twitterStreamService.GetSampleStreamAsync();
            return response;
        }
    }
}
