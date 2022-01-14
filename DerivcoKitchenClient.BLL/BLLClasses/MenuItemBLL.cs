using DerivcoKitchenClient.BLL.DataContract;

namespace DerivcoKitchenClient.BLL.BLLClasses
{
    public class MenuItemBLL : SharedBLL
    {
        private readonly IHttpClientFactory HttpClientFactory;
        private readonly ApplicationUserBLL ApplicationUserBLL;

        public MenuItemBLL(IHttpClientFactory httpClientFactory)
        {
            HttpClientFactory = httpClientFactory;
            ApplicationUserBLL = new ApplicationUserBLL(httpClientFactory);
        }

        public async Task<MenuItemResp> GetMenuItemByMenuItemId(Guid menuItemId)
        {
            HttpClient _httpClient = CreateHttpClient(HttpClientFactory);
            _httpClient.DefaultRequestHeaders.Add("AccessToken", await ApplicationUserBLL.GetAccessToken());

            string _parameters = "?menuItemId=" + menuItemId;

            using HttpResponseMessage _httpResponseMessage = await _httpClient.GetAsync($"api/MenuItem/V1/GetMenuItemByMenuItemId{_parameters}");

            if (!_httpResponseMessage.IsSuccessStatusCode)
                throw new Exception(ConstructClientError(await _httpResponseMessage.Content.ReadAsAsync<ApiErrorResp>()));

            return await _httpResponseMessage.Content.ReadAsAsync<MenuItemResp>();
        }

        public async Task<MenuItemPaginationResp> GetMenuItemsByCriteria(GetMenuItemsByCriteriaReq getMenuItemsByCriteriaReq)
        {
            HttpClient _httpClient = CreateHttpClient(HttpClientFactory);
            _httpClient.DefaultRequestHeaders.Add("AccessToken", await ApplicationUserBLL.GetAccessToken());

            string _parameters = "?menuCategoryName=" + getMenuItemsByCriteriaReq.MenuCategoryName +
                                 "&name=" + getMenuItemsByCriteriaReq.Name +
                                 "&skip=" + getMenuItemsByCriteriaReq.Skip;

            using HttpResponseMessage _httpResponseMessage = await _httpClient.GetAsync($"api/MenuItem/V1/GetMenuItemsByCriteria{_parameters}");

            if (!_httpResponseMessage.IsSuccessStatusCode)
                throw new Exception(ConstructClientError(await _httpResponseMessage.Content.ReadAsAsync<ApiErrorResp>()));

            return await _httpResponseMessage.Content.ReadAsAsync<MenuItemPaginationResp>();
        }
    }
}
