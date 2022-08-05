using System.Collections.Generic;
using System.Threading.Tasks;

namespace TwitterCodeChallenge.Core
{
    public interface ITwitterStreamService
    {
        Task<Tweet> GetSampleStreamAsync();
        List<string> GetHashTags(string tweet);
    }
}
