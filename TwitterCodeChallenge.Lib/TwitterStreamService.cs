using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TwitterCodeChallenge.Core;

namespace TwitterCodeChallenge.Lib
{

    public class TwitterStreamService : TwitterServiceBase, ITwitterStreamService
    {
        public TwitterStreamService(ILogger<TwitterStreamService> logger, HttpClient httpClient) : base(logger, httpClient)
        {
        }

        public List<string> GetHashTags(string tweet)
        {
            var hashTagPattern = $"(^|\\s)#([A-Za-z_][A-Za-z0-9_]*)";
            if (string.IsNullOrEmpty(tweet)) return new List<string>();
            var regEx = new Regex(hashTagPattern, RegexOptions.IgnoreCase);
            var matches = regEx.Matches(tweet).Cast<Match>().Select(x => x.Value);
            return matches?.ToList();
        }

        public async Task<Tweet> GetSampleStreamAsync()
        {
            Tweet tweet = new Tweet();
            var req = new HttpRequestMessage(HttpMethod.Get, "2/tweets/sample/stream");

            try
            {
                using var resp = await HttpClient.SendAsync(req, HttpCompletionOption.ResponseHeadersRead);
                resp.EnsureSuccessStatusCode();
                await using var stream = await resp.Content.ReadAsStreamAsync();

                using (var reader = new StreamReader(stream))
                {
                    using var jReader = new JsonTextReader(reader);
                    JsonSerializer jsonSerializer = new JsonSerializer();

                    if (jsonSerializer.Deserialize(jReader) is JObject o)
                    {
                        var tweetData = o?.SelectToken("data.text")?.Value<string>();
                        Logger?.LogInformation(tweetData);
                        tweet.Data = tweetData;
                    }
                };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                tweet.ErrorMessage = ex.Message;
            }
            return tweet;
        }
    }
}
