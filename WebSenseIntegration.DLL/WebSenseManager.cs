

namespace Siemplify.Integrations.WebSense
{
    public class WebSenseManager
    {
        public const string DefaultApiHost = @"https://10.0.0.2:15873/web/api/v1/";

        private readonly string _apiUsername;
        private readonly string _apiKey;

        public WebSenseManager(string apiUsername, string apiKey, string apiHost = DefaultApiHost)
        {
            _apiUsername = apiUsername;
            _apiKey = apiKey;
        }
        
    }
}
