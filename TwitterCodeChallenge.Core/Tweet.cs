namespace TwitterCodeChallenge.Core
{
    public class Tweet
    {
        public string Data { get; set; }
        public string ErrorMessage { get; set; }

        public bool HasError { get { return !string.IsNullOrEmpty(ErrorMessage); } }
    }
}
