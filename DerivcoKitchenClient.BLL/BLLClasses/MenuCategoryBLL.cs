using DerivcoKitchenClient.BLL.DataContract;

namespace DerivcoKitchenClient.BLL.BLLClasses
{
    public class MenuCategoryBLL : SharedBLL
    {
        private readonly IHttpClientFactory HttpClientFactory;
        private readonly ApplicationUserBLL ApplicationUserBLL;

        public MenuCategoryBLL(IHttpClientFactory httpClientFactory)
        {
            HttpClientFactory = httpClientFactory;
            ApplicationUserBLL = new ApplicationUserBLL(httpClientFactory);
        }

        public async Task<List<MenuCategoryResp>> GetMenuCategories()
        {
            HttpClient _httpClient = CreateHttpClient(HttpClientFactory);
            _httpClient.DefaultRequestHeaders.Add("AccessToken", await ApplicationUserBLL.GetAccessToken());

            using HttpResponseMessage _httpResponseMessage = await _httpClient.GetAsync("api/MenuCategory/V1/GetMenuCategories");

            if (!_httpResponseMessage.IsSuccessStatusCode)
                throw new Exception(ConstructClientError(await _httpResponseMessage.Content.ReadAsAsync<ApiErrorResp>()));

            return await _httpResponseMessage.Content.ReadAsAsync<List<MenuCategoryResp>>();
        }
    }
}
