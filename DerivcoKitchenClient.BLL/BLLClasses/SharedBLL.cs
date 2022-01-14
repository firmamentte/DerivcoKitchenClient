using DerivcoKitchenClient.BLL.DataContract;

namespace DerivcoKitchenClient.BLL.BLLClasses
{
    public abstract class SharedBLL
    {
        public string ConstructClientError(ApiErrorResp apiErrorResp)
        {
            int _counter = apiErrorResp.Errors.Count();
            string _errorMessage = "\n";

            foreach (string error in apiErrorResp.Errors)
            {
                if (_counter > 1)
                    _errorMessage += $"→ {error}.\n";
                else
                    _errorMessage += $"{error}.\n";
            }

            return _errorMessage;
        }

        public HttpClient CreateHttpClient(IHttpClientFactory httpClientFactory)
        {
            HttpClient _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri(StaticClass.DerivcoKitchenWebAPIBaseAddress);
            _httpClient.Timeout = TimeSpan.FromMilliseconds(-1);
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(StaticClass.MediaTypeWithQualityHeaderValue);

            return _httpClient;
        }
    }
}
