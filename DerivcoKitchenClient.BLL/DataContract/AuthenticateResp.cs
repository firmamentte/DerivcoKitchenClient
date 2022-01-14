namespace DerivcoKitchenClient.BLL.DataContract
{
    public class AuthenticateResp
    {
        public string AccessToken { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}
