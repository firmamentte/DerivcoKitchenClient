using DerivcoKitchenClient.BLL.DataContract;

namespace DerivcoKitchenClient.BLL.BLLClasses
{
    public class PurchaseOrderBLL : SharedBLL
    {
        private readonly IHttpClientFactory HttpClientFactory;
        private readonly ApplicationUserBLL ApplicationUserBLL;

        public PurchaseOrderBLL(IHttpClientFactory httpClientFactory)
        {
            HttpClientFactory = httpClientFactory;
            ApplicationUserBLL = new ApplicationUserBLL(httpClientFactory);
        }

        public async Task<PurchaseOrderResp> CreatePurchaseOrder(string emailAddress, List<LineItemReq> lineItemReqs)
        {
            HttpClient _httpClient = CreateHttpClient(HttpClientFactory);
            _httpClient.DefaultRequestHeaders.Add("AccessToken", await ApplicationUserBLL.GetAccessToken());
            _httpClient.DefaultRequestHeaders.Add("EmailAddress", emailAddress);

            using HttpResponseMessage _httpResponseMessage = await _httpClient.PostAsJsonAsync($"api/PurchaseOrder/V1/CreatePurchaseOrder", lineItemReqs);

            if (!_httpResponseMessage.IsSuccessStatusCode)
                throw new Exception(ConstructClientError(await _httpResponseMessage.Content.ReadAsAsync<ApiErrorResp>()));

            return await _httpResponseMessage.Content.ReadAsAsync<PurchaseOrderResp>();
        }
    }
}
