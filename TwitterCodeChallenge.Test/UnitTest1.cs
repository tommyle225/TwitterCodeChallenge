using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Text.RegularExpressions;

namespace TwitterCodeChallenge.Test
{
    [TestClass]
    public class UnitTest1
    {
        private string token= "AAAAAAAAAAAAAAAAAAAAABV1fAEAAAAASA%2FQZ0SJVwxbTRiJE12IH2YzInM%3DjiBt5vfWqioRnx4gzZ8ku6l80PVg6xVE00h3rZGkqtbqolfzgM";
        private string api= "https://api.twitter.com/";

        

        [TestMethod]
        public void TestHashTag()
        {
            var hashTagPattern = $"(^|\\s)#([A-Za-z_][A-Za-z0-9_]*)";

            var teststring = "This is a #hashtag test";
            var regEx = new Regex(hashTagPattern, RegexOptions.IgnoreCase);
            
            var matches = regEx.Matches(teststring).Cast<Match>().Select(x => x.Value);
            Assert.IsNotNull(matches);

            var notAhashTag = "this is not @hashtag. # not a hash tag";
            
            matches = regEx.Matches(notAhashTag).Cast<Match>().Select(x => x.Value);
            Assert.IsTrue(matches.Any()==false);
        }
    }
}

